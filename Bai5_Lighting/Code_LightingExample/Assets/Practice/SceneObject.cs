using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Practice
{
    public enum Model { Lambert, Phong, BlinnPhong }
    public class SceneObject : MonoBehaviour
    {
        public Color meshColor;

        [Range(0f, 1f)]
        public float reflectCoeff = 0;

        [Range(0f, 1f)]
        public float glassRefract = 0;

        Model model = Model.Phong;

        public virtual HitData Intersect(MyRay ray)
        {
            HitData hitData = new HitData();
            
            return hitData;
        }

        public Color GetFinalColor(MyRay myRay, HitData data, MyPointLight[] lights)
        {
            Color color = Color.black;

            for(int i = 0; i < lights.Length; i ++)
            {
                MyRay lightRay = lights[i].GetLightRay(data.intersectionPoint);
                
                switch(model)
                {
                    case Model.Lambert:
                        color += GetLambertReflectionColor(data.normal, lightRay.direction, lights[i].lightColor);
                        break;
                    case Model.Phong:
                        color += GetPhongReflectionColor(myRay.direction, data.normal, lightRay.direction, lights[i].lightColor);
                        break;

                    case Model.BlinnPhong:
                        color += GetBlinnPhongReflectionColor(myRay.direction, data.normal, lightRay.direction, lights[i].lightColor);
                        break;
                }    
              
            }    

            return color;
        }

        //----Start Question 2------------------

        public Color GetBlinnPhongReflectionColor(Vector3 eye, Vector3 normal, Vector3 lightDirection, Color lightColor)
        {

            return meshColor;
        }

        public Color GetPhongReflectionColor(Vector3 eye, Vector3 normal, Vector3 lightDirection, Color lightColor)
        {

            return meshColor ;
        }    

        

        public Color GetLambertReflectionColor(Vector3 normal, Vector3 lightDirection, Color lightColor)
        {

            return meshColor;
        }

        //----End Question 2------------------
    }

    public class HitData
    {
        public SceneObject obj;
        public float distance;
        public Vector3 normal;
        public Vector3 intersectionPoint;
        public bool isIntersect;
    }

}

