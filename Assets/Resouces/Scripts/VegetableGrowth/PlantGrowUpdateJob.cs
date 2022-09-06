using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;
using UnityEngine.Jobs;

/// <summary>
/// Example namespace for growing tomatoes test
/// </summary>
namespace GrowVegetablesExample
{
    /// <summary>
    /// Scales all GameObjects in TransformAccess
    /// </summary>
    [BurstCompile]
    public struct PlantGrowUpdateJob : IJobParallelForTransform
    {
        public NativeArray<PlantGrow.Data> PlatGrowDataArray;

        public void Execute(int index, TransformAccess transform)
        {
            transform.localScale *= 1.05f;
        }
    }
}

