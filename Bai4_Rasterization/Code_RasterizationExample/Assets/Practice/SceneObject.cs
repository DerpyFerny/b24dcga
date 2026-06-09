using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Practice
{
    public class SceneObject : MonoBehaviour
    {
        public virtual HitData Intersect(MyRay ray)
        {
            return null;
        }
    }

    public class HitData
    {
        public Color color;
        public float distance;
        public bool isIntersect;
    }

}

