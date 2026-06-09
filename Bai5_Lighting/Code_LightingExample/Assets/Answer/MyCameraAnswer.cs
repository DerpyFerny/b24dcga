using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SocialPlatforms;
using static UnityEditor.PlayerSettings;
using static UnityEngine.GraphicsBuffer;

namespace Answer
{
    public class MyCameraAnswer : MonoBehaviour
    {
        public bool renderImage;
        public RenderTexture renderTexture;
        [Header("Camera Settings")]
        [Range(0, 120f)]
        public float fieldOfView = 30f;
        [Range(1, 1.5f)]
        public float aspectRatio = 1; //width devide by height
        public float near = 0;
        public float far = 0;

        [Header("View Screen Settings")]
        [Range(1, 500)]
        public int resolution = 0; //
        public bool isDrawPixel = false;

        [Header("Scene Object")]
        public SceneObjectAnswer[] objects;

        public MyPointLightAnswer[] lights;
        float maxBound = 1;
        private void OnDrawGizmos()
        {
            Vector3 cubeSize = new Vector3(0.1f, 0.1f, 0.2f);
            Vector3 origin = transform.position;

            Matrix4x4 trs = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);

            Gizmos.DrawCube(origin - Vector3.forward * cubeSize.z / 2, cubeSize);
            Gizmos.matrix = trs;
            Gizmos.DrawFrustum(Vector3.zero, fieldOfView, far, near, aspectRatio);
            //Gizmos.matrix = Matrix4x4.identity;

            //draw a view plane to view image
            Vector3 viewPlanePos = near * Vector3.forward;
            float viewPlaneHeight = near * Mathf.Tan(Mathf.Deg2Rad * fieldOfView / 2) * 2;
            float viewPlaneWidth = viewPlaneHeight * aspectRatio;
            Vector3 viewPlaneSize = new Vector3(viewPlaneWidth, viewPlaneHeight, 0.00001f);

            //rendering to image
            if (!renderImage)
            {
                return;
            }

            float time = Time.realtimeSinceStartup;

            Texture2D texture = new Texture2D((int)(aspectRatio * resolution), resolution);

            renderTexture.Release();

            renderTexture.width = (int)(aspectRatio * resolution);
            renderTexture.height = resolution;

            renderTexture.Create();

            //if (renderImage == false) return;

            if (isDrawPixel == false)
            {
                
                //draw view screen
                Gizmos.color = Color.white;
                Gizmos.DrawCube(viewPlanePos, viewPlaneSize);
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(viewPlanePos, viewPlaneSize);

            }
            else
            {
                float pixelHeight = viewPlaneHeight / resolution;
                float pixelWidth = viewPlaneWidth / (resolution * aspectRatio);

                List<Task<Color>> tasks = new List<Task<Color>>();

                //draw each pixel
                for (int i = 0; i < resolution * aspectRatio; i++)
                {
                    for (int j = 0; j < resolution; j++)
                    {
                        //calculate pixel pos and size
                        Vector3 pixelPos = new Vector3(pixelWidth / 2 + pixelWidth * i - viewPlaneWidth / 2,
                            pixelHeight / 2 + pixelHeight * j - viewPlaneHeight / 2,
                            viewPlanePos.z);
                        Vector3 pixelSize = new Vector3(pixelWidth, pixelHeight, 0.00001f);

                        //Ray tracing to calculate HitData
                        Vector3 targetColor = Vector3.zero;
                        int numbRay = 20;
                        for (int k = 0; k < numbRay; k++)
                        {
                            Vector3 destination = pixelPos;
                            destination.x = UnityEngine.Random.Range(pixelPos.x - pixelSize.x / 2, pixelPos.x + pixelSize.x / 2);
                            destination.y = UnityEngine.Random.Range(pixelPos.y - pixelSize.y / 2, pixelPos.y + pixelSize.y / 2);

                            Vector3 direction = (trs.MultiplyPoint(destination) - origin).normalized;

                            MyRay ray = new MyRay(origin, direction);

                            Color c = TraceRay(ray);

                            targetColor += new Vector3(c.r, c.g, c.b);
                        }
                        //Debug.Log(targetColor);
                        targetColor /= numbRay;

                        //change color of Pixel
                        texture.SetPixel(i, j, new Color(targetColor.x, targetColor.y, targetColor.z));
                        //render
                        //Draw pixel
                        //Gizmos.DrawCube(pixelPos, pixelSize);
                        //Gizmos.color = Color.green;
                        //Gizmos.DrawWireCube(pixelPos, pixelSize);
                    }
                }
       
                texture.Apply();

                RenderTexture.active = renderTexture;
                Graphics.Blit(texture, renderTexture);
                RenderTexture.active = null;
            }

