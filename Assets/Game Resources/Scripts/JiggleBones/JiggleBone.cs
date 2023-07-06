using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;


namespace JiggleBones
{
    public struct JiggleBone
    {
        public int Index;
        public int? ParentIndex;
        public int? ChildIndex;

        private Vector3 _position;
        private Vector3 _previousPosition;
        private Vector3 _extrapolatedPosition;

        private double _updateTime;
        private double _previousUpdateTime;

        private PositionFrame _currentTargetAnimatedBoneFrame;
        private PositionFrame _lastTargetAnimatedBoneFrame;
        private Vector3 _currentFixedAnimatedBonePosition;

        private Quaternion _boneRotationChangeCheck;
        private Vector3 _bonePositionChangeCheck;
        private Quaternion _lastValidPoseBoneRotation;
        private Vector3 _lastValidPoseBoneLocalPosition;

        private Vector3? _transformPosition;
        private Matrix4x4? _fromLocalToWorld;
        private Matrix4x4? _fromWorldToLocal;

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
            _transformPosition = transform.position;
            _fromLocalToWorld = transform.localToWorldMatrix;
            _fromWorldToLocal = transform.worldToLocalMatrix;
        }

        private void SetChildIndex(int value)
        {
            ChildIndex = value;
        }

        public void SetParentIndex(int value)
        {
            ParentIndex = value;
        }

        public readonly (Vector3, Vector3, Vector3, 
               double, double, 
               PositionFrame, PositionFrame, Vector3, 
               Quaternion, Vector3, Quaternion, Vector3, 
               Vector3?, Matrix4x4?, Matrix4x4?) 
               GetAllFields()
        {
            return (_position, _previousPosition, _extrapolatedPosition, 
                    _updateTime, _previousUpdateTime, 
                    _currentTargetAnimatedBoneFrame, _lastTargetAnimatedBoneFrame, _currentFixedAnimatedBonePosition, 
                    _boneRotationChangeCheck, _bonePositionChangeCheck, _lastValidPoseBoneRotation, _lastValidPoseBoneLocalPosition,
                    _transformPosition, _fromLocalToWorld, _fromWorldToLocal);
        }


        public JiggleBone(UnityEngine.Transform transform, int index, int? parentIndex, Vector3 position, List<JiggleBone> bones)
        {
            Index = index;
            ParentIndex = parentIndex;
            _position = position;
            _previousPosition = position;
            if (transform != null)
            {
                _lastValidPoseBoneRotation = transform.localRotation;
                _lastValidPoseBoneLocalPosition = transform.localPosition;
                _transformPosition = transform.position;
                _fromLocalToWorld = transform.localToWorldMatrix;
                _fromWorldToLocal = transform.worldToLocalMatrix;
            }
            else
            {
                _lastValidPoseBoneRotation = default;
                _lastValidPoseBoneLocalPosition = default;
                _transformPosition = null;
                _fromLocalToWorld = null;
                _fromWorldToLocal = null;
            }

            _updateTime = Time.timeAsDouble;
            _previousUpdateTime = Time.timeAsDouble;
            _lastTargetAnimatedBoneFrame = new PositionFrame(position, Time.timeAsDouble);
            _currentTargetAnimatedBoneFrame = _lastTargetAnimatedBoneFrame;

            
            if (ParentIndex != null)
            {
                var bone = bones[ParentIndex.Value];
                bone.SetChildIndex(Index);
                bones[parentIndex.Value] = bone;
            }

            ChildIndex = null;

            _boneRotationChangeCheck = default;
            _bonePositionChangeCheck = default;
            _currentFixedAnimatedBonePosition = default;
            _extrapolatedPosition = default;
        }
        
        public readonly void PrepareBone(ref TransformAccess transform, bool transformExist)
        {
            // If bone is not animated, return to last unadulterated pose
            if (transformExist)
            {
                if (_boneRotationChangeCheck == transform.localRotation)
                {
                    transform.localRotation = _lastValidPoseBoneRotation;
                }
                if (_bonePositionChangeCheck == transform.localPosition)
                {
                    transform.localPosition = _lastValidPoseBoneLocalPosition;
                }
            }
        }
        
