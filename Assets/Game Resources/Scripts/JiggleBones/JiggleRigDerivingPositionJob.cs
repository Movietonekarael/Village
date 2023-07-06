using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Burst;


namespace JiggleBones
{
    [BurstCompile]
    public unsafe struct JiggleRigDerivingPositionJob : IJobParallelForTransform
    {
        [NativeDisableContainerSafetyRestriction] private NativeArray<JiggleBone> _bones;
        [NativeDisableUnsafePtrRestriction][ReadOnly] private readonly double* _time;
        [NativeDisableUnsafePtrRestriction][ReadOnly] private readonly float* _fixedDeltaTime;
        [NativeDisableUnsafePtrRestriction] private readonly Vector3* _offset;
        [ReadOnly] private const float _SMOOTHING = 1f;


        public JiggleRigDerivingPositionJob(NativeArray<JiggleBone> bones,
                                            double* time,
                                            float* fixedDeltaTime,
                                            Vector3* offset)
        {
            _bones = bones;
            _time = time;
            _fixedDeltaTime = fixedDeltaTime;
            _offset = offset;
        }

        public void Execute(int index, TransformAccess transform)
        {
            DeriveAllBonesPositions(index, ref transform);
        }

        private void DeriveAllBonesPositions(int index, ref TransformAccess transform)
        {
            DeriveBonePosition(index, ref transform);
            if (index == _bones.Length - 2)
            {
                DeriveBonePosition(index + 1, ref transform);
            }
        }

        private void DeriveBonePosition(int index, ref TransformAccess transform)
        {
            if (index != 0)
            {
                var bone = _bones[index];

                SetupOffset(ref bone, ref index, ref transform);
                bone.DeriveFinalSolvePosition(*_offset, _SMOOTHING, *_time, *_fixedDeltaTime);

                _bones[index] = bone;
            }
        }

        private readonly void SetupOffset(ref JiggleBone bone, ref int index, ref TransformAccess transform)
        {
            if (index == 1)
            {
                Vector3 virtualPosition = bone.DeriveFinalSolvePosition(Vector3.zero, _SMOOTHING, *_time, *_fixedDeltaTime);
                *_offset = transform.position - virtualPosition;
            }
        }
    }
}

