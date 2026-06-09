using System;
using Unity.Mathematics;
using UnityEngine;

public class GameOverLine : MonoBehaviour
{
    public static event Action GameOVer;

    private float time;
    private float TargetTime = 2.0f;
    private bool isOn = false;

    private void OnTriggerStay2D(Collider2D collision)
    {  
        if (collision.gameObject.GetComponent<CircleComponent>())
        {
            time = 0;
            isOn = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) => isOn = false;

    private void Update()
    {
        if(isOn)
        {
            time += math.clamp(Time.deltaTime, 0, TargetTime);
            if(time >= TargetTime && !GameManager.isPausing)
            {
                GameOVer?.Invoke();
            }
        }
    }
}
