using log4net.Util;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.UIElements;
using static PlasticGui.LaunchDiffParameters;

namespace JiggleBones
{
    public struct PositionFrame
    {
        public Vector3 position;
        public double time;
        public PositionFrame(Vector3 position, double time)
        {
            this.position = position;
            this.time = time;
        }
    }

    public struct JiggleBone
    {
        public int Index;
        public int? ParentIndex;
        public int? ChildIndex;

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

        public void IncreaseIndex()
        {
            Index++;
            if (ParentIndex != null)
                ParentIndex++;
            if (ChildIndex != null)
                ChildIndex++;
        }

        public void SetTransformInfo(ref TransformAccess transform)
        {
            transformPosition = transform.position;
            fromLocalToWorld = transform.localToWorldMatrix;
            fromWorldToLocal = transform.worldToLocalMatrix;
        }

        public void SetChildIndex(int value)
        {
            ChildIndex = value;
        }

        public void SetParentIndex(int value)
        {
            ParentIndex = value;
        }

        public JiggleBone(UnityEngine.Transform transform, int index, int? parentIndex, Vector3 position, List<JiggleBone> bones)
        {
            this.Index = index;
            this.ParentIndex = parentIndex;
            this.Position = position;
            PreviousPosition = position;
            if (transform != null)
            {
                lastValidPoseBoneRotation = transform.localRotation;
                lastValidPoseBoneLocalPosition = transform.localPosition;
            }
            else
            {
                lastValidPoseBoneRotation = default;
                lastValidPoseBoneLocalPosition = default;
            }

            UpdateTime = Time.timeAsDouble;
            PreviousUpdateTime = Time.timeAsDouble;
            LastTargetAnimatedBoneFrame = new PositionFrame(position, Time.timeAsDouble);
            CurrentTargetAnimatedBoneFrame = LastTargetAnimatedBoneFrame;

            
            if (ParentIndex != null)
            {
                var bone = bones[ParentIndex.Value];
                bone.SetChildIndex(Index);
                bones[parentIndex.Value] = bone;
            }

            ChildIndex = null;

            boneRotationChangeCheck = default;
            bonePositionChangeCheck = default;
            CurrentFixedAnimatedBonePosition = default;
            transformPosition = null;
            fromLocalToWorld = null;
            fromWorldToLocal = null;
            ExtrapolatedPosition = default;
        }
        
        public void PrepareBone(ref TransformAccess transform, ref NativeArray<JiggleBone> bones, double time, bool transformExist)
        {
            // If bone is not animated, return to last unadulterated pose
            if (transformExist)
            {
                if (boneRotationChangeCheck == transform.localRotation)
                {
                    transform.localRotation = lastValidPoseBoneRotation;
                }
                if (bonePositionChangeCheck == transform.localPosition)
                {
                    transform.localPosition = lastValidPoseBoneLocalPosition;
                }
            }
            //CacheAnimationPosition(ref transform, ref bones, time, transformExist);
        }
        
        public void CacheAnimationPosition(ref TransformAccess transform, ref NativeArray<JiggleBone> bones, double time, bool transformExist)
        {
            // Purely virtual particles need to reconstruct their desired position.
            LastTargetAnimatedBoneFrame = CurrentTargetAnimatedBoneFrame;
            if (!transformExist)
            {
                Vector3 parentTransformPosition = bones[ParentIndex.Value].transformPosition.Value;
                if (bones[ParentIndex.Value].ParentIndex != null && 
                    bones[ParentIndex.Value].ParentIndex.Value != 0)
                {
                    //Vector3 projectedForward = (parentTransformPosition - parent.parent.transform.position).normalized;
                    Vector3 pos = bones[bones[ParentIndex.Value].ParentIndex.Value].fromWorldToLocal.Value * parentTransformPosition;
                    pos = bones[ParentIndex.Value].fromLocalToWorld.Value * pos;
                    CurrentTargetAnimatedBoneFrame = new PositionFrame(pos, time);
                }
                else
                {
                    // parent.transform.parent is guaranteed to exist here, unless the user is jiggling a single bone by itself (which throws an exception).
                    //Vector3 projectedForward = (parentTransformPosition - parent.transform.parent.position).normalized;
                    Vector3 pos = bones[bones[ParentIndex.Value].ParentIndex.Value].fromWorldToLocal.Value * parentTransformPosition;
                    pos = bones[ParentIndex.Value].fromLocalToWorld.Value * pos;
                    CurrentTargetAnimatedBoneFrame = new PositionFrame(pos, time);
                }
                return;
            }
            CurrentTargetAnimatedBoneFrame = new PositionFrame(transform.position, time);
            lastValidPoseBoneRotation = transform.localRotation;
            lastValidPoseBoneLocalPosition = transform.localPosition;
        }
        
