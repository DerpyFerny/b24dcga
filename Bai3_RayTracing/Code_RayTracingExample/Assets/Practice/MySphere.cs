using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Practice
{
    public class MySphere : SceneObject
    {
        public Vector3 center;
        public float radius;
        public Color color;

        private void OnDrawGizmos()
        {
            center = transform.position;
            Gizmos.color = color;
            Gizmos.DrawSphere(center, radius);
        }

        public override HitData Intersect(MyRay ray)
        {
            //----------Start Question 3---------------------------------------------------------
            HitData hitData = new HitData();

            //check if object is in front of camera


            //check if ray is intersected with object and calculate distance from camera to hit point
            //delta = b*b - 4ac

            float a = 0;
            float b = 0;
            float c = 0;

            float delta = -1;

            if (delta < 0)
            {
                return null;
            }

            hitData.distance = 0;
            hitData.color = color;
            hitData.isIntersect = true;

            return hitData;

            //----------End Question 3---------------------------------------------------------
        }
    }
}

