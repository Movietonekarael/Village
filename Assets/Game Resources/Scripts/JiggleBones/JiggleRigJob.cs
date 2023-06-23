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
        [NativeDisableContainerSafetyRestriction] public NativeArray<JiggleBone> Bones;
        [ReadOnly] public double Time;
        [ReadOnly] public float FixedDeltaTime;
        [ReadOnly] public JiggleSettingsBase JiggleSettings;
        [ReadOnly] public Vector3 Wind;
        [ReadOnly] private const float _SMOOTHING = 1f;
        [ReadOnly] public Vector3 Gravity;

        [NativeDisableContainerSafetyRestriction] public NativeArray<int> Pass;
        [NativeDisableContainerSafetyRestriction] public NativeArray<double> AccumulationArray;
        [NativeDisableContainerSafetyRestriction] public NativeArray<Vector3> OffsetArray;

        public int PassValue { get { return Pass[0]; } set { Pass[0] = value; } }
        public double Accumulation { get { return AccumulationArray[0]; } set { AccumulationArray[0] = value; } }
        private Vector3 _offset { get { return OffsetArray[0]; } set { OffsetArray[0] = value; } }

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
                bone.PrepareBone(ref transform, transformExist);
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
                    Vector3 virtualPosition = bone.DeriveFinalSolvePosition(Vector3.zero, _SMOOTHING, Time, FixedDeltaTime);
                    _offset = transform.position - virtualPosition;
                }
                bone.DeriveFinalSolvePosition(_offset, _SMOOTHING, Time, FixedDeltaTime);

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
}