        public void CacheAnimationPosition(ref TransformAccess transform, ref NativeArray<JiggleBone> bones, double time, bool transformExist)
        {
            // Purely virtual particles need to reconstruct their desired position.
            _lastTargetAnimatedBoneFrame = _currentTargetAnimatedBoneFrame;
            if (!transformExist)
            {
                var parentTransformPosition = bones[ParentIndex.Value]._transformPosition.Value;
                if (bones[ParentIndex.Value].ParentIndex != null && 
                    bones[ParentIndex.Value].ParentIndex.Value != 0)
                {
                    Vector3 pos = bones[bones[ParentIndex.Value].ParentIndex.Value]._fromWorldToLocal.Value * parentTransformPosition;
                    pos = bones[ParentIndex.Value]._fromLocalToWorld.Value * pos;
                    _currentTargetAnimatedBoneFrame = new PositionFrame(pos, time);
                }
                else
                {
                    // parent.transform.parent is guaranteed to exist here, unless the user is jiggling a single bone by itself (which throws an exception).
                    Vector3 pos = bones[bones[ParentIndex.Value].ParentIndex.Value]._fromWorldToLocal.Value * parentTransformPosition;
                    pos = bones[ParentIndex.Value]._fromLocalToWorld.Value * pos;
                    _currentTargetAnimatedBoneFrame = new PositionFrame(pos, time);
                }
                return;
            }
            _currentTargetAnimatedBoneFrame = new PositionFrame(transform.position, time);
            _lastValidPoseBoneRotation = transform.localRotation;
            _lastValidPoseBoneLocalPosition = transform.localPosition;
        }
        
        public void Simulate(ref JiggleSettingsBase jiggleSettings, 
                             ref Vector3 wind, 
                             Vector3 gravity,
                             double accumulationTime, 
                             float fixedDeltaTime,
                             ref NativeArray<JiggleBone> bones)
        {
            _currentFixedAnimatedBonePosition = GetTargetBonePosition(_lastTargetAnimatedBoneFrame, _currentTargetAnimatedBoneFrame, accumulationTime);

            if (ParentIndex.Value == 0)
            {
                SetNewPosition(_currentFixedAnimatedBonePosition, accumulationTime);
                return;
            }
            var localSpaceVelocity = (_position - _previousPosition) - (bones[ParentIndex.Value]._position - bones[ParentIndex.Value]._previousPosition);
            var newPosition = NextPhysicsPosition(_position, 
                                                  _previousPosition, 
                                                  localSpaceVelocity, 
                                                  fixedDeltaTime,
                                                  gravity,
                                                  jiggleSettings.GravityMultiplier,
                                                  jiggleSettings.Friction,
                                                  jiggleSettings.AirFriction);

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
            var diff = next.time - prev.time;
            if (diff == 0)
            {
                return next.position;
            }
            var t = (time - prev.time) / diff;
            return Vector3.Lerp(prev.position, next.position, (float)t);
        }

        private void SetNewPosition(Vector3 newPosition, double time)
        {
            _previousUpdateTime = _updateTime;
            _previousPosition = _position;
            _updateTime = time;
            _position = newPosition;
        }

        public Vector3 DeriveFinalSolvePosition(Vector3 offset, float smoothing, double time, float fixedDeltaTime)
        {
            var t = ((time - smoothing * fixedDeltaTime) - _previousUpdateTime) / fixedDeltaTime;
            _extrapolatedPosition = offset + Vector3.LerpUnclamped(_previousPosition, _position, (float)t);
            return _extrapolatedPosition;
        }

        private static Vector3 NextPhysicsPosition(Vector3 newPosition, 
                                                  Vector3 previousPosition, 
                                                  Vector3 localSpaceVelocity, 
                                                  float deltaTime, 
                                                  Vector3 gravity, 
                                                  float gravityMultiplier, 
                                                  float friction, 
                                                  float airFriction)
        {
            var squaredDeltaTime = deltaTime * deltaTime;
            var vel = newPosition - previousPosition - localSpaceVelocity;
            return newPosition + vel * (1f - airFriction) + localSpaceVelocity * (1f - friction) + gravity * (gravityMultiplier * squaredDeltaTime);
        }

