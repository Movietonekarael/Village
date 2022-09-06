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
    public class PlantGrowManeger : MonoBehaviour
    {
        /// <summary>
        /// List of PlantGrow class to work with
        /// </summary>
        private List<PlantGrow> plants;
        /// <summary>
        /// IJobParallelForTransform structS
        /// </summary>
        private PlantGrowUpdateJob job;
        /// <summary>
        /// Unused native array with data structures for jobs
        /// </summary>
        private NativeArray<PlantGrow.Data> platGrowDataArray;
        /// <summary>
        /// Special class for containing array of Transform classes and working with jobs
        /// </summary>
        private TransformAccessArray transformAccessArray;

        /// <summary>
        /// Setups all data needed for job to work
        /// </summary>
        private void Awake()
        {
            /// Finding and adding GameObjects with PlantGrow class
            PlantGrow[] array;
            array = FindObjectsOfType<PlantGrow>();
            plants = new List<PlantGrow>();

            foreach (PlantGrow item in array)
                plants.Add(item);

            /// Setuping NativeArray and TransformAccess array
            transformAccessArray = new TransformAccessArray(plants.Count);

            var plantsData = new PlantGrow.Data[plants.Count];

            for (int i = 0; i < plantsData.Length; i++)
            {
                plantsData[i] = new PlantGrow.Data(plants[i].scaleKoef);
                transformAccessArray.Add(plants[i].transform);
            }

            platGrowDataArray = new NativeArray<PlantGrow.Data>(plantsData, Allocator.Persistent);

            ///Providing data for job
            job = new PlantGrowUpdateJob
            {
                PlatGrowDataArray = platGrowDataArray
            };

        }
        /// <summary>
        /// Time step between scales in seconds
        /// </summary>
        [SerializeField] private float timeStep = 3.0f;
        /// <summary>
        /// Variable for containig last scale time
        /// </summary>
        private float lastTime = .0f;
        /// <summary>
        /// Starts and waits for job to complete every time step has passed
        /// </summary>
        private void Update()
        {
            if(Time.fixedTime - lastTime >= timeStep)
            {
                var jobHandlle = job.Schedule(transformAccessArray);
                jobHandlle.Complete();

                lastTime += timeStep;
            }

        }

        /// <summary>
        /// Disposes arrays memory for preventing memory leaks
        /// </summary>
        private void OnDestroy()
        {
            platGrowDataArray.Dispose();
            transformAccessArray.Dispose();
        }

    }
}

