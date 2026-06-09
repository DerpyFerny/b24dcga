


using UnityEngine;

namespace Answer
{
    public class MyCameraAnswer : MonoBehaviour
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
        public MyMeshAnswer[] objects;

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
                            if (frame[i , j] != Color.white) Gizmos.DrawRay(origin, (pixelPos - origin).normalized * 5f);
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
                    depthBuffer[i , j] = Mathf.Infinity;
                }
            }
        }

        void DrawTriangle(Triangle triangle, MyMeshAnswer mesh, bool isFirstTriangle)
        {
            Vector3 pixelA = VertexToPixel(mesh.transform, transform, triangle.pointA);
            Vector3 pixelB = VertexToPixel(mesh.transform, transform, triangle.pointB);
            Vector3 pixelC = VertexToPixel(mesh.transform, transform, triangle.pointC);

            if (isDrawFirstOnly && !isFirstTriangle) return;

            //-------------------------Start Question 2--------------------------------------
            //calculate bound
            int minX = Mathf.Min(Mathf.Min((int)pixelA.x, (int)pixelB.x), (int)pixelC.x);
            int maxX = Mathf.Max(Mathf.Max((int)pixelA.x, (int)pixelB.x), (int)pixelC.x);
            int minY = Mathf.Min(Mathf.Min((int)pixelA.y, (int)pixelB.y), (int)pixelC.y);
            int maxY = Mathf.Max(Mathf.Max((int)pixelA.y, (int)pixelB.y), (int)pixelC.y);

            minX = Mathf.Clamp(minX, 0, width - 1);
            maxX = Mathf.Clamp(maxX, 0, width - 1);
            minY = Mathf.Clamp(minY, 0, height - 1);
            maxY = Mathf.Clamp(maxY, 0, height - 1);
            //-------------------------End Question 2--------------------------------------

            if (isDrawBound)
            {
                for (int i = minX; i <= maxX; i++)
                {
                    for (int j = minY; j <= maxY; j++)
                    {
                        if(i == minX || i == maxX || j == minY || j == maxY)
                            frame[i, j] = Color.black;
                    }
                }
                if(!isTransform3Dto2D) return;
            }

            if (isTransform3Dto2D)
            {
                if(((int)pixelA.x >= 0 && (int)pixelA.x < width) && ((int)pixelA.y >= 0 && (int)pixelA.y < height)) 
                    frame[(int)pixelA.x, (int)pixelA.y] = triangle.colorA;

                if (((int)pixelB.x >= 0 && (int)pixelB.x < width) && ((int)pixelB.y >= 0 && (int)pixelB.y < height))
                    frame[(int)pixelB.x, (int)pixelB.y] = triangle.colorB;

                if (((int)pixelC.x >= 0 && (int)pixelC.x < width) && ((int)pixelC.y >= 0 && (int)pixelC.y < height))
                    frame[(int)pixelC.x, (int)pixelC.y] = triangle.colorC;

                if(!isPixelTest) return;
            }

            //draw inside triangle
            for (int i = minX; i <= maxX; i++)
            {
                for(int j = minY; j <= maxY; j++)
                {
                    Vector2 P = new Vector2(i, j);
                    Vector3 bary = GetBarycentricCoordinate(P, pixelA, pixelB, pixelC);

                    float sum = bary.x + bary.y + bary.z;
                    float offset = 0.01f;

                    //-------------------------Start Question 4--------------------------------------
                    if (sum > 1 - offset && sum < 1 + offset)
                    {
                        float depth = bary.x * pixelA.z 
                            + bary.y * pixelB.z + bary.z * pixelC.z;

                        if(depth <= depthBuffer[i, j])
                        {
                            //update color and depth buffer
                            frame[i, j] = bary.x * triangle.colorA 
                                + bary.y * triangle.colorB 
                                + bary.z * triangle.colorC;
                            depthBuffer[i, j] = depth;
                        }
                    }
                    //-------------------------End Question 4--------------------------------------
                }
            }
        }

        Vector3 GetBarycentricCoordinate(Vector2 p, Vector2 a, Vector2 b, Vector2 c)
        {
            //-------------------------Start Question 3--------------------------------------
            Vector2 PA = a - p;
            Vector2 PB = b - p;
            Vector2 PC = c - p;
            float areaPAB = PA.magnitude * PB.magnitude * Mathf.Sin(Vector2.Angle(PA, PB) * Mathf.Deg2Rad) / 2f;
            float areaPAC = PA.magnitude * PC.magnitude * Mathf.Sin(Vector2.Angle(PA, PC) * Mathf.Deg2Rad) / 2f;
            float areaPBC = PB.magnitude * PC.magnitude * Mathf.Sin(Vector2.Angle(PB, PC) * Mathf.Deg2Rad) / 2f;

            Vector2 AB = b - a;
            Vector2 AC = c - a;
            float areaABC = AB.magnitude * AC.magnitude * Mathf.Sin(Vector2.Angle(AB, AC) * Mathf.Deg2Rad) / 2f;

            return new Vector3(areaPBC / areaABC, areaPAC / areaABC, areaPAB / areaABC);
            //-------------------------End Question 3-----------------------------------------
        }

        Vector3 VertexToPixel(Transform meshTransform, Transform cameraTransform, Vector3 point)
        {
            //-------------------------Start Question 1--------------------------------------
            //calculate Model Matrix = Translate * Rotate * Scale
            Matrix4x4 modelMatrix = Matrix4x4.TRS(meshTransform.position, meshTransform.rotation, meshTransform.localScale);

            //calculate View Matrix = cameraTRS ^-1
            Matrix4x4 cameraModelMatrix = Matrix4x4.TRS(cameraTransform.position, cameraTransform.rotation, cameraTransform.localScale);
            
            Matrix4x4 viewMatrix = cameraModelMatrix.inverse;

            //calculate Projection Matrix (n: near, f: far, t: top, b: bottom, r: right, l: left)
            // 2n/width    0      0      0
            //     0     2n/height 0      0
            //     0         0      (f + n)/(n - f) 2nf/(n - f)
            //     0         0            -1             0

            Matrix4x4 projectionMatrix = Matrix4x4.zero;
            float viewPlaneHeight = near * Mathf.Tan(Mathf.Deg2Rad * fieldOfView / 2) * 2;
            float viewPlaneWidth = viewPlaneHeight * aspectRatio;
            projectionMatrix[0, 0] = 2 * near / viewPlaneWidth;
            projectionMatrix[1, 1] = 2 * near / viewPlaneHeight;
            projectionMatrix[2, 2] = (far + near) / (near - far);
            projectionMatrix[2, 3] = 2 * near * far / (near - far);
            projectionMatrix[3, 2] = -1;

            Matrix4x4 mvp = projectionMatrix * viewMatrix * modelMatrix;

            //multiply vertex to matrix
            Vector4 point4D = new Vector4(point.x, point.y, point.z, 1);
            Vector4 pointRes = mvp * point4D;
            pointRes = pointRes * (1 / pointRes.w);

            //convert to pixel coordinate
            float x = (-pointRes.x + 1) / 2;
            float y = (pointRes.y + 1) / 2;

            x = Mathf.Floor(x * width);
            y = Mathf.Floor(y * height);

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


