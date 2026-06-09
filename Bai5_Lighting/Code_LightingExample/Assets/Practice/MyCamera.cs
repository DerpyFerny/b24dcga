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

namespace Practice
{
    public class MyCamera : MonoBehaviour
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
        public SceneObject[] objects;

        public MyPointLight[] lights;
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

                        //----------Start Question 5--------
                        Vector3 direction = (trs.MultiplyPoint(pixelPos) - origin).normalized;

                        MyRay ray = new MyRay(origin, direction);

                        Color targetColor = TraceRay(ray);

                        //change color of Pixel
                        texture.SetPixel(i, j, targetColor);

                        //----------End Question 5---------

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
                //----------Start Question 5--------


                //----------End Question 5---------
                

                if (n == maxBound) return target;

                if (targetData.obj.reflectCoeff > 0)
                {
                    //------Start Question 3----------------

                    //------End Question 3----------------
                }

                if (targetData.obj.glassRefract > 0)
                {
                    float glassN = 1.5f;
                    //------Start Question 4----------------

                    //------End Question 4----------------
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


