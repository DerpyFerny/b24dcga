using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Practice
{
    public class MyMesh : SceneObject
    {
        public Vector3[] vertices;
        public int[] indices;

        public void DrawMesh()
        {
            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = indices;
            mesh.RecalculateNormals();

            Gizmos.color = meshColor;
            Gizmos.DrawMesh(mesh, 0, transform.position, transform.rotation, transform.localScale);
            Gizmos.color = Color.white;
            Gizmos.DrawWireMesh(mesh, 0, transform.position, transform.rotation, transform.localScale);
        }

        private void OnDrawGizmos()
        {
            DrawMesh();
        }

        public override HitData Intersect(MyRay ray)
        {
            HitData hitData = new HitData();
            hitData.distance = float.PositiveInfinity;

            Triangle[] tris = ToTriangles();

            for(int i = 0; i < tris.Length; i ++)
            {
                Vector3 worldA = transform.localToWorldMatrix.MultiplyPoint(tris[i].pointA);
                Vector3 worldB = transform.localToWorldMatrix.MultiplyPoint(tris[i].pointB);
                Vector3 worldC = transform.localToWorldMatrix.MultiplyPoint(tris[i].pointC);

                Vector3 AB = worldB - worldA;
                Vector3 AC = worldC - worldA;
                Vector3 n = Vector3.Cross(AB, AC).normalized;

                if (Vector3.Dot(n, ray.direction) > 0) continue;

                Vector3 OA = worldA - ray.origin;

                float d = Vector3.Dot(OA, n) / Vector3.Dot(ray.direction, n);

                Vector3 intersectionPoint = ray.origin + d * ray.direction;

                //get barycentric coordinate
                Vector3 bary = GetBarycentricCoordinate(intersectionPoint, worldA, worldB, worldC);
                float sum = bary.x + bary.y + bary.z;

                if(sum > 0.99f && sum < 1.01)
                {
                    //intersectionPoint is in triangle
                    if(d < hitData.distance)
                    {
                        hitData.distance = d;
                        hitData.intersectionPoint = ray.origin + d * ray.direction;
                        //----Start Question 1 ----------------

                        hitData.normal = Vector3.zero;
                        //----End Question 1-------------------


                        hitData.obj = this;
                        hitData.isIntersect = true;
                    }    
                }    
            }    

            return hitData;
        }

        Vector3 GetBarycentricCoordinate(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
        {
            Vector3 PA = a - p;
            Vector3 PB = b - p;
            Vector3 PC = c - p;
            float areaPAB = PA.magnitude * PB.magnitude * Mathf.Sin(Vector3.Angle(PA, PB) * Mathf.Deg2Rad) / 2f;
            float areaPAC = PA.magnitude * PC.magnitude * Mathf.Sin(Vector3.Angle(PA, PC) * Mathf.Deg2Rad) / 2f;
            float areaPBC = PB.magnitude * PC.magnitude * Mathf.Sin(Vector3.Angle(PB, PC) * Mathf.Deg2Rad) / 2f;

            Vector3 AB = b - a;
            Vector3 AC = c - a;
            float areaABC = AB.magnitude * AC.magnitude * Mathf.Sin(Vector3.Angle(AB, AC) * Mathf.Deg2Rad) / 2f;

            return new Vector3(areaPBC / areaABC, areaPAC / areaABC, areaPAB / areaABC);
        }

        public Triangle[] ToTriangles()
        {
            List<Triangle> triangles = new List<Triangle>();

            for (int i = 0; i < indices.Length; i += 3)
            {
                triangles.Add(new Triangle(vertices[indices[i]], vertices[indices[i + 1]], vertices[indices[i + 2]], meshColor));
            }

            return triangles.ToArray();
        }
    }

    public class Triangle
    {
        public Vector3 pointA;
        public Vector3 pointB;
        public Vector3 pointC;

        public Color colorA;
        public Color colorB;
        public Color colorC;

        public Triangle(Vector3 _pointA, Vector3 _pointB, Vector3 _pointC, Color vertexColor)
        {
            pointA = _pointA;
            pointB = _pointB;
            pointC = _pointC;

            colorA = vertexColor;
            colorB = vertexColor;
            colorC = vertexColor;


        }
    }
}

