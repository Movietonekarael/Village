using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Burst;
using System;
using JetBrains.Annotations;

namespace JiggleBones
{
    

    [System.Serializable]
    public class JiggleRig
    {
        [Tooltip("The root bone from which an individual JiggleRig will be constructed. The JiggleRig encompasses all children of the specified root.")]
        public Transform rootTransform;
        [Tooltip("The settings that the rig should update with, create them using the Create->JigglePhysics->Settings menu option.")]
        public JiggleSettings jiggleSettings;
        [Tooltip("The list of transforms to ignore during the jiggle. Each bone listed will also ignore all the children of the specified bone.")]
        public List<Transform> ignoredTransforms;
        [HideInInspector] public List<JiggleBone> bones;
        [HideInInspector] public List<Transform> bonesTransforms;

        public JiggleRig(Transform _rootTransform, JiggleSettings _jiggleSettings)
        {
            rootTransform = _rootTransform;
            jiggleSettings = _jiggleSettings;
            ignoredTransforms = new();
            bones = new();
            bonesTransforms = new();
        }
    }

    public class JiggleBuilder : MonoBehaviour
    {
        public List<JiggleRig> _jiggleRigs;
        private Dictionary<Transform, JiggleRig> _jiggleRigLookup;

        [Tooltip("An air force that is applied to the entire rig, this is useful to plug in some wind volumes from external sources.")]
        public Vector3 wind;


        [SerializeField] private bool _setupOnAwake = false;
        public bool AllowUpdate = true;

        private void Awake()
        {
            //accumulation = 0f;

            if (_setupOnAwake)
                Setup();
        }

        public void Setup()
        {
            _jiggleRigLookup ??= new Dictionary<Transform, JiggleRig>();
            _jiggleRigLookup.Clear();
            _jiggleRigs ??= new List<JiggleRig>();

            foreach (JiggleRig rig in _jiggleRigs)
            {
                if (_jiggleRigLookup.ContainsKey(rig.rootTransform))
                {
                    throw new UnityException("JiggleRig was added to transform where one already exists!");
                }
                _jiggleRigLookup.Add(rig.rootTransform, rig);

                rig.bones = new List<JiggleBone>();
                CreateBones(rig, rig.rootTransform, null);

                rig.bonesTransforms.Insert(0, rig.bonesTransforms[0].parent);
                rig.bones.Insert(0, new JiggleBone(rig.bonesTransforms[0], 0, null, rig.bonesTransforms[0].position, rig.bones));

                for (var i = 1; i < rig.bones.Count; i++)
                {
                    var bone = rig.bones[i];
                    bone.IncreaseIndex();
                    if (i == 1)
                    {
                        bone.SetParentIndex(0);
                    }
                    rig.bones[i] = bone;
                }


            }

            CreateArrays();
        }