        public void Simulate(ref JiggleSettingsBase jiggleSettings, 
                             ref Vector3 wind, 
                             Vector3 gravity,
                             double accumulationTime, 
                             float fixedDeltaTime,
                             ref NativeArray<JiggleBone> bones)
        {
            CurrentFixedAnimatedBonePosition = GetTargetBonePosition(LastTargetAnimatedBoneFrame, CurrentTargetAnimatedBoneFrame, accumulationTime);

            if (ParentIndex.Value == 0)
            {
                SetNewPosition(CurrentFixedAnimatedBonePosition, accumulationTime);
                return;
            }
            Vector3 localSpaceVelocity = (Position - PreviousPosition) - (bones[ParentIndex.Value].Position - bones[ParentIndex.Value].PreviousPosition);
            //Debug.DrawLine(position, position + localSpaceVelocity, Color.cyan);
            Vector3 newPosition = JiggleBone.NextPhysicsPosition(
                Position, 
                PreviousPosition, 
                localSpaceVelocity, 
                fixedDeltaTime,
                gravity,
                jiggleSettings.GravityMultiplier,
                jiggleSettings.Friction,
                jiggleSettings.AirFriction
            );
            newPosition += wind * (fixedDeltaTime * jiggleSettings.AirFriction);
            newPosition = ConstrainAngle(newPosition, 
                                         jiggleSettings.AngleElasticity * jiggleSettings.AngleElasticity, 
                                         jiggleSettings.ElasticitySoften,
                                         ref bones);
            newPosition = ConstrainLength(newPosition, jiggleSettings.LengthElasticity * jiggleSettings.LengthElasticity, ref bones);
            SetNewPosition(newPosition, accumulationTime);
        }
        
        private readonly Vector3 GetTargetBonePosition(PositionFrame prev, PositionFrame next, double time)
        {
            double diff = next.time - prev.time;
            if (diff == 0)
            {
                return next.position;
            }
            double t = (time - prev.time) / diff;
            return Vector3.Lerp(prev.position, next.position, (float)t);
        }

        public void SetNewPosition(Vector3 newPosition, double time)
        {
            PreviousUpdateTime = UpdateTime;
            PreviousPosition = Position;
            //if (parent!=null) previousLocalPosition = parent.transform.InverseTransformPoint(previousPosition);
            UpdateTime = time;
            Position = newPosition;
        }

        public Vector3 DeriveFinalSolvePosition(Vector3 offset, float smoothing, double time, float fixedDeltaTime)
        {
            double t = ((time - smoothing * fixedDeltaTime) - PreviousUpdateTime) / fixedDeltaTime;
            ExtrapolatedPosition = offset + Vector3.LerpUnclamped(PreviousPosition, Position, (float)t);
            return ExtrapolatedPosition;
        }

        public static Vector3 NextPhysicsPosition(Vector3 newPosition, 
                                                  Vector3 previousPosition, 
                                                  Vector3 localSpaceVelocity, 
                                                  float deltaTime, 
                                                  Vector3 gravity, 
                                                  float gravityMultiplier, 
                                                  float friction, 
                                                  float airFriction)
        {
            float squaredDeltaTime = deltaTime * deltaTime;
            Vector3 vel = newPosition - previousPosition - localSpaceVelocity;
            return newPosition + vel * (1f - airFriction) + localSpaceVelocity * (1f - friction) + gravity * (gravityMultiplier * squaredDeltaTime);
        }

