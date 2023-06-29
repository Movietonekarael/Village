using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Burst;
using System;
using Unity.Collections.LowLevel.Unsafe;
using System.ComponentModel;

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
        private NativeArray<int>[] _pass;
        private NativeArray<double>[] _accumulations;
        private NativeArray<Vector3>[] _offsets;

        private JiggleRigJob[] _jobs;
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
                _pass[i].Dispose();
                _accumulations[i].Dispose();
                _offsets[i].Dispose();
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

        private void CreateArrays()
        {
            var rigCount = JiggleRigs.Count;

            _jobs = new JiggleRigJob[rigCount];
            _jobHandles = new JobHandle[rigCount];
            _accessArrays = new TransformAccessArray[rigCount];
            _jiggleBonesArrays = new NativeArray<JiggleBone>[rigCount];
            _pass = new NativeArray<int>[rigCount];
            _accumulations = new NativeArray<double>[rigCount];
            _offsets = new NativeArray<Vector3>[rigCount];

            for (var i = 0; i < rigCount; i++)
            {
                _accessArrays[i] = new TransformAccessArray(JiggleRigs[i].BonesTransforms.ToArray(), 1);
                _jiggleBonesArrays[i] = new NativeArray<JiggleBone>(JiggleRigs[i].Bones.ToArray(), Allocator.Persistent);
                _pass[i] = new NativeArray<int>(1, Allocator.Persistent);
                _pass[i][0] = 1;
                _accumulations[i] = new NativeArray<double>(1, Allocator.Persistent);
                _accumulations[i][0] = 0;
                _offsets[i] = new NativeArray<Vector3>(1, Allocator.Persistent);
                _offsets[i][0] = Vector3.zero;

                _jobs[i] = new JiggleRigJob(_jiggleBonesArrays[i],
                                            Wind,
                                            Physics.gravity,
                                            JiggleRigs[i].JiggleSettings.GetSettingsStruct(),
                                            _pass[i],
                                            _accumulations[i],
                                            _offsets[i]);
            }

            JiggleRigs = null;
            _jiggleRigLookup = null;
        }

        public void AddRig(JiggleRig rig)
        {
            JiggleRigs.Add(rig);
        }

        public JiggleRigJob GetJob(int index)
        {
            return _jobs[index];
        }

        private void HandleJob()
        {
            var timeAsDouble = Time.timeAsDouble;
            var fixedDeltaTime = Time.fixedDeltaTime;
            var deltaTime = Time.deltaTime;

            for (var i = 0; i < _jobHandles.Length; i++)
            {
                _jobs[i].Time = timeAsDouble;
                _jobs[i].FixedDeltaTime = fixedDeltaTime;
                _jobs[i].Accumulation = System.Math.Min(_jobs[i].Accumulation + deltaTime, fixedDeltaTime * 4f);
                var handle1 = _jobs[i].Schedule(_accessArrays[i]);
                var handle2 = _jobs[i].Schedule(handle1);
                var handle3 = _jobs[i].Schedule(_accessArrays[i], handle2);
                _jobHandles[i] = _jobs[i].Schedule(_accessArrays[i], handle3);
            }
        }

        public void SetTimeVariables()
        {
            var timeAsDouble = Time.timeAsDouble;
            var fixedDeltaTime = Time.fixedDeltaTime;
            var deltaTime = Time.deltaTime;

            for (var i = 0; i < _jobHandles.Length; i++)
            {
                _jobs[i].Time = timeAsDouble;
                _jobs[i].FixedDeltaTime = fixedDeltaTime;
                _jobs[i].Accumulation = System.Math.Min(_jobs[i].Accumulation + deltaTime, fixedDeltaTime * 4f);
            }
        }

        public void ScheduleTransformJobs(int pass)
        {
            for (var i = 0; i < _jobHandles.Length; i++)
            {
                _jobs[i].PassValue = pass;
                _jobHandles[i] = _jobs[i].Schedule(_accessArrays[i]);
            }
        }

        public void ScheduleJobs()
        {
            for (var i = 0; i < _jobHandles.Length; i++)
            {
                _jobHandles[i] = _jobs[i].Schedule();
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

