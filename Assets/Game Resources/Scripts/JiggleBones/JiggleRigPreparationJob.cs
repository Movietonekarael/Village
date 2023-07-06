using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Jobs;
using Unity.Burst;


namespace JiggleBones
{
    [BurstCompile]
    public unsafe struct JiggleRigPreparationJob : IJobParallelForTransform
    {
        [NativeDisableContainerSafetyRestriction] private NativeArray<JiggleBone> _bones;
        [NativeDisableUnsafePtrRestriction][ReadOnly] private readonly double* _time;


        public JiggleRigPreparationJob(NativeArray<JiggleBone> bones, double* time)
        {
            _bones = bones;
            _time = time;
        }

        public void Execute(int index, TransformAccess transform)
        {
            PrepareAllBones(ref index, ref transform);
        }

        private void PrepareAllBones(ref int index, ref TransformAccess transform)
        {
            PrepareBone(index, ref transform, true);
            if (index == _bones.Length - 2)
            {
                PrepareBone(index + 1, ref transform, false);
            }
        }

        private void PrepareBone(int index, ref TransformAccess transform, bool transformExist)
        {
            var bone = _bones[index];

            PrepareBone(ref bone, ref index, ref transform, ref transformExist);
            SetTransformInfo(ref bone, ref transform, ref transformExist);
            CacheAnimationPosition(ref bone, ref index, ref transform, ref transformExist);

            _bones[index] = bone;
        }

        private readonly void PrepareBone(ref JiggleBone bone, ref int index, ref TransformAccess transform, ref bool transformExist)
        {
            if (index != 0)
            {
                bone.PrepareBone(ref transform, transformExist);
            }
        }

        private readonly void SetTransformInfo(ref JiggleBone bone, ref TransformAccess transform, ref bool transformExist)
        {
            if (transformExist)
            {
                bone.SetTransformInfo(ref transform);
            }
        }

        private void CacheAnimationPosition(ref JiggleBone bone, ref int index, ref TransformAccess transform, ref bool transformExist)
        {
            if (index != 0)
            {
                bone.CacheAnimationPosition(ref transform, ref _bones, *_time, transformExist);
            }
        }
    }
}

