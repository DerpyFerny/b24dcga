using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Answer
{
    public enum Model { Lambert, Phong, BlinnPhong }
    public class SceneObjectAnswer : MonoBehaviour
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

        public Color GetFinalColor(MyRay myRay, HitData data, MyPointLightAnswer[] lights)
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

        public Color GetBlinnPhongReflectionColor(Vector3 eye, Vector3 normal, Vector3 lightDirection, Color lightColor)
        {

            Vector3 h = (eye + lightDirection).normalized;
            float specular = Mathf.Pow(Vector3.Dot(normal, h), 8);
            float diffuse = Vector3.Dot(normal, lightDirection);

            float specularCoeff = 0.2f;

            return meshColor * lightColor * ((1 - specularCoeff) * diffuse + specularCoeff * specular);
        }

        public Color GetPhongReflectionColor(Vector3 eye, Vector3 normal, Vector3 lightDirection, Color lightColor)
        {
            
            Vector3 r = Vector3.Reflect(-lightDirection, normal);
            float specular = Mathf.Pow(Vector3.Dot(eye, r), 8);
            float diffuse = Vector3.Dot(normal, lightDirection);

            float specularCoeff = 0.2f;

            return meshColor * lightColor * ((1 - specularCoeff) * diffuse + specularCoeff * specular);
        }    

        

        public Color GetLambertReflectionColor(Vector3 normal, Vector3 lightDirection, Color lightColor)
        {
            float diffuse = Vector3.Dot(normal, lightDirection);
            diffuse = Mathf.Clamp(diffuse, 0.0f, 1.0f);
            return meshColor * diffuse * lightColor;
        }    
    }

    public class HitData
    {
        public SceneObjectAnswer obj;
        public float distance;
        public Vector3 normal;
        public Vector3 intersectionPoint;
        public bool isIntersect;
    }

}

