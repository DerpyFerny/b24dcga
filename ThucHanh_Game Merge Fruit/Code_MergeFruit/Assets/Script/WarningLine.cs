using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class WarningLine : MonoBehaviour
{
    [SerializeField]
    private GameObject GameOverLine;

    private float time;
    private float TargetTime = 2.0f;

    private bool onWarning = false;

    private void OnTriggerExit2D(Collider2D collision)
    {
        GameOverLine.SetActive(false);
        onWarning = false;
        time = 0.0f;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        onWarning = true;
        if(collision.GetComponent<CircleComponent>().isCrossLine)
        {
            GameOverLine.SetActive(true);
        }
    }

    private void Update()
    {
        if(onWarning)
        {
            time += math.clamp(Time.deltaTime, 0, TargetTime);
            if (time >= TargetTime && !GameManager.isPausing)
            {
                GameOverLine.SetActive(true);
            }
        }
    }
}
