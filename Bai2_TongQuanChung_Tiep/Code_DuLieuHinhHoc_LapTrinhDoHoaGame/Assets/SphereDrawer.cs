using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereDrawer : MonoBehaviour
{
    public Color sphereColor;
    public Color wireColor;
    public Sphere sphere;

    public Color sphereColor2;
    public Color wireColor2;
    public Sphere sphere2;

    private void OnDrawGizmos()
    {
        Gizmos.color = sphereColor;

        if (sphere.CollideWith(sphere2) == true) Gizmos.color = Color.red;

        Gizmos.DrawSphere(sphere.center.ToVec3(), sphere.radius);
        Gizmos.color = wireColor;
        Gizmos.DrawWireSphere(sphere.center.ToVec3(), sphere.radius);

        Gizmos.color = sphereColor2;

        if (sphere.CollideWith(sphere2) == true) Gizmos.color = Color.red; 

        Gizmos.DrawSphere(sphere2.center.ToVec3(), sphere2.radius);
        Gizmos.color = wireColor2;
        Gizmos.DrawWireSphere(sphere2.center.ToVec3(), sphere2.radius);
    }

}

[Serializable]

public struct Sphere
{
    public Point center;
    public int radius;

    public bool CollideWith(Sphere sphere)
    {
        Point distance = center.Subtract(sphere.center);
        distance.Length();
        float sumradius = sphere.radius + radius;
        Debug.Log(distance.Length());
        if (distance.Length() > sumradius) return false;
        return true;
    }

    public void ResetCenter()
    {
        center.x = 0;
        center.y = 0;
        center.z = 0;
    }
}