using UnityEngine;

public class HandleHook : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static HandleHook instance;
    public GameObject droppingImg;
    public GameObject draggingImg;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else Destroy(this);
    }

    void OnEnable()
    {
        GameManager.SetDragging += SetDragging;
        MoveCircle.SetDropping += SetDropping;
    }

    void OnDisable()
    {
        GameManager.SetDragging -= SetDragging;
        MoveCircle.SetDropping -= SetDropping;
    }

    private void SetDragging()
    {
        draggingImg.SetActive(true);
        droppingImg.SetActive(false);
    }
    private void SetDropping()
    {
        draggingImg.SetActive(false);
        droppingImg.SetActive(true);
    }
}
