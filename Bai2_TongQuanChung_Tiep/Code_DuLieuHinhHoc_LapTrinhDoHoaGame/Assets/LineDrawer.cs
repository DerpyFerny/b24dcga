using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LineDrawer : MonoBehaviour
{
    public Color color;
    public Point pointA;
    public Point pointB;
    
    private void OnDrawGizmos()
    {
        Line line = new Line();
        line.start = pointA;
        line.end = pointB;
        Gizmos.color = color;
        line.DrawLine();
    }
}

[Serializable]
public struct Point
{
    public float x;
    public float y;
    public float z; //3D

    public Point Subtract(Point B)
    {

        Point result = new Point();
        //
        result.x = B.x - x;
        result.y = B.y - y;
        result.z = B.z - z;


        return result;
    }

    public float Length()
    {
        // sqrt (resultx^2 + resulty^2 + resultz^2)
        float result = Mathf.Sqrt(x * x + y * y + z * z);
        return result;
    }

    //covert point to vec3
    public Vector3 ToVec3()
    {
        return new Vector3(x, y, 0);
    }
}

[Serializable]
public struct Line
{
    public Point start;
    public Point end;

    public void DrawLine()
    {
        Gizmos.DrawLine(start.ToVec3(), end.ToVec3());
    }
}