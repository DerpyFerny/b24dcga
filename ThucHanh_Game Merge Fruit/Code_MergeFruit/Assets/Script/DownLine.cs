using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class DownLine : MonoBehaviour
{
    [SerializeField] private Transform bottomWall; // Gán từ Inspector hoặc tag "BottomWall"
    [SerializeField] private Material dashMaterial;
    [SerializeField]
    private float dashDensity; // càng lớn thì dash càng nhỏ
    [SerializeField]
    private float scrollSpeed; // tốc độ chạy xuống
    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        if (bottomWall == null)
            bottomWall = GameObject.FindWithTag("BottomWall")?.transform;

        // Cấu hình LineRenderer
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.35f;
        lineRenderer.endWidth = 0.35f;
        lineRenderer.useWorldSpace = true;
        lineRenderer.material = dashMaterial;
        lineRenderer.textureMode = LineTextureMode.Tile;
        lineRenderer.numCapVertices = 0;
        lineRenderer.alignment = LineAlignment.View;

        dashDensity = .25f;
        scrollSpeed = .8f;
    }

    void Update()
    {
        if (bottomWall == null) return;

        Vector3 start = transform.position;
        BoxCollider2D col = bottomWall.GetComponent<BoxCollider2D>();

        float wallTopY = bottomWall.position.y + col.offset.y + col.size.y * 0.5f;
        Vector3 end = new Vector3(start.x, wallTopY, start.z);

        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);

        float distance = Vector3.Distance(start, end);

        // Scale ngang để dash nằm dọc
        lineRenderer.material.mainTextureScale = new Vector2(distance * dashDensity, 1);

        // Offset theo X nếu dash nằm ngang
        lineRenderer.material.mainTextureOffset = new Vector2(-Time.time * scrollSpeed, 0);

    }
}
