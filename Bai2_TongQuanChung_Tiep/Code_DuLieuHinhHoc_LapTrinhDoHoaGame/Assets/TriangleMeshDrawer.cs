using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriangleMeshDrawer : MonoBehaviour
{
    public Color triangleColor;
    public Triangle triangle;
    private void OnDrawGizmos()
    {
        Gizmos.color = triangleColor;
        Gizmos.DrawMesh(triangle.ToMesh());
        Gizmos.DrawWireMesh(triangle.ToMesh());

        Vector3 AB = triangle.pointB.ToVec3() - triangle.pointA.ToVec3();
        Vector3 AC = triangle.pointC.ToVec3() - triangle.pointA.ToVec3();
        Vector3 normal = Vector3.Cross(AC, AB);
        Vector3 normal2 = Vector3.Cross(AB, AC);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(triangle.pointA.ToVec3(), triangle.pointA.ToVec3() + 1f * normal.normalized);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(triangle.pointA.ToVec3(), triangle.pointA.ToVec3() + 1f * normal2.normalized);
    }
}

[Serializable]
public struct Triangle
{
    public Point pointA;
    public Point pointB;
    public Point pointC;
    public bool isFlip;
    public Mesh ToMesh()
    {
        Mesh mesh = new Mesh();
        Vector3[] vert = new Vector3[3] { pointA.ToVec3(), pointB.ToVec3(), pointC.ToVec3() };
        int[] triangle = new int[3] { 0, 1, 2 }; //A B C
        
        if(isFlip) 
            triangle = new int[3] { 0, 2, 1 }; // A C B

        //tich co huong AB AC
        //tich co huong AC AB


        //update to mesh
        mesh.vertices = vert;
        mesh.triangles = triangle;
        mesh.RecalculateNormals();
        return mesh;
    }
}