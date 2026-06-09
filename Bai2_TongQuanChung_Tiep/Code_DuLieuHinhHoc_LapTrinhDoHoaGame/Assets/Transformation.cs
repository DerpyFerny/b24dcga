using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/*
 * Matrix Calculating
 * Affine Transformation
*/

public class Transformation : MonoBehaviour
{
    public Sphere sun;
    public Sphere earth;
    [Header("transform")]
    public Point translateVector;
    private void OnDrawGizmos()
    {
        //fix starting position of sun and earth
        sun.ResetCenter();
        earth.ResetCenter();

        //sun
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(sun.center.ToVec3(), sun.radius);
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(sun.center.ToVec3(), sun.radius);

        //translate earth a vector (2, 0, 0)
        Matrix4 translateMatrix = new Matrix4().Translate(translateVector);
        earth.center = translateMatrix.MultiplyToPoint(earth.center);
        translateMatrix.print();

        //implement code to make earth auto rotate arround sun

        //earth
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(earth.center.ToVec3(), earth.radius);
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(earth.center.ToVec3(), earth.radius);
    }


}

public struct Matrix4
{
    float[,] mat;

    public Point MultiplyToPoint(Point point)
    {
        if (mat == null) return new Point(); //if mat is Null return (0,0,0)

        Point result = new Point();
        
        //convert point3D to point4D
        float[] point4D = new float[4] {point.x, point.y, point.z, 1};

        //Muliply matrix to Vector
        //point[0] = row[0, 0] * point[0] + row[1, 0] * point[1] + ... + row[n, 0] * point[n]

        return result;
    }

    public Matrix4 MultiplyToMatrix(Matrix4 targetMatrix)
    {
        if (mat == null) return new Matrix4(); //if mat is Null return (0,0,0)

        Matrix4 result = new Matrix4().Identity();

        //Multiply current matrix to targetMatrix

        return result;
    }

    public Matrix4 Identity()
    {
        // 1 0 0 0
        // 0 1 0 0
        // 0 0 1 0
        // 0 0 0 1

        mat = new float[4, 4]
        {
            { 1, 0, 0, 0 },   
            { 0, 1, 0, 0 },   
            { 0, 0, 1, 0 },   
            { 0, 0, 0, 1 }    
        };
        return this;
    }

    public Matrix4 Translate(Point translate)
    {
        // 1 0 0 0
        // 0 1 0 0
        // 0 0 1 0
        // translateX translateY translateZ 1

        mat = new float[4, 4]
        {
            { 1, 0, 0, 0 },
            { 0, 1, 0, 0 },
            { 0, 0, 1, 0 },
            { translate.x, translate.y, translate.z, 1 }
        };

        return this;
    }

    public void print()
    {
        Debug.Log(mat[0, 0] + " " + mat[0, 1] + " " + mat[0, 2] + " " + mat[0, 3]);
        Debug.Log(mat[1, 0] + " " + mat[1, 1] + " " + mat[1, 2] + " " + mat[1, 3]);
        Debug.Log(mat[2, 0] + " " + mat[2, 1] + " " + mat[2, 2] + " " + mat[2, 3]);
        Debug.Log(mat[3, 0] + " " + mat[3, 1] + " " + mat[3, 2] + " " + mat[3, 3]);
    }

    public Matrix4 Scale(Point scale)
    {
        // scaleX 0 0 0
        // 0 scaleY 0 0
        // 0 0 scaleZ 0
        // 0 0 0 1

        return this;
    }

    public Matrix4 Rotate(Point rotate)
    {
        Matrix4 matRotateX = new Matrix4().RotateAroundX(rotate.x);
        Matrix4 matRotateY = new Matrix4().RotateAroundY(rotate.y);
        Matrix4 matRotateZ = new Matrix4().RotateAroundZ(rotate.z);
        return matRotateX.MultiplyToMatrix(matRotateY).MultiplyToMatrix(matRotateZ);
    }

    public Matrix4 RotateAroundX(float theta)
    {
        // 1 0 0 0
        // 0 cos(theta) -sin(theta) 0
        // 0 sin(theta) cos(theta) 0
        // 0 0 0 1

        return this;
    }

    public Matrix4 RotateAroundY(float theta)
    {
        // cos(theta) 0 sin(theta) 0
        // 0 1 0 0
        // -sin(theta) 0 cos(theta) 0
        // 0 0 0 1

        return this;
    }

    public Matrix4 RotateAroundZ(float theta)
    {
        // cos(theta) -sin(theta) 0 0
        // sin(theta) cos(theta) 0 0
        // 0 0 1 0
        // 0 0 0 1

        return this;
    }
}