        private Vector3 ConstrainAngle(Vector3 newPosition, float elasticity, float elasticitySoften, ref NativeArray<JiggleBone> bones)
        {
            Vector3 parentParentPosition;
            Vector3 poseParentParent;
            if (bones[ParentIndex.Value].ParentIndex.Value == 0)
            {
                poseParentParent = bones[ParentIndex.Value]._currentFixedAnimatedBonePosition + 
                                  (bones[ParentIndex.Value]._currentFixedAnimatedBonePosition - _currentFixedAnimatedBonePosition);
                parentParentPosition = poseParentParent;
            }
            else
            {
                parentParentPosition = bones[bones[ParentIndex.Value].ParentIndex.Value]._position;
                poseParentParent = bones[bones[ParentIndex.Value].ParentIndex.Value]._currentFixedAnimatedBonePosition;
            }
            var parentAimTargetPose = bones[ParentIndex.Value]._currentFixedAnimatedBonePosition - poseParentParent;
            var parentAim = bones[ParentIndex.Value]._position - parentParentPosition;
            var TargetPoseToPose = Quaternion.FromToRotation(parentAimTargetPose, parentAim);
            var currentPose = _currentFixedAnimatedBonePosition - poseParentParent;
            var constraintTarget = TargetPoseToPose * currentPose;
            var error = Vector3.Distance(newPosition, parentParentPosition + constraintTarget);
            error /= GetLengthToParent(ref bones);
            error = Mathf.Clamp01(error);
            error = Mathf.Pow(error, elasticitySoften * 2f);
            return Vector3.Lerp(newPosition, parentParentPosition + constraintTarget, elasticity * error);
        }

        private Vector3 ConstrainLength(Vector3 newPosition, float elasticity, ref NativeArray<JiggleBone> bones)
        {
            var diff = newPosition - bones[ParentIndex.Value]._position;
            var dir = diff.normalized;
            return Vector3.Lerp(newPosition, bones[ParentIndex.Value]._position + dir * GetLengthToParent(ref bones), elasticity);
        }

        private float GetLengthToParent(ref NativeArray<JiggleBone> bones)
        {
            if (ParentIndex.Value == 0)
            {
                return 0.1f;
            }
            return Vector3.Distance(_currentFixedAnimatedBonePosition, bones[ParentIndex.Value]._currentFixedAnimatedBonePosition);
        }

        public void PoseBone(ref TransformAccess transform, float blend, ref NativeArray<JiggleBone> bones, bool transformExist)
        {
            if (ChildIndex != null)
            {
                var positionBlend = Vector3.Lerp(_currentTargetAnimatedBoneFrame.position, _extrapolatedPosition, blend);
                var childPositionBlend = Vector3.Lerp(bones[ChildIndex.Value]._currentTargetAnimatedBoneFrame.position,
                                                          bones[ChildIndex.Value]._extrapolatedPosition, 
                                                          blend);

                if (ParentIndex != null && ParentIndex.Value != 0)
                {
                    transform.position = positionBlend;
                }
                Vector3 childPosition;
                if (bones[ChildIndex.Value]._transformPosition == null)
                {
                    if (ParentIndex != null && ParentIndex.Value != 0)
                    { // If we have a proper jigglebone parent...
                        childPosition = bones[ParentIndex.Value]._fromWorldToLocal.Value * transform.position;
                        childPosition = transform.localToWorldMatrix * childPosition;
                    }
                    else
                    { // Otherwise we guess with the parent transform
                        childPosition = bones[ParentIndex.Value]._fromWorldToLocal.Value * transform.position;
                        childPosition = transform.localToWorldMatrix * childPosition;
                    }
                }
                else
                {
                    childPosition = bones[ChildIndex.Value]._transformPosition.Value;
                }
                var cachedAnimatedVector = childPosition - transform.position;
                var simulatedVector = childPositionBlend - positionBlend;
                var animPoseToPhysicsPose = Quaternion.FromToRotation(cachedAnimatedVector, simulatedVector);

                transform.rotation = animPoseToPhysicsPose * transform.rotation;
            }
            if (transformExist)
            {
                _boneRotationChangeCheck = transform.localRotation;
                _bonePositionChangeCheck = transform.localPosition;
            }
        }
    }
}