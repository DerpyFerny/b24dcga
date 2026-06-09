using UnityEngine;

public class ShakeObject : MonoBehaviour
{
    [SerializeField]
    private float duration = 0.5f;

    [SerializeField]
    private float magnitude = 0.8f; 
    
    private float rotationMagnitude = 5f;

    private Vector3 originalPos;
    private Quaternion originalRot;
    private float elapsed = 0f;
    private bool isShaking = false;

    private void OnEnable()
    {
        Booster.booster4 += StartShaking;
    }

    private void OnDisable()
    {
        Booster.booster4 -= StartShaking;
    }

    public void StartShaking()
    {
        if (!isShaking)
        {
            originalPos = transform.localPosition;
            originalRot = transform.localRotation;
            elapsed = 0f;
            isShaking = true;
        }
    }

    void Update()
    {
        if (isShaking)
        {
            if (elapsed < duration)
            {
                float x = Random.Range(-1f, 1f) * magnitude;
                float y = Random.Range(-1f, 1f) * magnitude;

                float zRot = Mathf.Sin(Time.time * 50f) * rotationMagnitude;

                transform.localPosition = originalPos + new Vector3(x, y, 0f);
                transform.localRotation = Quaternion.Euler(0, 0, zRot);


                elapsed += Time.deltaTime;
            }
            else
            {
                isShaking = false;
                transform.localPosition = originalPos;
                transform.localRotation = originalRot;
            }
        }
    }
}
