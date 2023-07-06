using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Jobs;
using Unity.Burst;


namespace JiggleBones
{
    [BurstCompile]
    public unsafe struct JiggleRigPosingJob : IJobParallelForTransform
    {
        [NativeDisableContainerSafetyRestriction] private NativeArray<JiggleBone> _bones;
        [NativeDisableUnsafePtrRestriction][ReadOnly] private JiggleSettingsBase* _jiggleSettings;


        public JiggleRigPosingJob(NativeArray<JiggleBone> bones,
                                  JiggleSettingsBase* jiggleSettings)
        {
            _bones = bones;
            _jiggleSettings = jiggleSettings;
        }

        public void Execute(int index, TransformAccess transform)
        {
            PoseAllBones(index, ref transform);
        }

        private void PoseAllBones(int index, ref TransformAccess transform)
        {
            PoseBone(index, ref transform, true);
            if (index == _bones.Length - 2)
            {
                PoseBone(index + 1, ref transform, false);
            }
        }

        private void PoseBone(int index, ref TransformAccess transform, bool transformExist)
        {
            var bone = _bones[index];

            PoseBone(ref bone, ref index, ref transform, ref transformExist);
            SetTransformInfo(ref bone, ref transform, ref transformExist);

            _bones[index] = bone;
        }

        private void PoseBone(ref JiggleBone bone, ref int index, ref TransformAccess transform, ref bool transformExist)
        {
            if (index != 0)
            {
                bone.PoseBone(ref transform, _jiggleSettings->Blend, ref _bones, transformExist);
            }
        }

        private readonly void SetTransformInfo(ref JiggleBone bone, ref TransformAccess transform, ref bool transformExist)
        {
            if (transformExist)
            {
                bone.SetTransformInfo(ref transform);
            }
        }
    }
}

