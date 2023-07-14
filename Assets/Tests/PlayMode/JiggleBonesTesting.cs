using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.TestTools;
using UnityEngine.ResourceManagement.AsyncOperations;
using JigglePhysics;
using JiggleBones;


namespace Testing
{
    public partial class JiggleBonesTesting
    {
        private const float _GRAVITY = 1;
        private const float _FRICTION = 0.05f;
        private const float _AIR_FRICTION = 0.01f;
        private const float _BLEND = 1;
        private const float _ANGLE_ELASTICITY = 0.5f;
        private const float _ELASTICITY_SOFTEN = 0.2f;
        private const float _LENGTH_ELASTICITY = 0.2f;

        [UnityTest]
        public IEnumerator JiggleBonesHaveRightValues()
        {
            var opHandle = Addressables.LoadAssetAsync<GameObject>("Medium Girl");

            yield return opHandle;

            if (opHandle.Status == AsyncOperationStatus.Succeeded)
            {
                var obj1 = UnityEngine.Object.Instantiate(opHandle.Result);
                var obj2 = UnityEngine.Object.Instantiate(opHandle.Result);
                obj1.name = "Medium Girl1";
                obj2.name = "Medium Girl2";
            }

            var MediumGirlObj1 = GameObject.Find("Medium Girl1");
            var MediumGirlObj2 = GameObject.Find("Medium Girl2");
            var MediumGirl1 = MediumGirlObj1.transform;
            var MediumGirl2 = MediumGirlObj2.transform;
            var LeftBreastObj1 = GameObject.Find("Medium Girl1/metarig/Bone/spine/spine.001/spine.002/spine.003/breast.L");
            var LeftBreastObj2 = GameObject.Find("Medium Girl2/metarig/Bone/spine/spine.001/spine.002/spine.003/breast.L");
            var LeftBreast1 = LeftBreastObj1.transform;
            var LeftBreast2 = LeftBreastObj2.transform;
            var RightBreastObj1 = GameObject.Find("Medium Girl1/metarig/Bone/spine/spine.001/spine.002/spine.003/breast.R");
            var RightBreastObj2 = GameObject.Find("Medium Girl2/metarig/Bone/spine/spine.001/spine.002/spine.003/breast.R");
            var RightBreast1 = RightBreastObj1.transform;
            var RightBreast2 = RightBreastObj2.transform;


            var rigBuilder1 = MediumGirlObj1.AddComponent<JiggleRigBuilder>();
            var rigBuilder2 = MediumGirlObj2.AddComponent<JiggleBuilder>();

            var settings1 = CreateSettings();
            rigBuilder1.AddJiggleRig(LeftBreast1, settings1);
            rigBuilder1.AddJiggleRig(RightBreast1, settings1);
            rigBuilder1.AllowUpdate = false;

            var settings2 = CreateStructSettings();
            rigBuilder2.AddRig(new JiggleBones.JiggleRig(LeftBreast2, settings2));
            rigBuilder2.AddRig(new JiggleBones.JiggleRig(RightBreast2, settings2));

            rigBuilder2.AllowUpdate = false;
            rigBuilder2.Setup();

            CompareRigs(rigBuilder1, rigBuilder2);

            var x = 0f;
            for (var i = 0; i < 100; i++)
            {
                var y = Mathf.Sin(x * .1f);
                MediumGirl1.Translate(new Vector3(x, y, 0));
                MediumGirl2.Translate(new Vector3(x, y, 0));

                //Debug.Log("Preparing bones...");
                rigBuilder1.PrepareAllBones();
                rigBuilder2.SetTimeVariables();
                rigBuilder2.SchedulePreparationJobs();
                rigBuilder2.WaitForJobs();
                CompareRigs(rigBuilder1, rigBuilder2);

                //Debug.Log("Simulating bones...");
                rigBuilder1.SimulateAllBones();
                rigBuilder2.SchedulesSimulateJobs();
                rigBuilder2.WaitForJobs();
                CompareRigs(rigBuilder1, rigBuilder2);

                //Debug.Log("Deriving bones...");
                rigBuilder1.DeriveAllBones();
                rigBuilder2.ScheduleDerivingJobs();
                rigBuilder2.WaitForJobs();
                CompareRigs(rigBuilder1, rigBuilder2);

                //Debug.Log("Posing bones...");
                rigBuilder1.PoseAllBones();
                rigBuilder2.SchedulePosingJobs();
                rigBuilder2.WaitForJobs();
                CompareRigs(rigBuilder1, rigBuilder2);

                yield return null;
                x++;
            }
        }

        private void CompareRigs(JigglePhysics.JiggleRigBuilder builder1, JiggleBones.JiggleBuilder builder2)
        {
            for (var i = 0; i < builder1.jiggleRigs.Count; i++)
            {
                CompareRigs(builder1.jiggleRigs[i], builder2.JiggleBonesArrays[i].ToArray());
            }
        }

        private void CompareRigs(JigglePhysics.JiggleRigBuilder.JiggleRig rig1,
                                 JiggleBones.JiggleBone[] bones2)
        {
            List<BoneTrace> rig1Traces = new();
            List<BoneTrace> rig2Traces = new();

            for (var i = 0; i < rig1.simulatedPoints.Count; i++)
            {
                rig1Traces.Add(new BoneTrace(rig1.simulatedPoints[i]));
                rig2Traces.Add(new BoneTrace(bones2[i + 1]));
            }

            var equality = true;

            for (var i = 0; i < rig1Traces.Count; i++)
            {
                equality = equality && (rig1Traces[i] == rig2Traces[i]);

                if (!equality)
                {
                    Debug.LogWarning("<color=red>Try run tests with burst disabled.</color>");
                }

                Assert.AreEqual(rig1Traces[i], rig2Traces[i]);
            }
        }

        private JigglePhysics.JiggleSettings CreateSettings()
        {
            var settings = ScriptableObject.CreateInstance<JigglePhysics.JiggleSettings>();
            settings.SetParameter(JigglePhysics.JiggleSettingsBase.JiggleSettingParameter.Gravity, _GRAVITY);
            settings.SetParameter(JigglePhysics.JiggleSettingsBase.JiggleSettingParameter.Friction, _FRICTION);
            settings.SetParameter(JigglePhysics.JiggleSettingsBase.JiggleSettingParameter.AirFriction, _AIR_FRICTION);
            settings.SetParameter(JigglePhysics.JiggleSettingsBase.JiggleSettingParameter.Blend, _BLEND);
            settings.SetParameter(JigglePhysics.JiggleSettingsBase.JiggleSettingParameter.AngleElasticity, _ANGLE_ELASTICITY);
            settings.SetParameter(JigglePhysics.JiggleSettingsBase.JiggleSettingParameter.ElasticitySoften, _ELASTICITY_SOFTEN);
            settings.SetParameter(JigglePhysics.JiggleSettingsBase.JiggleSettingParameter.LengthElasticity, _LENGTH_ELASTICITY);
            return settings;
        }

        private JiggleBones.JiggleSettings CreateStructSettings()
        {
            var settings = ScriptableObject.CreateInstance<JiggleBones.JiggleSettings>();
            settings.gravityMultiplier = _GRAVITY;
            settings.friction = _FRICTION;
            settings.airFriction = _AIR_FRICTION;
            settings.blend = _BLEND;
            settings.angleElasticity = _ANGLE_ELASTICITY;
            settings.elasticitySoften = _ELASTICITY_SOFTEN;
            settings.lengthElasticity = _LENGTH_ELASTICITY;
            return settings;
        }
    }
}