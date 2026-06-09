using Answer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPointLightAnswer : MonoBehaviour
{
    public Color lightColor;
    
    public MyRay GetLightRay(Vector3 destination)
    {
        Vector3 origin = transform.position;
        Vector3 direction = (origin - destination).normalized;
        return new MyRay(origin, direction);
    } 
    
    public MyRay SampleShadowRay(Vector3 destination)
    {
        Vector3 origin = transform.position;
        Vector3 direction = (destination - origin).normalized;

        //choose ref vector
        Vector3 referenceVector = Vector3.up;
        if (Mathf.Approximately(Vector3.Dot(direction, Vector3.up), 1f) || Mathf.Approximately(Vector3.Dot(direction, Vector3.up), -1f))
        {
            referenceVector = Vector3.forward; 
        }

        Vector3 vectorUp = Vector3.Cross(direction, referenceVector).normalized;
        Vector3 vectorRight = Vector3.Cross(direction, vectorUp).normalized;

        origin = transform.position + vectorUp * Random.Range(0, 1f) + vectorRight * Random.Range(0, 1f);
        direction = (destination - origin).normalized;

        return new MyRay(origin, direction);
    }    
}
