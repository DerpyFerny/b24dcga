using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Answer
{
    public class SceneObjectAnswer : MonoBehaviour
    {
        public virtual HitData Intersect(MyRay ray)
        {
            HitData hitData = new HitData();
            return hitData;
        }
    }

    public class HitData
    {
        public Color color;
        public float distance;
        public bool isIntersect;
    }

}

