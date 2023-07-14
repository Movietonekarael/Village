using System.Collections.Generic;
using UnityEngine;
using System;


namespace Testing
{
    public partial class JiggleBonesTesting
    {
        private partial struct BoneTrace : IEquatable<BoneTrace>
        {
            private Vector3 _position;
            private Vector3 _previousPosition;
            private Vector3 _extrapolatedPosition;

            private readonly double _updateTime;
            private readonly double _previousUpdateTime;

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

            public BoneTrace(JiggleBones.JiggleBone bone)
            {
                var values = bone.GetAllFields();
                _position = values.Item1;
                _previousPosition = values.Item2;
                _extrapolatedPosition = values.Item3;
                _updateTime = values.Item4;
                _previousUpdateTime = values.Item5;
                _currentTargetAnimatedBoneFrame = new(values.Item6);
                _lastTargetAnimatedBoneFrame = new(values.Item7);
                _currentFixedAnimatedBonePosition = values.Item8;
                _boneRotationChangeCheck = values.Item9;
                _bonePositionChangeCheck = values.Item10;
                _lastValidPoseBoneRotation = values.Item11;
                _lastValidPoseBoneLocalPosition = values.Item12;
                _transformPosition = values.Item13;
                _fromLocalToWorld = values.Item14;
                _fromWorldToLocal = values.Item15;
            }

            public BoneTrace(JigglePhysics.JiggleBone bone)
            {
                _position = bone.position;
                _previousPosition = bone.previousPosition;
                _extrapolatedPosition = bone.extrapolatedPosition;
                _updateTime = bone.updateTime;
                _previousUpdateTime = bone.previousUpdateTime;
                _currentTargetAnimatedBoneFrame = new(bone.currentTargetAnimatedBoneFrame);
                _lastTargetAnimatedBoneFrame = new(bone.lastTargetAnimatedBoneFrame);
                _currentFixedAnimatedBonePosition = bone.currentFixedAnimatedBonePosition;
                _boneRotationChangeCheck = bone.boneRotationChangeCheck;
                _bonePositionChangeCheck = bone.bonePositionChangeCheck;
                _lastValidPoseBoneRotation = bone.lastValidPoseBoneRotation;
                _lastValidPoseBoneLocalPosition = bone.lastValidPoseBoneLocalPosition;
                if (bone.transform != null)
                {
                    _transformPosition = bone.transform.position;
                    _fromLocalToWorld = bone.transform.localToWorldMatrix;
                    _fromWorldToLocal = bone.transform.worldToLocalMatrix;
                }
                else
                {
                    _transformPosition = null;
                    _fromLocalToWorld = null;
                    _fromWorldToLocal = null;
                }
            }

            public override bool Equals(object obj)
            {
                return obj is BoneTrace trace && Equals(trace);
            }

            public bool Equals(BoneTrace other)
            {
                return _position.Equals(other._position) &&
                       _previousPosition.Equals(other._previousPosition) &&
                       _extrapolatedPosition.Equals(other._extrapolatedPosition) &&
                       _updateTime == other._updateTime &&
                       _previousUpdateTime == other._previousUpdateTime &&
                       _currentTargetAnimatedBoneFrame.Equals(other._currentTargetAnimatedBoneFrame) &&
                       _lastTargetAnimatedBoneFrame.Equals(other._lastTargetAnimatedBoneFrame) &&
                       _currentFixedAnimatedBonePosition.Equals(other._currentFixedAnimatedBonePosition) &&
                       _boneRotationChangeCheck.Equals(other._boneRotationChangeCheck) &&
                       _bonePositionChangeCheck.Equals(other._bonePositionChangeCheck) &&
                       _lastValidPoseBoneRotation.Equals(other._lastValidPoseBoneRotation) &&
                       _lastValidPoseBoneLocalPosition.Equals(other._lastValidPoseBoneLocalPosition) &&
                       EqualityComparer<Vector3?>.Default.Equals(_transformPosition, other._transformPosition) &&
                       EqualityComparer<Matrix4x4?>.Default.Equals(_fromLocalToWorld, other._fromLocalToWorld) &&
                       EqualityComparer<Matrix4x4?>.Default.Equals(_fromWorldToLocal, other._fromWorldToLocal);
            }

            public override int GetHashCode()
            {
                var hash = new HashCode();
                hash.Add(_position);
                hash.Add(_previousPosition);
                hash.Add(_extrapolatedPosition);
                hash.Add(_updateTime);
                hash.Add(_previousUpdateTime);
                hash.Add(_currentTargetAnimatedBoneFrame);
                hash.Add(_lastTargetAnimatedBoneFrame);
                hash.Add(_currentFixedAnimatedBonePosition);
                hash.Add(_boneRotationChangeCheck);
                hash.Add(_bonePositionChangeCheck);
                hash.Add(_lastValidPoseBoneRotation);
                hash.Add(_lastValidPoseBoneLocalPosition);
                hash.Add(_transformPosition);
                hash.Add(_fromLocalToWorld);
                hash.Add(_fromWorldToLocal);
                return hash.ToHashCode();
            }

            public static bool operator ==(BoneTrace a, BoneTrace b)
            {
                var equality = true;
                equality = equality && BonesValueEquals(a._position, b._position, "Position");
                equality = equality && BonesValueEquals(a._previousPosition, b._previousPosition, "PreviousPosition");
                equality = equality && BonesValueEquals(a._extrapolatedPosition, b._extrapolatedPosition, "ExtrapolatedPosition");
                equality = equality && BonesValueEquals(a._updateTime, b._updateTime, "UpdateTime");
                equality = equality && BonesValueEquals(a._previousUpdateTime, b._previousUpdateTime, "PreviousUpdateTime");
                equality = equality && BonesValueEquals(a._currentTargetAnimatedBoneFrame, b._currentTargetAnimatedBoneFrame, "CurrentTargetAnimatedBoneFrame");
                equality = equality && BonesValueEquals(a._lastTargetAnimatedBoneFrame, b._lastTargetAnimatedBoneFrame, "LastTargetAnimatedBoneFrame");
                equality = equality && BonesValueEquals(a._currentFixedAnimatedBonePosition, b._currentFixedAnimatedBonePosition, "CurrentFixedAnimatedBonePosition");
                equality = equality && BonesValueEquals(a._boneRotationChangeCheck, b._boneRotationChangeCheck, "boneRotationChangeCheck");
                equality = equality && BonesValueEquals(a._bonePositionChangeCheck, b._bonePositionChangeCheck, "bonePositionChangeCheck");
                equality = equality && BonesValueEquals(a._lastValidPoseBoneRotation, b._lastValidPoseBoneRotation, "lastValidPoseBoneRotation");
                equality = equality && BonesValueEquals(a._lastValidPoseBoneLocalPosition, b._lastValidPoseBoneLocalPosition, "lastValidPoseBoneLocalPosition");
                equality = equality && BonesValueEquals(a._transformPosition, b._transformPosition, "transformPosition");
                equality = equality && BonesValueEquals(a._fromLocalToWorld, b._fromLocalToWorld, "fromLocalToWorld");
                equality = equality && BonesValueEquals(a._fromWorldToLocal, b._fromWorldToLocal, "fromWorldToLocal");
                return equality;
            }

            public static bool operator !=(BoneTrace a, BoneTrace b)
            {
                return !(a == b);
            }

            private static bool BonesValueEquals<T>(T a, T b, string name) where T : IEquatable<T>
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
}