        private void CreateBones(JiggleRig rig, Transform currentTransform, int? parentJiggleBoneIndex)
        {
            JiggleBone newJiggleBone = new JiggleBone(currentTransform, 
                                                      rig.bones.Count, 
                                                      parentJiggleBoneIndex, 
                                                      currentTransform.position, 
                                                      rig.bones);
            rig.bones.Add(newJiggleBone);
            rig.bonesTransforms.Add(currentTransform);

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
                        float lengthToParent = Vector3.Distance(currentTransform.position, currentTransform.parent.position);
                        Vector3 projectedForwardReal = (currentTransform.position - currentTransform.parent.position).normalized;
                        rig.bones.Add(new JiggleBone(null, 
                                                     rig.bones.Count, 
                                                     newJiggleBone.Index, 
                                                     currentTransform.position + projectedForwardReal * lengthToParent,
                                                     rig.bones));
                        rig.bonesTransforms.Add(null);
                        return;
                    }
                }
                var parentTransform = rig.bonesTransforms[parentJiggleBoneIndex.Value];
                Vector3 projectedForward = (currentTransform.position - parentTransform.position).normalized;
                float length = 0.1f;
                if (rig.bones[parentJiggleBoneIndex.Value].ParentIndex != null)
                {
                    length = Vector3.Distance(parentTransform.position, parentTransform.parent.transform.position);
                }
                rig.bones.Add(new JiggleBone(null,
                                             rig.bones.Count,
                                             newJiggleBone.Index, 
                                             currentTransform.position + projectedForward * length,
                                             rig.bones));
                rig.bonesTransforms.Add(null);
                return;
            }

            for (int i = 0; i < currentTransform.childCount; i++)
            {
                if (rig.ignoredTransforms.Contains(currentTransform.GetChild(i)))
                {
                    continue;
                }
                CreateBones(rig, currentTransform.GetChild(i), newJiggleBone.Index);
            }
        }

        [BurstCompile]
        public struct JiggleRigJob : IJobParallelForTransform, IJob
        {
            [NativeDisableContainerSafetyRestriction] public NativeArray<JiggleBone> Bones;
            [ReadOnly] public double Time;
            [ReadOnly] public float FixedDeltaTime;
            [ReadOnly] public JiggleSettingsBase JiggleSettings;
            [ReadOnly] public Vector3 Wind;
            [ReadOnly] private const float _smoothing = 1f;
            [ReadOnly] public Vector3 Gravity;

            [NativeDisableContainerSafetyRestriction] public NativeArray<int> pass;
            [NativeDisableContainerSafetyRestriction] public NativeArray<double> accumulationArray;
            [NativeDisableContainerSafetyRestriction] public NativeArray<Vector3> offsetArray;

            public int PassValue { get{ return pass[0]; } set{ pass[0] = value; } }
            public double Accumulation { get{ return accumulationArray[0]; } set{ accumulationArray[0] = value; } }
            private Vector3 _offset { get { return offsetArray[0]; } set { offsetArray[0] = value; } }

            public void Execute()
            {
                while (Accumulation > FixedDeltaTime)
                {
                    Accumulation -= FixedDeltaTime;
                    var accumulationTime = Time - Accumulation;
                    for (var i = 1; i < Bones.Length; i++)
                    {
                        var bone = Bones[i];
                        bone.Simulate(ref JiggleSettings, ref Wind, Gravity, accumulationTime, FixedDeltaTime, ref Bones);
                        Bones[i] = bone;
                    }
                }
                PassValue++;
            }

            public void Execute(int index, TransformAccess transform)
            {
                if (PassValue == 1)
                {
                    FirstProcess(index, ref transform, true);
                    if (index == Bones.Length - 2)
                    {
                        FirstProcess(index + 1, ref transform, false);
                        PassValue++;
                    }
                }
                else if (PassValue == 3)
                {
                    DeriveAllPositions(index, ref transform);
                    if (index == Bones.Length - 2)
                    {
                        DeriveAllPositions(index + 1, ref transform);
                        PassValue++;
                    }
                }
                else if (PassValue == 4)
                {
                    PoseAllBones(index, ref transform, true);
                    if (index == Bones.Length - 2)
                    {
                        PoseAllBones(index + 1, ref transform, false);
                        PassValue = 1;
                    }
                }
            }

            private void FirstProcess(int index, ref TransformAccess transform, bool transformExist)
            {
                var bone = Bones[index];

                if (index != 0)
                {
                    bone.PrepareBone(ref transform, ref Bones, Time, transformExist);
                }
                if (transformExist)
                {
                    bone.SetTransformInfo(ref transform);
                }
                if (index != 0)
                {
                    bone.CacheAnimationPosition(ref transform, ref Bones, Time, transformExist);
                }

                Bones[index] = bone;
            }

            private void DeriveAllPositions(int index, ref TransformAccess transform)
            {
                

                if (index != 0)
                {
                    var bone = Bones[index];

                    if (index == 1)
                    {
                        Vector3 virtualPosition = bone.DeriveFinalSolvePosition(Vector3.zero, _smoothing, Time, FixedDeltaTime);
                        _offset = transform.position - virtualPosition;
                    }
                    bone.DeriveFinalSolvePosition(_offset, _smoothing, Time, FixedDeltaTime);

                    Bones[index] = bone;
                }

                
            }

            private void PoseAllBones(int index, ref TransformAccess transform, bool transformExist)
            {
                var bone = Bones[index];

                if (index != 0)
                { 
                    bone.PoseBone(ref transform, JiggleSettings.Blend, ref Bones, transformExist);
                }
                if (transformExist)
                {
                    bone.SetTransformInfo(ref transform);
                }

                Bones[index] = bone;
            }
        }

        private TransformAccessArray[] _accessArrays;
        private NativeArray<JiggleBone>[] _jiggleBonesArrays;
        private NativeArray<int>[] _pass;
        private NativeArray<double>[] _accumulations;
        private NativeArray<Vector3>[] _offsets;
        //private NativeArray<double>[] _time;

        public JiggleRigJob[] _jobs;

        private void CreateArrays()
        {
            var rigCount = _jiggleRigs.Count;

            _jobs = new JiggleRigJob[rigCount];
            _jobHandles = new JobHandle[rigCount];
            _accessArrays = new TransformAccessArray[rigCount];
            _jiggleBonesArrays = new NativeArray<JiggleBone>[rigCount];
            _pass = new NativeArray<int>[rigCount];
            _accumulations = new NativeArray<double>[rigCount];
            _offsets = new NativeArray<Vector3>[rigCount];
            //_time = new NativeArray<double>[rigCount];

            for (var i = 0; i < rigCount; i++)
            {
                _accessArrays[i] = new TransformAccessArray(_jiggleRigs[i].bonesTransforms.ToArray(), 1);
                _jiggleBonesArrays[i] = new NativeArray<JiggleBone>(_jiggleRigs[i].bones.ToArray(), Allocator.Persistent);
                _pass[i] = new NativeArray<int>(1, Allocator.Persistent);
                _pass[i][0] = 1;
                _accumulations[i] = new NativeArray<double>(1, Allocator.Persistent);
                _accumulations[i][0] = 0;
                _offsets[i] = new NativeArray<Vector3>(1, Allocator.Persistent);
                _offsets[i][0] = Vector3.zero;
                //_time[i] = new NativeArray<double>(1, Allocator.Persistent);
                //_time[i][0] = Time.timeAsDouble;

                _jobs[i] = new JiggleRigJob
                {
                    Bones = _jiggleBonesArrays[i],
                    Wind = wind,
                    Gravity = Physics.gravity,
                    JiggleSettings = _jiggleRigs[i].jiggleSettings.GetSettingsStruct(),
                    pass = _pass[i],
                    accumulationArray = _accumulations[i],
                    offsetArray = _offsets[i]
                    //timeArray = _time[i]
                };
            }

        }

        private JobHandle[] _jobHandles;

        private void Update()
        {
            if (AllowUpdate)
            {
                //HandleJob();
                WaitForJobs();
            }
           /* SetTimeVariables();
            ScheduleTransformJobs();
            WaitForJobs();
            ScheduleJobs();
            WaitForJobs();
            ScheduleTransformJobs();
            WaitForJobs();
            ScheduleTransformJobs();
            WaitForJobs();*/
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

        private void LateUpdate()
        {
            if (AllowUpdate)
            {
                HandleJob();
                //WaitForJobs();
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
                //_time[i].Dispose();
            }
        }
    }
}

