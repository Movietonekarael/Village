using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;


namespace JiggleBones
{
    public partial class JiggleBuilder : MonoBehaviour
    {
        public List<JiggleRig> JiggleRigs;
        private Dictionary<Transform, JiggleRig> _jiggleRigLookup;

        [Tooltip("An air force that is applied to the entire rig, this is useful to plug in some wind volumes from external sources.")]
        public Vector3 Wind;

        private TransformAccessArray[] _accessArrays;
        private NativeArray<JiggleBone>[] _jiggleBonesArrays;
        public NativeArray<JiggleBone>[] JiggleBonesArrays => _jiggleBonesArrays;
        private JiggleSettingsBase[] _settings;
        private double[] _accumulations;
        private Vector3[] _offsets;
        private double _time;
        private float _fixedDeltaTime;
        private Vector3 _gravity;

        private JiggleRigPreparationJob[] _preparationJobs;
        private JiggleRigSimulationJob[] _simulationJobs;
        private JiggleRigDerivingPositionJob[] _derivingPositionJobs;
        private JiggleRigPosingJob[] _posingJobs;
        private JobHandle[] _jobHandles;

        [HideInInspector] public bool SetupOnAwake = true;
        [HideInInspector] public bool AllowUpdate = true;


        private void Awake()
        {
            if (SetupOnAwake)
                Setup();
        }

        private void Update()
        {
            if (AllowUpdate)
            {
                WaitForJobs();
            }
        }

        private void LateUpdate()
        {
            if (AllowUpdate)
            {
                HandleJob();
            }
        }

        private void OnDestroy()
        {
            for (var i = 0; i < _jiggleBonesArrays.Length; i++)
            {
                _jiggleBonesArrays[i].Dispose();
                _accessArrays[i].Dispose();
            }
        }

        public void Setup()
        {
            _jiggleRigLookup ??= new Dictionary<Transform, JiggleRig>();
            _jiggleRigLookup.Clear();
            JiggleRigs ??= new List<JiggleRig>();

            foreach (JiggleRig rig in JiggleRigs)
            {
                if (_jiggleRigLookup.ContainsKey(rig.RootTransform))
                {
                    throw new UnityException("JiggleRig was added to transform where one already exists!");
                }
                _jiggleRigLookup.Add(rig.RootTransform, rig);

                rig.Bones = new List<JiggleBone>();
                CreateBones(rig, rig.RootTransform, null);

                rig.BonesTransforms.Insert(0, rig.BonesTransforms[0].parent);
                rig.Bones.Insert(0, new JiggleBone(rig.BonesTransforms[0], 0, null, rig.BonesTransforms[0].position, rig.Bones));

                for (var i = 1; i < rig.Bones.Count; i++)
                {
                    var bone = rig.Bones[i];
                    bone.IncreaseIndex();
                    if (i == 1)
                    {
                        bone.SetParentIndex(0);
                    }
                    rig.Bones[i] = bone;
                }
            }
            CreateArrays();
        }

        private void CreateBones(JiggleRig rig, Transform currentTransform, int? parentJiggleBoneIndex)
        {
            var newJiggleBone = new JiggleBone(currentTransform, 
                                               rig.Bones.Count, 
                                               parentJiggleBoneIndex, 
                                               currentTransform.position, 
                                               rig.Bones);
            rig.Bones.Add(newJiggleBone);
            rig.BonesTransforms.Add(currentTransform);

            if (currentTransform.childCount == 0)
            {
                if (newJiggleBone.ParentIndex == null)
                {
                    if (currentTransform.parent == null)
                    {
                        throw new UnityException("Can't have a singular jiggle bone with no parents. That doesn't even make sense!");
                    }
                    else
                    {
                        var lengthToParent = Vector3.Distance(currentTransform.position, currentTransform.parent.position);
                        var projectedForwardReal = (currentTransform.position - currentTransform.parent.position).normalized;
                        rig.Bones.Add(new JiggleBone(null, 
                                                     rig.Bones.Count, 
                                                     newJiggleBone.Index, 
                                                     currentTransform.position + projectedForwardReal * lengthToParent,
                                                     rig.Bones));
                        rig.BonesTransforms.Add(null);
                        return;
                    }
                }
                var parentTransform = rig.BonesTransforms[parentJiggleBoneIndex.Value];
                var projectedForward = (currentTransform.position - parentTransform.position).normalized;
                var length = 0.1f;
                if (rig.Bones[parentJiggleBoneIndex.Value].ParentIndex != null)
                {
                    length = Vector3.Distance(parentTransform.position, parentTransform.parent.transform.position);
                }
                rig.Bones.Add(new JiggleBone(null,
                                             rig.Bones.Count,
                                             newJiggleBone.Index, 
                                             currentTransform.position + projectedForward * length,
                                             rig.Bones));
                rig.BonesTransforms.Add(null);
                return;
            }

            for (var i = 0; i < currentTransform.childCount; i++)
            {
                if (rig.IgnoredTransforms.Contains(currentTransform.GetChild(i)))
                {
                    continue;
                }
                CreateBones(rig, currentTransform.GetChild(i), newJiggleBone.Index);
            }
        }