        public Vector3 ConstrainAngle(Vector3 newPosition, float elasticity, float elasticitySoften, ref NativeArray<JiggleBone> bones)
        {
            Vector3 parentParentPosition;
            Vector3 poseParentParent;
            if (bones[ParentIndex.Value].ParentIndex.Value == 0)
            {
                poseParentParent = bones[ParentIndex.Value].CurrentFixedAnimatedBonePosition + 
                                  (bones[ParentIndex.Value].CurrentFixedAnimatedBonePosition - CurrentFixedAnimatedBonePosition);
                parentParentPosition = poseParentParent;
            }
            else
            {
                parentParentPosition = bones[bones[ParentIndex.Value].ParentIndex.Value].Position;
                poseParentParent = bones[bones[ParentIndex.Value].ParentIndex.Value].CurrentFixedAnimatedBonePosition;
            }
            Vector3 parentAimTargetPose = bones[ParentIndex.Value].CurrentFixedAnimatedBonePosition - poseParentParent;
            Vector3 parentAim = bones[ParentIndex.Value].Position - parentParentPosition;
            Quaternion TargetPoseToPose = Quaternion.FromToRotation(parentAimTargetPose, parentAim);
            Vector3 currentPose = CurrentFixedAnimatedBonePosition - poseParentParent;
            Vector3 constraintTarget = TargetPoseToPose * currentPose;
            float error = Vector3.Distance(newPosition, parentParentPosition + constraintTarget);
            error /= GetLengthToParent(ref bones);
            error = Mathf.Clamp01(error);
            error = Mathf.Pow(error, elasticitySoften * 2f);
            return Vector3.Lerp(newPosition, parentParentPosition + constraintTarget, elasticity * error);
        }

        public Vector3 ConstrainLength(Vector3 newPosition, float elasticity, ref NativeArray<JiggleBone> bones)
        {
            Vector3 diff = newPosition - bones[ParentIndex.Value].Position;
            Vector3 dir = diff.normalized;
            return Vector3.Lerp(newPosition, bones[ParentIndex.Value].Position + dir * GetLengthToParent(ref bones), elasticity);
        }

        private float GetLengthToParent(ref NativeArray<JiggleBone> bones)
        {
            if (ParentIndex.Value == 0)
            {
                return 0.1f;
            }
            return Vector3.Distance(CurrentFixedAnimatedBonePosition, bones[ParentIndex.Value].CurrentFixedAnimatedBonePosition);
        }

        public void PoseBone(ref TransformAccess transform, float blend, ref NativeArray<JiggleBone> bones, bool transformExist)
        {
            if (ChildIndex != null)
            {
                Vector3 positionBlend = Vector3.Lerp(CurrentTargetAnimatedBoneFrame.position, ExtrapolatedPosition, blend);
                Vector3 childPositionBlend = Vector3.Lerp(bones[ChildIndex.Value].CurrentTargetAnimatedBoneFrame.position,
                                                          bones[ChildIndex.Value].ExtrapolatedPosition, 
                                                          blend);

                //Debug.Log(positionBlend);

                if (ParentIndex != null && ParentIndex.Value != 0)
                {
                    transform.position = positionBlend;
                }
                Vector3 childPosition;
                if (bones[ChildIndex.Value].transformPosition == null)
                {
                    if (ParentIndex != null && ParentIndex.Value != 0)
                    { // If we have a proper jigglebone parent...
                        childPosition = bones[ParentIndex.Value].fromWorldToLocal.Value * transform.position;
                        childPosition = transform.localToWorldMatrix * childPosition;
                    }
                    else
                    { // Otherwise we guess with the parent transform
                        childPosition = bones[ParentIndex.Value].fromWorldToLocal.Value * transform.position;
                        childPosition = transform.localToWorldMatrix * childPosition;
                    }
                }
                else
                {
                    childPosition = bones[ChildIndex.Value].transformPosition.Value;
                }
                Vector3 cachedAnimatedVector = childPosition - transform.position;
                Vector3 simulatedVector = childPositionBlend - positionBlend;
                Quaternion animPoseToPhysicsPose = Quaternion.FromToRotation(cachedAnimatedVector, simulatedVector);
                //Debug.Log(animPoseToPhysicsPose);
                transform.rotation = animPoseToPhysicsPose * transform.rotation;
            }
            if (transformExist)
            {
                boneRotationChangeCheck = transform.localRotation;
                bonePositionChangeCheck = transform.localPosition;
            }
        }
    }
}