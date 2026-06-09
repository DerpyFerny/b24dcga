


using UnityEngine;

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
        public MyMesh[] objects;

        float[,] depthBuffer;
        Color[,] frame;
        int width = 0;
        int height = 0;

        public bool isTransform3Dto2D = false;
        public bool isDrawFirstOnly => isDrawBound;
        public bool isDrawBound = false;
        public bool isPixelTest = false;

        private void OnDrawGizmos()
        {
            Vector3 cubeSize = new Vector3(0.1f, 0.1f, 0.2f);
            Vector3 origin = Vector3.zero;
            Matrix4x4 cameraMat = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale) * Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 180, 0), Vector3.one);
            Gizmos.matrix = cameraMat;
            Gizmos.DrawCube(origin - Vector3.forward * cubeSize.z / 2, cubeSize);
            Gizmos.DrawFrustum(origin, fieldOfView, far, near, aspectRatio);
            Gizmos.matrix = Matrix4x4.identity;

            //draw a view plane to view image
            Vector3 viewPlanePos = origin + near * Vector3.forward;
            float viewPlaneHeight = near * Mathf.Tan(Mathf.Deg2Rad * fieldOfView / 2) * 2;
            float viewPlaneWidth = viewPlaneHeight * aspectRatio;
            Vector3 viewPlaneSize = new Vector3(viewPlaneWidth, viewPlaneHeight, 0.00001f);

            //convert all mesh to triangle

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
                width = (int)(resolution * aspectRatio);
                height = resolution;

                //primary buffer
                frame = new Color[width, height];
                //depth buffer
                depthBuffer = new float[width, height];

                ClearBuffer();
                for (int i = 0; i < objects.Length; i++)
                {
                    Triangle[] triangles = objects[i].ToTriangles();
                    for (int j = 0; j < triangles.Length; j++)
                    {
                        DrawTriangle(triangles[j], objects[i], i == 0 && j == 0);
                    }
                }

                //draw each pixel
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        //calculate pixel pos and size
                        Vector3 pixelPos = new Vector3(pixelWidth / 2 + pixelWidth * i - viewPlaneWidth / 2,
                            pixelHeight / 2 + pixelHeight * j - viewPlaneHeight / 2,
                            viewPlanePos.z);
                        Vector3 pixelSize = new Vector3(pixelWidth, pixelHeight, 0.00001f);

                        //change color of Pixel
                        Gizmos.color = frame[i, j];

                        Gizmos.matrix = cameraMat;
                        //render
                        if (isShowRay)
                        {
                            if (frame[i, j] != Color.white) Gizmos.DrawRay(origin, (pixelPos - origin).normalized * 5f);
                        }

                        {
                            //Draw pixel
                            Gizmos.DrawCube(pixelPos, pixelSize);
                        }
                        Gizmos.color = Color.green;
                        Gizmos.DrawWireCube(pixelPos, pixelSize);
                    }
                }
            }

            //
        }

        void ClearBuffer()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    frame[i, j] = Color.white;
                    depthBuffer[i, j] = Mathf.Infinity;
                }
            }
        }

        void DrawTriangle(Triangle triangle, MyMesh mesh, bool isFirstTriangle)
        {
            Vector3 pixelA = VertexToPixel(mesh.transform, transform, triangle.pointA);
            Vector3 pixelB = VertexToPixel(mesh.transform, transform, triangle.pointB);
            Vector3 pixelC = VertexToPixel(mesh.transform, transform, triangle.pointC);

            if (isDrawFirstOnly && !isFirstTriangle) return;

            //-------------------------Start Question 2--------------------------------------
            //calculate bound
            int minX = 0;
            int maxX = 0;
            int minY = 0;
            int maxY = 0;

            //-------------------------End Question 2--------------------------------------

            if (isDrawBound)
            {
                for (int i = minX; i <= maxX; i++)
                {
                    for (int j = minY; j <= maxY; j++)
                    {
                        if (i == minX || i == maxX || j == minY || j == maxY)
                            frame[i, j] = Color.black;
                    }
                }
                if (!isTransform3Dto2D) return;
            }

            if (isTransform3Dto2D)
            {
                if (((int)pixelA.x >= 0 && (int)pixelA.x < width) && ((int)pixelA.y >= 0 && (int)pixelA.y < height))
                    frame[(int)pixelA.x, (int)pixelA.y] = triangle.colorA;

                if (((int)pixelB.x >= 0 && (int)pixelB.x < width) && ((int)pixelB.y >= 0 && (int)pixelB.y < height))
                    frame[(int)pixelB.x, (int)pixelB.y] = triangle.colorB;

                if (((int)pixelC.x >= 0 && (int)pixelC.x < width) && ((int)pixelC.y >= 0 && (int)pixelC.y < height))
                    frame[(int)pixelC.x, (int)pixelC.y] = triangle.colorC;

                if (!isPixelTest) return;
            }

            //draw inside triangle
            for (int i = minX; i <= maxX; i++)
            {
                for (int j = minY; j <= maxY; j++)
                {
                    Vector2 P = new Vector2(i, j);
                    Vector3 bary = GetBarycentricCoordinate(P, pixelA, pixelB, pixelC);

                    float sum = bary.x + bary.y + bary.z;
                    float offset = 0.01f;

                    //-------------------------Start Question 4--------------------------------------
                    if (sum > 1 - offset && sum < 1 + offset)
                    {
                        //update color and depth buffer
                        frame[i, j] = Color.white;
                        depthBuffer[i, j] = 0;
                    }
                    //-------------------------End Question 4--------------------------------------
                }
            }
        }

        Vector3 GetBarycentricCoordinate(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
        {
            //-------------------------Start Question 3--------------------------------------
            float alpha = 0;
            float beta = 0;
            float gamma = 0;

            return new Vector3(alpha, beta, gamma);
            //-------------------------End Question 3-----------------------------------------
        }

        Vector3 VertexToPixel(Transform meshTransform, Transform cameraTransform, Vector3 point)
        {
            //-------------------------Start Question 1--------------------------------------
            //calculate Model Matrix = Translate * Rotate * Scale
            Matrix4x4 modelMatrix = Matrix4x4.zero;

            //calculate View Matrix = cameraTRS ^-1
           
            Matrix4x4 viewMatrix = Matrix4x4.zero;

            //calculate Projection Matrix (n: near, f: far, t: top, b: bottom, r: right, l: left)
            // 2n/width    0      0      0
            //     0     2n/height 0      0
            //     0         0      (f + n)/(n - f) 2nf/(n - f)
            //     0         0            -1             0

            Matrix4x4 projectionMatrix = Matrix4x4.zero;


            Matrix4x4 mvp = projectionMatrix * viewMatrix * modelMatrix;

            //multiply vertex to matrix
            Vector4 point4D = new Vector4(point.x, point.y, point.z, 1);
            Vector4 pointRes = point4D;


            //convert to pixel coordinate
            float x = 0;
            float y = 0;

            //Gizmos.matrix = Matrix4x4.identity;

            return new Vector3(x, y, pointRes.z);
            //-------------------------End Question 1--------------------------------------
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