            Debug.Log($"Finish rendering in {Time.realtimeSinceStartup - time}s" );

            //
            renderImage = false;
        }
        
        public bool TestShadow(Vector3 intersectionPoint)
        {
            bool isHitShadow = false;
            for (int i = 0; i < lights.Length; i++)
            {
                MyRay shadowRay = lights[i].SampleShadowRay(intersectionPoint);
                //
                for (int k = 0; k < objects.Length; k++)
                {
                    HitData data = objects[k].Intersect(shadowRay);
                    if (data != null && data.isIntersect == true)
                    {
                        //
                        Vector3 ho = (shadowRay.origin - data.intersectionPoint).normalized;
                        Vector3 hd = (intersectionPoint - data.intersectionPoint).normalized;
        
                        if (Vector3.Dot(ho, hd) < 0)
                        {
                            isHitShadow = true;
           
                            break;
                        }

                    }
                }

                if (isHitShadow) break;
            }

            return isHitShadow;
        }    

        public Color TraceRay(MyRay ray, int n = 0)
        {
            HitData targetData = null;
            for (int k = 0; k < objects.Length; k++)
            {
                HitData data = objects[k].Intersect(ray);
                if (data != null && data.isIntersect == true)
                {
                    if (targetData == null) targetData = data;
                    else
                    {

                        if (data.distance < targetData.distance)
                        {
                            targetData = data;
                        }
                    }
                }
            }

            //change color of Pixel
            if (targetData != null)
            {
                Color target = targetData.obj.GetFinalColor(ray, targetData, lights);
                //
                Vector3 finalColor = Vector3.zero;
                int numbShadowRay = 10;
                for(int i = 0; i < numbShadowRay; i ++)
                {
                    bool isHitShadow = TestShadow(targetData.intersectionPoint);
                    if (isHitShadow)
                    {
                        finalColor += Vector3.zero;
                    } else
                    {
                        finalColor += new Vector3(target.r, target.g, target.b);
                    }
                }
                finalColor /= numbShadowRay;

                target = new Color(finalColor.x, finalColor.y, finalColor.z);

                if (n == maxBound) return target;

                if(targetData.obj.reflectCoeff > 0)
                {
                    Vector3 reflectRay = Vector3.Reflect(ray.direction, targetData.normal);
                    return (1 - targetData.obj.reflectCoeff) * target 
                        + targetData.obj.reflectCoeff * TraceRay(new MyRay(targetData.intersectionPoint, reflectRay), n + 1);
                } 
                
                if(targetData.obj.glassRefract > 0)
                {
                    float glassN = 1.5f;
                    Vector3 refractRay = Refract(ray.direction, targetData.normal, 1, glassN);
                    //
                    Vector3 refractOrigin = targetData.intersectionPoint + refractRay * 0.1f;

                    if (targetData.obj is MySphereAnswer)
                    {
                        HitData sphereData = targetData.obj.Intersect(new MyRay(refractOrigin, refractRay));
                        
                        refractRay = Refract(refractRay, sphereData.normal, glassN, 1);
                        refractOrigin = sphereData.intersectionPoint + refractRay * 0.1f;
                    }

                    //
                    return (1 - targetData.obj.glassRefract) * target 
                        + targetData.obj.glassRefract * TraceRay(new MyRay(refractOrigin, refractRay), n + 1);
                }



                return target;
            }
            else
            {

                return Color.white;
            }

        }

        public Vector3 Refract(Vector3 I, Vector3 N, float n1, float n2)
        {
            float eta = n1 / n2;
            float cosI = -Vector3.Dot(N, I);
            float sinT2 = eta * eta * (1f - cosI * cosI);

            // Total internal reflection
            if (sinT2 > 1f)
                return Vector3.zero;

            float cosT = Mathf.Sqrt(1f - sinT2);

            Vector3 T = eta * I + (eta * cosI - cosT) * N;
            return T.normalized;
        }

    }



    public class MyRay
    {
        public Vector3 origin;
        public Vector3 direction;

        public MyRay(Vector3 _origin, Vector3 _direction)
        {
            origin = _origin; direction = _direction;
        }

        public void Draw()
        {
            Gizmos.DrawRay(origin, direction * 5f);
        }

    }

}


