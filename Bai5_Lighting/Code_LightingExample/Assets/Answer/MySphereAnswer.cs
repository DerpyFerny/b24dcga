using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Answer
{
    public class MySphereAnswer : SceneObjectAnswer
    {
        [HideInInspector]public Vector3 center;
        public float radius;

        private void OnDrawGizmos()
        {
            center = transform.position;
            Gizmos.color = meshColor;
            Gizmos.DrawSphere(center, radius);
        }

        public override HitData Intersect(MyRay ray)
        {
            HitData hitData = new HitData();
            center = transform.position;
            //
            Vector3 rsVec = center - ray.origin;
            float rsDotDir = Vector3.Dot(rsVec, ray.direction);

            if (Vector3.Dot(rsVec.normalized, ray.direction) < 0)
            {
                return null;
            }

            float a = ray.direction.sqrMagnitude;
            float b = -2 * rsDotDir;
            float c = rsVec.sqrMagnitude - radius * radius;

            //delta = b*b - 4ac
            float delta = b * b - 4 * a * c;

            if (delta < 0)
            {
                return null;
            }

            hitData.distance = (-b - Mathf.Sqrt(delta)) / (2 * a);

            if(hitData.distance <= 0)
            {
                hitData.distance = (-b + Mathf.Sqrt(delta)) / (2 * a);
            }

            hitData.intersectionPoint = ray.origin + hitData.distance * ray.direction;
            hitData.normal = (hitData.intersectionPoint - center).normalized;
            hitData.obj = this;

            float dirDotNor = Vector3.Dot(ray.direction, hitData.normal);
            hitData.isIntersect = dirDotNor < 0;

            return hitData;
        }
    }
}

