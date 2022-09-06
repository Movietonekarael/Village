using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameaCore.Editor
{
    //[RequireComponent(typeof(CapsuleCollider))]
    public class DrawCapsuleCollider : MonoBehaviour
    {
        private CapsuleCollider _capsuleCollider = null;
        private readonly float _colliderSizeCoefficient = .685f;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;

            if (_capsuleCollider = GetComponent<CapsuleCollider>())
            {
                var position = transform.position;
                var center = _capsuleCollider.center * _colliderSizeCoefficient;
                var height = _capsuleCollider.height * _colliderSizeCoefficient;
                var radius = _capsuleCollider.radius * _colliderSizeCoefficient;

                var deltaPosition = new Vector3(.0f, height / 2 - radius, .0f);

                var circlePosition = position + center + deltaPosition;
                DrawCircle(circlePosition, radius);
                DrawHalfSphere(circlePosition, radius, true);

                circlePosition = position + center - deltaPosition;
                DrawCircle(circlePosition, radius);
                DrawHalfSphere(circlePosition, radius, false);

                var point1 = position + center + deltaPosition + new Vector3(radius, .0f, .0f);
                var point2 = position + center - deltaPosition + new Vector3(radius, .0f, .0f);
                Gizmos.DrawLine(point1, point2);

                point1 = position + center + deltaPosition + new Vector3(-radius, .0f, .0f);
                point2 = position + center - deltaPosition + new Vector3(-radius, .0f, .0f);
                Gizmos.DrawLine(point1, point2);

                point1 = position + center + deltaPosition + new Vector3(.0f, .0f, radius);
                point2 = position + center - deltaPosition + new Vector3(.0f, .0f, radius);
                Gizmos.DrawLine(point1, point2);

                point1 = position + center + deltaPosition + new Vector3(.0f, .0f, -radius);
                point2 = position + center - deltaPosition + new Vector3(.0f, .0f, -radius);
                Gizmos.DrawLine(point1, point2);
            }
            
        }

        private void DrawCircle(Vector3 position, float radius, uint sectors = 32)
        {
            for (var i = 0; i < sectors; i++)
            {
                var angle = 2 * Mathf.PI * i / sectors;
                var point1 = new Vector3(position.x + radius * Mathf.Cos(angle), position.y, position.z + radius * Mathf.Sin(angle));
                angle = 2 * Mathf.PI * (i + 1) / sectors;
                var point2 = new Vector3(position.x + radius * Mathf.Cos(angle), position.y, position.z + radius * Mathf.Sin(angle));
                Gizmos.DrawLine(point1, point2);
            }
        }

        private void DrawHalfSphere(Vector3 position, float radius, bool isUpward = true, uint sectors = 16)
        {
            for (var i = 0; i < sectors; i++)
            {
                var coef = isUpward ? 1 : -1;

                var angle = Mathf.PI * i / sectors;
                var pointZ1 = new Vector3(position.x, position.y + radius * Mathf.Sin(angle) * coef, position.z + radius * Mathf.Cos(angle) * coef);
                var pointX1 = new Vector3(position.x + radius * Mathf.Cos(angle) * coef, position.y + radius * Mathf.Sin(angle) * coef, position.z);

                angle = Mathf.PI * (i + 1) / sectors;
                var pointZ2 = new Vector3(position.x, position.y + radius * Mathf.Sin(angle) * coef, position.z + radius * Mathf.Cos(angle) * coef);
                var pointX2 = new Vector3(position.x + radius * Mathf.Cos(angle) * coef, position.y + radius * Mathf.Sin(angle) * coef, position.z);

                Gizmos.DrawLine(pointZ1, pointZ2);
                Gizmos.DrawLine(pointX1, pointX2);
            }
        }
    }

}