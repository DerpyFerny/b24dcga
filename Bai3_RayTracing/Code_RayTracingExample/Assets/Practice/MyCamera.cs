using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

namespace Practice
{
    public class MyCamera : MonoBehaviour
    {
        [Header("Camera Settings")]
        [Range(0, 120f)]
        public float fieldOfView = 30f;
        [Range(1, 1.5f)]
        public float aspectRatio = 1; //width devide by height
        public float near = 0;
        public float far = 0;

        [Header("View Screen Settings")]
        [Range(1, 150)]
        public int resolution = 0; //
        public bool isDrawPixel = false;
        public bool isShowRay = false;

        [Header("Scene Object")]
        public SceneObject[] objects;

        private void OnDrawGizmos()
        {
            Vector3 cubeSize = new Vector3(0.1f, 0.1f, 0.2f);
            Vector3 origin = transform.position;
            Gizmos.DrawCube(origin - Vector3.forward * cubeSize.z / 2, cubeSize);
            Gizmos.DrawFrustum(origin, fieldOfView, far, near, aspectRatio);

            Vector3 lookDir = Vector3.forward;

            //----------Start Question 1---------------------------------------------------------
            //Calculate position and size of view plane
            Vector3 viewPlanePos = new Vector3();
            float viewPlaneHeight = 0;
            float viewPlaneWidth = 0;
            Vector3 viewPlaneSize = new Vector3(viewPlaneWidth, viewPlaneHeight, 0.00001f);
            //----------End Question 1-----------------------------------------------------------
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

                int numberOfVerticalPixel = resolution; //so pixel theo chieu doc
                int numberOfHorizontalPixel = (int)(resolution * aspectRatio); // so pixel theo chieu ngang

                //draw each pixel
                for (int i = 0; i < numberOfHorizontalPixel; i++)
                {
                    for (int j = 0; j < numberOfVerticalPixel; j++)
                    {
                        //----------Start Question 2---------------------------------------------------------
                        //calculate size of pixel
                        float pixelHeight = 0;
                        float pixelWidth = 0;

                        //calculate position of pixel
                        float pixelPosX = 0;
                        float pixelPosY = 0;
                        Vector3 pixelPos = new Vector3(pixelPosX, pixelPosY, viewPlanePos.z);
                        Vector3 pixelSize = new Vector3(pixelWidth, pixelHeight, 0.00001f);
                        //----------End Question 2-----------------------------------------------------------

                        //Ray tracing to calculate HitData
                        Vector3 direction = (pixelPos - origin).normalized;
                        MyRay ray = new MyRay(origin, direction);
                        HitData targetData = RayTrace(ray);

                        //change color of Pixel
                        DrawPixelData(ray, targetData, pixelPos, pixelSize);
                    }
                }
            }
        }

        public HitData RayTrace(MyRay ray)
        {
            HitData targetData = null;
            for (int k = 0; k < objects.Length; k++)
            {
                HitData data = objects[k].Intersect(ray);
                if (data != null)
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

            return targetData;
        }

        public void DrawPixelData(MyRay ray, HitData hitData, Vector3 pixelPos, Vector3 pixelSize)
        {
            if (hitData != null)
            {
                Gizmos.color = hitData.color;
            }
            else
            {
                Gizmos.color = Color.white;
            }

            //render
            if (isShowRay)
            {
                //Draw ray
                ray.Draw();
                Gizmos.DrawSphere(pixelPos, pixelSize.x / 5f);
            }
            else
            {
                //Draw pixel
                Gizmos.DrawCube(pixelPos, pixelSize);
            }
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(pixelPos, pixelSize);
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