        private unsafe void CreateArrays()
        {
            var rigCount = JiggleRigs.Count;


            _preparationJobs = new JiggleRigPreparationJob[rigCount];
            _simulationJobs = new JiggleRigSimulationJob[rigCount];
            _derivingPositionJobs = new JiggleRigDerivingPositionJob[rigCount];
            _posingJobs = new JiggleRigPosingJob[rigCount];
            _jobHandles = new JobHandle[rigCount];
            _accessArrays = new TransformAccessArray[rigCount];
            _jiggleBonesArrays = new NativeArray<JiggleBone>[rigCount];
            _settings = new JiggleSettingsBase[rigCount];
            _accumulations = new double[rigCount];
            _offsets = new Vector3[rigCount];
            _time = Time.timeAsDouble;
            _fixedDeltaTime = Time.fixedDeltaTime;
            _gravity = Physics.gravity;


            for (var i = 0; i < rigCount; i++)
            {
                _accessArrays[i] = new TransformAccessArray(JiggleRigs[i].BonesTransforms.ToArray(), 1);
                _jiggleBonesArrays[i] = new NativeArray<JiggleBone>(JiggleRigs[i].Bones.ToArray(), Allocator.Persistent);
                _settings[i] = JiggleRigs[i].JiggleSettings.GetSettingsStruct();
                _accumulations[i] = 0;
                _offsets[i] = Vector3.zero;


                fixed (double* time = &_time)
                fixed (float* fixedDeltaTime = &_fixedDeltaTime)
                fixed (double* accumulation = &_accumulations[i])
                fixed (Vector3* wind = &Wind)
                fixed (Vector3* gravity = &_gravity)
                fixed (JiggleSettingsBase* settings = &_settings[i])
                fixed (Vector3* offset = &_offsets[i])
                {
                    _preparationJobs[i] = new JiggleRigPreparationJob(_jiggleBonesArrays[i], time);
                    _simulationJobs[i] = new JiggleRigSimulationJob(_jiggleBonesArrays[i],
                                                                    accumulation,
                                                                    time,
                                                                    fixedDeltaTime,
                                                                    wind,
                                                                    gravity,
                                                                    settings);
                    _derivingPositionJobs[i] = new JiggleRigDerivingPositionJob(_jiggleBonesArrays[i],
                                                                                time,
                                                                                fixedDeltaTime,
                                                                                offset);
                    _posingJobs[i] = new JiggleRigPosingJob(_jiggleBonesArrays[i], settings);
                }
            }
            JiggleRigs = null;
            _jiggleRigLookup = null;
        }

        public void AddRig(JiggleRig rig)
        {
            JiggleRigs ??= new();
            JiggleRigs.Add(rig);
        }

        private void HandleJob()
        {
            _time = Time.timeAsDouble;
            _fixedDeltaTime = Time.fixedDeltaTime;
            var deltaTime = Time.deltaTime;

            for (var i = 0; i < _jobHandles.Length; i++)
            {
                _accumulations[i] = System.Math.Min(_accumulations[i] + deltaTime, _fixedDeltaTime * 4f);
                var handle1 = _preparationJobs[i].Schedule(_accessArrays[i]);
                var handle2 = _simulationJobs[i].Schedule(handle1);
                var handle3 = _derivingPositionJobs[i].Schedule(_accessArrays[i], handle2);
                _jobHandles[i] = _posingJobs[i].Schedule(_accessArrays[i], handle3);
            }
        }

        public void SetTimeVariables()
        {
            _time = Time.timeAsDouble;
            _fixedDeltaTime = Time.fixedDeltaTime;
            var deltaTime = Time.deltaTime;

            for (var i = 0; i < _jobHandles.Length; i++)
            {
                _accumulations[i] = System.Math.Min(_accumulations[i] + deltaTime, _fixedDeltaTime * 4f);
            }
        }

        public void SchedulePreparationJobs()
        {
            for (var i = 0; i < _jobHandles.Length; i++)
            {
                _jobHandles[i] = _preparationJobs[i].Schedule(_accessArrays[i]);
            }
        }

        public void SchedulesSimulateJobs()
        {
            for (var i = 0; i < _jobHandles.Length; i++)
            {
                _jobHandles[i] = _simulationJobs[i].Schedule();
            }
        }

        public void ScheduleDerivingJobs()
        {
            for (var i = 0; i < _jobHandles.Length; i++)
            {
                _jobHandles[i] = _derivingPositionJobs[i].Schedule(_accessArrays[i]);
            }
        }

        public void SchedulePosingJobs()
        {
            for (var i = 0; i < _jobHandles.Length; i++)
            {
                _jobHandles[i] = _posingJobs[i].Schedule(_accessArrays[i]);
            }
        }

        public void WaitForJobs()
        {
            for (var i = 0; i < _jobHandles.Length; i++)
            {
                _jobHandles[i].Complete();
            }
        }
    }
}

