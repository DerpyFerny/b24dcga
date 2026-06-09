// using UnityEngine;

// /// <summary>
// /// Tự động tạo 3 collider vật lý (trái, phải, dưới) theo UI box, chỉ chạy 1 lần khi Start.
// /// </summary>
// [RequireComponent(typeof(Rigidbody2D))]
// public class AutoCreateUIColliderWalls : MonoBehaviour
// {
//     [Header("Target UI Box")]
//     public RectTransform uiTarget;

//     [Header("Wall Settings")]
//     public float thickness = 1f;
//     public string wallTag = "Wall";
//     public string wallLayer = "Default";

//     void Start()
//     {
//         if (uiTarget == null)
//         {
//             Debug.LogError("AutoCreateUIColliderWalls: RectTransform target chưa được gán.");
//             return;
//         }

//         // Cấu hình Rigidbody2D
//         Rigidbody2D rb = GetComponent<Rigidbody2D>();
//         rb.bodyType = RigidbodyType2D.Kinematic;
//         rb.gravityScale = 0;
//         rb.simulated = true;

//         // Lấy tọa độ 4 góc (world space) của UI box
//         Vector3[] corners = new Vector3[4];
//         uiTarget.GetWorldCorners(corners);

//         Vector2 bottomLeft = corners[0];
//         Vector2 topRight = corners[2];
//         float width = topRight.x - bottomLeft.x;
//         float height = topRight.y - bottomLeft.y;

//         // Tạo các collider cạnh
//         CreateCollider("LeftWall", new Vector2(thickness, height), new Vector2(bottomLeft.x + thickness / 2, bottomLeft.y + height / 2));
//         CreateCollider("RightWall", new Vector2(thickness, height), new Vector2(topRight.x - thickness / 2, bottomLeft.y + height / 2));
//         CreateCollider("BottomWall", new Vector2(width, thickness), new Vector2(bottomLeft.x + width / 2, bottomLeft.y + thickness / 2));
//     }

//     void CreateCollider(string name, Vector2 size, Vector2 position)
//     {
//         GameObject wall = new GameObject(name);
//         wall.transform.SetParent(this.transform);
//         wall.transform.position = position;

//         BoxCollider2D col = wall.AddComponent<BoxCollider2D>();
//         col.size = size;
//         col.isTrigger = false;

//         wall.tag = wallTag;
//         wall.layer = LayerMask.NameToLayer(wallLayer);
//     }
// }

using UnityEngine;

/// <summary>
/// Tự động tạo 3 collider vật lý (trái, phải, dưới) bao bên ngoài UI box, chỉ chạy 1 lần khi Start.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class AutoCreateUIColliderWalls : MonoBehaviour
{
    [Header("Target UI Box")]
    public RectTransform uiTarget;

    [Header("Wall Settings")]
    public float thickness = 1f;
    public string wallTag = "Wall";
    public string wallLayer = "Default";

    [SerializeField]
    private PhysicsMaterial2D wallMaterial; // Gán từ Inspector nếu cần

    void Start()
    {
        if (uiTarget == null)
        {
            Debug.LogError("AutoCreateUIColliderWalls: RectTransform target chưa được gán.");
            return;
        }

        // Cấu hình Rigidbody2D
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0;
        rb.simulated = true;

        // Lấy tọa độ 4 góc (world space) của UI box
        Vector3[] corners = new Vector3[4];
        uiTarget.GetWorldCorners(corners);

        Vector2 bottomLeft = corners[0];
        Vector2 topRight = corners[2];
        float width = topRight.x - bottomLeft.x;
        float height = topRight.y - bottomLeft.y;
        float centerY = bottomLeft.y + height / 2;

        // OUTER walls — dịch ra ngoài
        CreateCollider("LeftWall", new Vector2(thickness, height + thickness * 2), new Vector2(bottomLeft.x - thickness / 2f, centerY));
        CreateCollider("RightWall", new Vector2(thickness, height + thickness * 2), new Vector2(topRight.x + thickness / 2f, centerY));
        CreateCollider("BottomWall", new Vector2(width + thickness * 2, thickness), new Vector2(bottomLeft.x + width / 2f, bottomLeft.y - thickness / 2f));
    }

    void CreateCollider(string name, Vector2 size, Vector2 position)
    {
        GameObject wall = new GameObject(name);
        wall.transform.SetParent(this.transform);
        wall.transform.position = position;

        BoxCollider2D col = wall.AddComponent<BoxCollider2D>();
        col.size = size;
        col.isTrigger = false;
        if (wallMaterial != null)
        {
            col.sharedMaterial = wallMaterial;
        }

        wall.layer = LayerMask.NameToLayer(wallLayer);

        // Gán tag riêng cho BottomWall, còn lại dùng wallTag chung
        if (name == "BottomWall")
        {
            wall.tag = "BottomWall"; // ⚠ đảm bảo bạn đã tạo tag này trong Unity Editor
        }
        else
        {
            wall.tag = wallTag;
        }
    }
}
