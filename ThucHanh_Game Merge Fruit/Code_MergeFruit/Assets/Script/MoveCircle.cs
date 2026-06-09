using System;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;

public class MoveCircle : MonoBehaviour
{
    public static event Action Setup;

    public static event Action SetDropping;

    public bool isDrop { set; private get; } = false;

    private bool isDragging = false;
    public bool isReady = true;

    private float yOffset;
    private Vector3 offset;

    [SerializeField]
    private GameObject Hook;

    private float GameoverY;

    private float EvolutionTreeY;

    void Start()
    {
        GameoverY = GameObject.Find("LineGameOver").transform.position.y;
        EvolutionTreeY = GameObject.Find("EvolutionTree").transform.position.y;
    }
    private void OnEnable()
    {
        CircleComponent.AfterUpgrade += SetupInstiate;

        Hook = GameObject.Find("Hook");
    }


    private void OnMouseDown()
    {
        if (GameManager.MouseState == mouseState.DestroyChoosing && isDrop)
        {
            FinishBosster();

        }
        else if (GameManager.MouseState == mouseState.UpgradeChoosing && isDrop)
        {
            gameObject.GetComponent<CircleComponent>()?.OnUpgrade?.Invoke();

            FinishBosster();

        }
    }

    void Update()
    {
        //
        if (Input.GetMouseButtonDown(0) && !isDrop && GameManager.MouseState == mouseState.notChoosing && isReady)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (mousePos.y > GameoverY || mousePos.y < EvolutionTreeY) return;

            Collider2D hit = Physics2D.OverlapPoint(mousePos);

            isDragging = true;

            // Cho x khớp ngay với vị trí chuột
            offset = new Vector3(0, transform.position.y - mousePos.y, 0);
            transform.position = new Vector3(Mathf.Clamp(mousePos.x, -1.5f, 1.5f), transform.position.y, transform.position.z);
        }


        if (Input.GetMouseButtonUp(0) && isDragging && !isDrop && GameManager.isPausing == false)
        {
            Hook.transform.position = new Vector3(0, Hook.transform.position.y, Hook.transform.position.z);

            gameObject.GetComponent<LineRenderer>().enabled = false;
            isDragging = false;
            GameManager.isPausing = true;
            gameObject.GetComponent<Rigidbody2D>().gravityScale = 2;

            gameObject.transform.SetParent(GameObject.Find("Circles").transform);

            Setup?.Invoke();

            SetDropping?.Invoke();
            GetComponent<MoveCircle>().enabled = false;
        }

        if (isDragging)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;

            // Lấy bán kính collider (tính cả scale)
            float radius = GetComponent<CircleCollider2D>().radius * transform.localScale.x;

            // Clamp vào trong màn hình
            Vector3 clamped = ScreenUtils.ClampInsideScreen(mousePos + offset, radius);

            transform.position = new Vector3(clamped.x, transform.position.y, transform.position.z);

            // Hook đi theo
            Hook.transform.position = new Vector3(clamped.x, Hook.transform.position.y, Hook.transform.position.z);
        }
    }

    

    private void SetupInstiate(UnityEngine.Object circle)
    {
        circle.GetComponent<MoveCircle>().isDrop = true;
        circle.GetComponent<MoveCircle>().isDragging = false;
    }

    private void FinishBosster()
    {
        GameManager.TriggerMouseNotChoosing();

        Destroy(gameObject);
    }
}
