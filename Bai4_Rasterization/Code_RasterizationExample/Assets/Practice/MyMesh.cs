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
        public Color meshColor;

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