using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * the structure of a Cube and a Box
 * Box, Cube collider
*/
public class BoxDrawer : MonoBehaviour
{
    [Header("Cube settings")]
    public Color cubeColor;
    public Cube cube;

    [Header("Box Settings")]
    public Color boxColor;
    public Box box;
    private void OnDrawGizmos()
    {
        Gizmos.color = cubeColor;

        //change color of gizmos to red if two objects are collided

        Gizmos.DrawCube(cube.position.ToVec3(), cube.ShapeSize());

        Gizmos.color = boxColor;

        //change color of gizmos to red if two objects are collided

        Gizmos.DrawCube(box.position.ToVec3(), box.ShapeSize());

        
    }
}

[Serializable]
public struct Box
{
    public int width, height, depth;
    public Point position;

    public Vector3 ShapeSize()
    {
        return new Vector3(width, height, depth);
    }

    public bool CollideWith(Cube cube)
    {
        //check if box and cube is collided in X axis

        //check if box and cube is collided in Y axis

        //check if box and cube is collided in Z axis

        //return true if box and cube is collided in all 3 axis
        
        return false;
    }
}

[Serializable]
public struct Cube
{
    public int size;
    public Point position;

    public Vector3 ShapeSize()
    {
        return new Vector3(size, size, size);
    }

    public bool CollideWith(Box box)
    {
        //check if two box is collided in X axis

        //check if two box is collided in Y axis

        //check if two box is collided in Z axis

        //return true if box and cube is collided in all 3 axis
        return false;
    }
}
