using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.TestTools;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Runtime.InteropServices;
using JigglePhysics;
using JiggleBones;
using System;

public class JiggleBonesTesting
{
    [UnityTest]
    public IEnumerator TestOfTransformTranslate()
    {
        var obj = new GameObject();
        var tr = obj.transform;
        var trVector = new Vector3(2, 5, 3);
        var expectedTransform = tr.position + trVector;

        tr.Translate(trVector);

        yield return null;

        Assert.AreEqual(expectedTransform, tr.position);
    }

    [UnityTest]
    public IEnumerator TestBones()
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
        Debug.Log($"Number of bones: {rigBuilder1.jiggleRigs[0].simulatedPoints.Count}");
        rigBuilder1.AllowUpdate = false;

        var settings2 = CreateStructSettings();
        rigBuilder2._jiggleRigs = new()
        {
            new JiggleRig(LeftBreast2, settings2),
            new JiggleRig(RightBreast2, settings2)
        };
        rigBuilder2.AllowUpdate = false;
        rigBuilder2.Setup();

        CompareRigs(rigBuilder1, rigBuilder2);

        var x = 0f;
        for (var i = 0; i < 100; i++)
        {
            var y = Mathf.Sin(x * .1f);
            MediumGirl1.Translate(new Vector3(x, y, 0));
            MediumGirl2.Translate(new Vector3(x, y, 0));

            Debug.Log("Preparing bones...");
            rigBuilder1.PrepareAllBones();
            rigBuilder2.SetTimeVariables();
            rigBuilder2.ScheduleTransformJobs(1);
            rigBuilder2.WaitForJobs();
            CompareRigs(rigBuilder1, rigBuilder2);
            //CheckTransformPairsRotations(LeftBreast1, RightBreast1, LeftBreast2, RightBreast2);

            Debug.Log("Simulating bones...");
            rigBuilder1.SimulateAllBones();
            rigBuilder2.ScheduleJobs();
            rigBuilder2.WaitForJobs();
            CompareRigs(rigBuilder1, rigBuilder2);

            Debug.Log("Deriving bones...");
            rigBuilder1.DeriveAllBones();
            rigBuilder2.ScheduleTransformJobs(3);
            rigBuilder2.WaitForJobs();
            CompareRigs(rigBuilder1, rigBuilder2);

            Debug.Log("Posing bones...");
            rigBuilder1.PoseAllBones();
            rigBuilder2.ScheduleTransformJobs(4);
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
            Debug.Log($"Comparing rigs ¹{i}");
            CompareRigs(builder1.jiggleRigs[i], builder2._jobs[i]);
        }
    }

    private void CompareRigs(JigglePhysics.JiggleRigBuilder.JiggleRig rig1,
                             JiggleBones.JiggleBuilder.JiggleRigJob rig2)
    {
        List<BoneTrace> rig1Traces = new();
        List<BoneTrace> rig2Traces = new();

        for (var i = 0; i < rig1.simulatedPoints.Count; i++)
        {
            rig1Traces.Add(new BoneTrace(rig1.simulatedPoints[i]));
            rig2Traces.Add(new BoneTrace(rig2.Bones[i + 1]));
        }

        var equality = true;

        for (var i = 0; i < rig1Traces.Count; i++)
        {
            equality = equality && (rig1Traces[i] == rig2Traces[i]);
        }

        if (equality)
        {
            Debug.Log("<color=green>Rig bones are equal.</color>");
        }
        else
        {
            Debug.Log("<color=red>Rig bones aren't equal.</color>");
        }
    }

    private JigglePhysics.JiggleSettings CreateSettings()
    {
        var settings = ScriptableObject.CreateInstance<JigglePhysics.JiggleSettings>();
        settings.SetParameter(JigglePhysics.JiggleSettingsBase.JiggleSettingParameter.Gravity, 1);
        settings.SetParameter(JigglePhysics.JiggleSettingsBase.JiggleSettingParameter.Friction, 0.05f);
        settings.SetParameter(JigglePhysics.JiggleSettingsBase.JiggleSettingParameter.AirFriction, 0.01f);
        settings.SetParameter(JigglePhysics.JiggleSettingsBase.JiggleSettingParameter.Blend, 1);
        settings.SetParameter(JigglePhysics.JiggleSettingsBase.JiggleSettingParameter.AngleElasticity, 0.511f);
        settings.SetParameter(JigglePhysics.JiggleSettingsBase.JiggleSettingParameter.ElasticitySoften, 0.2f);
        settings.SetParameter(JigglePhysics.JiggleSettingsBase.JiggleSettingParameter.LengthElasticity, 0.4f);
        return settings;
    }

    private JiggleBones.JiggleSettings CreateStructSettings()
    {
        var settings = ScriptableObject.CreateInstance<JiggleBones.JiggleSettings>();
        settings.gravityMultiplier = 1;
        settings.friction = 0.05f;
        settings.airFriction = 0.01f;
        settings.blend = 1;
        settings.angleElasticity = 0.511f;
        settings.elasticitySoften = 0.2f;
        settings.lengthElasticity = 0.4f;
        return settings;
    }

    private void CheckTransformPairsRotations(Transform transform11, 
                                              Transform transform12,
                                              Transform transform21,
                                              Transform transform22)
    {
        //Debug.Log($"Shit: {transform11.rotation.eulerAngles} | {transform12.rotation.eulerAngles}");
        //Debug.Log($"Shit: {transform21.rotation.eulerAngles} | {transform22.rotation.eulerAngles}");
        if (transform11.rotation.eulerAngles == transform21.rotation.eulerAngles &&
            transform12.rotation.eulerAngles == transform22.rotation.eulerAngles)
        {
            Debug.Log("Transform check: <color=green>Success</color>");
        }   
        else
        {
            Debug.Log("Transform check: <color=red>Fail</color>");
        }
    }

    private struct BoneTrace : IEquatable<BoneTrace>
    {
        public struct PositionFrame : IEquatable<PositionFrame>
        {
            public Vector3 position;
            public double time;
            public PositionFrame(Vector3 position, double time)
            {
                this.position = position;
                this.time = time;
            }

            public PositionFrame (JiggleBones.PositionFrame frame)
            {
                position = frame.position;
                time = frame.time;
            }

            public PositionFrame(JigglePhysics.JiggleBone.PositionFrame frame)
            {
                position = frame.position;
                time = frame.time;
            }

            public readonly bool Equals(PositionFrame other)
            {
                var equality = true;
                equality = equality && other.position.Equals(position);
                equality = equality && other.time.Equals(time);
                return equality;
            }

            public override string ToString()
            {
                return $"(Position: {position}), (Time: {time})";
            }
        }

        public Vector3 Position;
        public Vector3 PreviousPosition;
        public Vector3 ExtrapolatedPosition;

        public double UpdateTime;
        public double PreviousUpdateTime;

        public PositionFrame CurrentTargetAnimatedBoneFrame;
        public PositionFrame LastTargetAnimatedBoneFrame;
        public Vector3 CurrentFixedAnimatedBonePosition;

        public Quaternion boneRotationChangeCheck;
        public Vector3 bonePositionChangeCheck;
        public Quaternion lastValidPoseBoneRotation;
        public Vector3 lastValidPoseBoneLocalPosition;

        public Vector3? transformPosition;
        public Matrix4x4? fromLocalToWorld;
        public Matrix4x4? fromWorldToLocal;

        public BoneTrace(JiggleBones.JiggleBone bone)
        {
            Position = bone.Position;
            PreviousPosition = bone.PreviousPosition;
            ExtrapolatedPosition = bone.ExtrapolatedPosition;
            UpdateTime = bone.UpdateTime;
            PreviousUpdateTime = bone.PreviousUpdateTime;
            CurrentTargetAnimatedBoneFrame = new(bone.CurrentTargetAnimatedBoneFrame);
            LastTargetAnimatedBoneFrame = new(bone.LastTargetAnimatedBoneFrame);
            CurrentFixedAnimatedBonePosition = bone.CurrentFixedAnimatedBonePosition;
            boneRotationChangeCheck = bone.boneRotationChangeCheck;
            bonePositionChangeCheck = bone.bonePositionChangeCheck;
            lastValidPoseBoneRotation = bone.lastValidPoseBoneRotation;
            lastValidPoseBoneLocalPosition = bone.lastValidPoseBoneLocalPosition;
            transformPosition = bone.transformPosition;
            fromLocalToWorld = bone.fromLocalToWorld;
            fromWorldToLocal = bone.fromWorldToLocal;
        }

        public BoneTrace(JigglePhysics.JiggleBone bone)
        {
            Position = bone.position;
            PreviousPosition = bone.previousPosition;
            ExtrapolatedPosition = bone.extrapolatedPosition;
            UpdateTime = bone.updateTime;
            PreviousUpdateTime = bone.previousUpdateTime;
            CurrentTargetAnimatedBoneFrame = new(bone.currentTargetAnimatedBoneFrame);
            LastTargetAnimatedBoneFrame = new(bone.lastTargetAnimatedBoneFrame);
            CurrentFixedAnimatedBonePosition = bone.currentFixedAnimatedBonePosition;
            boneRotationChangeCheck = bone.boneRotationChangeCheck;
            bonePositionChangeCheck = bone.bonePositionChangeCheck;
            lastValidPoseBoneRotation = bone.lastValidPoseBoneRotation;
            lastValidPoseBoneLocalPosition = bone.lastValidPoseBoneLocalPosition;
            if (bone.transform != null)
            {
                transformPosition = bone.transform.position;
                fromLocalToWorld = bone.transform.localToWorldMatrix;
                fromWorldToLocal = bone.transform.worldToLocalMatrix;
            }
            else
            {
                transformPosition = null;
                fromLocalToWorld = null;
                fromWorldToLocal = null;
            }
        }

        public override bool Equals(object obj)
        {
            return obj is BoneTrace trace && Equals(trace);
        }

        public bool Equals(BoneTrace other)
        {
            return Position.Equals(other.Position) &&
                   PreviousPosition.Equals(other.PreviousPosition) &&
                   ExtrapolatedPosition.Equals(other.ExtrapolatedPosition) &&
                   UpdateTime == other.UpdateTime &&
                   PreviousUpdateTime == other.PreviousUpdateTime &&
                   CurrentTargetAnimatedBoneFrame.Equals(other.CurrentTargetAnimatedBoneFrame) &&
                   LastTargetAnimatedBoneFrame.Equals(other.LastTargetAnimatedBoneFrame) &&
                   CurrentFixedAnimatedBonePosition.Equals(other.CurrentFixedAnimatedBonePosition) &&
                   boneRotationChangeCheck.Equals(other.boneRotationChangeCheck) &&
                   bonePositionChangeCheck.Equals(other.bonePositionChangeCheck) &&
                   lastValidPoseBoneRotation.Equals(other.lastValidPoseBoneRotation) &&
                   lastValidPoseBoneLocalPosition.Equals(other.lastValidPoseBoneLocalPosition) &&
                   EqualityComparer<Vector3?>.Default.Equals(transformPosition, other.transformPosition) &&
                   EqualityComparer<Matrix4x4?>.Default.Equals(fromLocalToWorld, other.fromLocalToWorld) &&
                   EqualityComparer<Matrix4x4?>.Default.Equals(fromWorldToLocal, other.fromWorldToLocal);
        }

        public override int GetHashCode()
        {
            HashCode hash = new();
            hash.Add(Position);
            hash.Add(PreviousPosition);
            hash.Add(ExtrapolatedPosition);
            hash.Add(UpdateTime);
            hash.Add(PreviousUpdateTime);
            hash.Add(CurrentTargetAnimatedBoneFrame);
            hash.Add(LastTargetAnimatedBoneFrame);
            hash.Add(CurrentFixedAnimatedBonePosition);
            hash.Add(boneRotationChangeCheck);
            hash.Add(bonePositionChangeCheck);
            hash.Add(lastValidPoseBoneRotation);
            hash.Add(lastValidPoseBoneLocalPosition);
            hash.Add(transformPosition);
            hash.Add(fromLocalToWorld);
            hash.Add(fromWorldToLocal);
            return hash.ToHashCode();
        }

        public static bool operator ==(BoneTrace a, BoneTrace b) 
        {
            var equality = true;
            equality = equality && BonesValueEquals(a.Position, b.Position, "Position");
            equality = equality && BonesValueEquals(a.PreviousPosition, b.PreviousPosition, "PreviousPosition");
            equality = equality && BonesValueEquals(a.ExtrapolatedPosition, b.ExtrapolatedPosition, "ExtrapolatedPosition");
            equality = equality && BonesValueEquals(a.UpdateTime, b.UpdateTime, "UpdateTime");
            equality = equality && BonesValueEquals(a.PreviousUpdateTime, b.PreviousUpdateTime, "PreviousUpdateTime");
            equality = equality && BonesValueEquals(a.CurrentTargetAnimatedBoneFrame, b.CurrentTargetAnimatedBoneFrame, "CurrentTargetAnimatedBoneFrame");
            equality = equality && BonesValueEquals(a.LastTargetAnimatedBoneFrame, b.LastTargetAnimatedBoneFrame, "LastTargetAnimatedBoneFrame");
            equality = equality && BonesValueEquals(a.CurrentFixedAnimatedBonePosition, b.CurrentFixedAnimatedBonePosition, "CurrentFixedAnimatedBonePosition");
            equality = equality && BonesValueEquals(a.boneRotationChangeCheck, b.boneRotationChangeCheck, "boneRotationChangeCheck");
            equality = equality && BonesValueEquals(a.bonePositionChangeCheck, b.bonePositionChangeCheck, "bonePositionChangeCheck");
            equality = equality && BonesValueEquals(a.lastValidPoseBoneRotation, b.lastValidPoseBoneRotation, "lastValidPoseBoneRotation");
            equality = equality && BonesValueEquals(a.lastValidPoseBoneLocalPosition, b.lastValidPoseBoneLocalPosition, "lastValidPoseBoneLocalPosition");
            equality = equality && BonesValueEquals(a.transformPosition, b.transformPosition, "transformPosition");
            equality = equality && BonesValueEquals(a.fromLocalToWorld, b.fromLocalToWorld, "fromLocalToWorld");
            equality = equality && BonesValueEquals(a.fromWorldToLocal, b.fromWorldToLocal, "fromWorldToLocal");
            return equality;
        }

        public static bool operator !=(BoneTrace a, BoneTrace b)
        {
            return !(a == b);
        }

        private static bool BonesValueEquals<T> (T a, T b, string name) where T : IEquatable<T>
        {
            if (!a.Equals(b))
            {
                DebugParametersArentEqual(a, b, name);
                return false;
            }
            else
            {
                return true;
            }
        }

        private static bool BonesValueEquals<T>(T? a, T? b, string name) where T : struct, IEquatable<T>
        {
            var equality = true;
            if (a == null && b == null)
            {
                equality = true;
            }
            else if ((a == null && b != null) || (a != null && b == null))
            {
                DebugParametersArentEqual(a, b, name);
                equality = false;
            }
            else
            {
                equality = equality && BonesValueEquals(a.Value, b.Value, name);
            }
            return equality;
        }

        private static void DebugParametersArentEqual<T>(T a, T b, string name)
        {
            Debug.Log($"Bone's {name} aren't equal: {a} | {b}");
        }
    }
}
