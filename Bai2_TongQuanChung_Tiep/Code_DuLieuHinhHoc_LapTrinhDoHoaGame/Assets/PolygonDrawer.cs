using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PolygonDrawer : MonoBehaviour
{

    // Define the points of your polygon in local space
    public Color polygonColor;
    public Polygon polygon;

    void OnDrawGizmos()
    {
        // Ensure we are in the editor and have points to draw
        if (polygon.totalPoint < 3)
        {
            //khong ve
            return;
        }

        // Set the color
        Handles.color = polygonColor;

        //get world point from polygon
        Vector3[] worldPoints = polygon.ToListVec3();

        // Draw the anti-aliased convex polygon
        Handles.DrawAAConvexPolygon(worldPoints);
        Gizmos.DrawLineStrip(worldPoints, true);
    }
}

[Serializable]
public struct Polygon
{
    public int totalPoint;
    public Point[] points; //tap hop point

    //convert
    public Vector3[] ToListVec3()
    {
        Vector3[] list = new Vector3[totalPoint];
        for(int i = 0; i < totalPoint; i ++)
        {
            list[i] = points[i].ToVec3();
        }
        return list;
    }
}