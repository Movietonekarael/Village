using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;
using Unity.Burst;


namespace JiggleBones
{
    [BurstCompile]
    public unsafe struct JiggleRigSimulationJob : IJob
    {
        [NativeDisableContainerSafetyRestriction] private NativeArray<JiggleBone> _bones;
        [NativeDisableUnsafePtrRestriction] private readonly double* _accumulation;
        [NativeDisableUnsafePtrRestriction][ReadOnly] private readonly double* _time;
        [NativeDisableUnsafePtrRestriction][ReadOnly] private readonly float* _fixedDeltaTime;
        [NativeDisableUnsafePtrRestriction][ReadOnly] private readonly JiggleSettingsBase* _jiggleSettings;
        [NativeDisableUnsafePtrRestriction][ReadOnly] private readonly Vector3* _wind;
        [NativeDisableUnsafePtrRestriction][ReadOnly] private readonly Vector3* _gravity;


        public JiggleRigSimulationJob(NativeArray<JiggleBone> bones,
                                      double* accumulation,
                                      double* time,
                                      float* fixedDeltaTime,
                                      Vector3* wind,
                                      Vector3* gravity,
                                      JiggleSettingsBase* jiggleSettings)
        {
            _bones = bones;
            _accumulation = accumulation;
            _time = time;
            _fixedDeltaTime = fixedDeltaTime;
            _wind = wind;
            _gravity = gravity;
            _jiggleSettings = jiggleSettings;
        }

        public void Execute()
        {
            SimulateBones();
        }

        private void SimulateBones()
        {
            while (*_accumulation > *_fixedDeltaTime)
            {
                *_accumulation -= *_fixedDeltaTime;
                var accumulationTime = *_time - *_accumulation;

                SimulateOneIteration(ref accumulationTime);
            }
        }

        private void SimulateOneIteration(ref double accumulationTime)
        {
            for (var i = 1; i < _bones.Length; i++)
            {
                var bone = _bones[i];
                bone.Simulate(ref *_jiggleSettings, ref *_wind, *_gravity, accumulationTime, *_fixedDeltaTime, ref _bones);
                _bones[i] = bone;
            }
        }
    }
}

