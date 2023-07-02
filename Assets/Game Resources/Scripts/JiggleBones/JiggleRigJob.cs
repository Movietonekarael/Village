using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Burst;

namespace JiggleBones
{
    [BurstCompile]
    public struct JiggleRigJob : IJobParallelForTransform, IJob
    {
        [NativeDisableContainerSafetyRestriction] private NativeArray<JiggleBone> _bones;

        [ReadOnly] public double Time;
        [ReadOnly] public float FixedDeltaTime;
        [ReadOnly] private JiggleSettingsBase _jiggleSettings;
        [ReadOnly] private Vector3 _wind;
        [ReadOnly] private const float _SMOOTHING = 1f;
        [ReadOnly] private Vector3 _gravity;

        [NativeDisableContainerSafetyRestriction] private NativeArray<int> _passArray;
        [NativeDisableContainerSafetyRestriction] private NativeArray<double> _accumulationArray;
        [NativeDisableContainerSafetyRestriction] private NativeArray<Vector3> _offsetArray;

        public JiggleRigJob(NativeArray<JiggleBone> bones,
                            Vector3 wind,
                            Vector3 gravity,
                            JiggleSettingsBase jiggleSettings,
                            NativeArray<int> pass,
                            NativeArray<double> accumulationArray,
                            NativeArray<Vector3> offsetArray)
        {
            _bones = bones;
            _wind = wind;
            _gravity = gravity;
            _jiggleSettings = jiggleSettings;
            _passArray = pass;
            _accumulationArray = accumulationArray;
            _offsetArray = offsetArray;
            Time = default;
            FixedDeltaTime = default;
        }

        public int PassValue { get { return _passArray[0]; } set { _passArray[0] = value; } }
        public double Accumulation { get { return _accumulationArray[0]; } set { _accumulationArray[0] = value; } }
        private Vector3 _offset { get { return _offsetArray[0]; } set { _offsetArray[0] = value; } }

        public void Execute()
        {
            while (Accumulation > FixedDeltaTime)
            {
                Accumulation -= FixedDeltaTime;
                var accumulationTime = Time - Accumulation;
                for (var i = 1; i < _bones.Length; i++)
                {
                    var bone = _bones[i];
                    bone.Simulate(ref _jiggleSettings, ref _wind, _gravity, accumulationTime, FixedDeltaTime, ref _bones);
                    _bones[i] = bone;
                }
            }
            PassValue++;
        }

        public void Execute(int index, TransformAccess transform)
        {
            if (PassValue == 1)
            {
                PrepareBones(index, ref transform, true);
                if (index == _bones.Length - 2)
                {
                    PrepareBones(index + 1, ref transform, false);
                    PassValue++;
                }
            }
            else if (PassValue == 3)
            {
                DeriveAllPositions(index, ref transform);
                if (index == _bones.Length - 2)
                {
                    DeriveAllPositions(index + 1, ref transform);
                    PassValue++;
                }
            }
            else if (PassValue == 4)
            {
                PoseAllBones(index, ref transform, true);
                if (index == _bones.Length - 2)
                {
                    PoseAllBones(index + 1, ref transform, false);
                    PassValue = 1;
                }
            }
        }

        private void PrepareBones(int index, ref TransformAccess transform, bool transformExist)
        {
            var bone = _bones[index];

            if (index != 0)
            {
                bone.PrepareBone(ref transform, transformExist);
            }
            if (transformExist)
            {
                bone.SetTransformInfo(ref transform);
            }
            if (index != 0)
            {
                bone.CacheAnimationPosition(ref transform, ref _bones, Time, transformExist);
            }
            _bones[index] = bone;
        }

        private void DeriveAllPositions(int index, ref TransformAccess transform)
        {
            if (index != 0)
            {
                var bone = _bones[index];

                if (index == 1)
                {
                    Vector3 virtualPosition = bone.DeriveFinalSolvePosition(Vector3.zero, _SMOOTHING, Time, FixedDeltaTime);
                    _offset = transform.position - virtualPosition;
                }
                bone.DeriveFinalSolvePosition(_offset, _SMOOTHING, Time, FixedDeltaTime);

                _bones[index] = bone;
            }
        }

        private void PoseAllBones(int index, ref TransformAccess transform, bool transformExist)
        {
            var bone = _bones[index];

            if (index != 0)
            {
                bone.PoseBone(ref transform, _jiggleSettings.Blend, ref _bones, transformExist);
            }
            if (transformExist)
            {
                bone.SetTransformInfo(ref transform);
            }

            _bones[index] = bone;
        }
    }
}

