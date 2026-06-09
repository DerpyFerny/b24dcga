using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallStretch : MonoBehaviour
{
    public Material stretchMat;
    SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.material = new Material(stretchMat);
    }

    public void Trigger(Vector3 contactDirection)
    {
        //need to completed

    }   
    
    IEnumerator PlayStretchAnim(float duration = 1f)
    {
        //need to completed

   

        spriteRenderer.material.SetFloat("_stretch", 0f);
        yield return null;
    }    
}
