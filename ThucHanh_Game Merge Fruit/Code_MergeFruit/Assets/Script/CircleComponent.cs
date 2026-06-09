using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class CircleComponent : MonoBehaviour
{
    public bool isCrossLine = false;

    public static event Action<CircleComponent> AddCircleQueueToDestroy;

    public static event Action<CircleComponent, CircleComponent, UnityEngine.Vector3> OnCircleMerged;

    public Action OnUpgrade;

    public static event Action<UnityEngine.Object> AfterUpgrade;

    private Vector3 targetScale;
    Vector3 currentScale = Vector3.zero;

    [SerializeField]
    float explosionRadius;
    [SerializeField]
    float explosionForce;

    bool isDroping = false;

    private Vector2 contactPoint;
    bool isMerging = false;
    [SerializeField] private int level;

    public void SetTargetScale(Vector3 scale)
    {
        targetScale = scale;
    }
    public int Level => level;
    public void SetLevel(int l) => level = l;
    [SerializeField] private AnimalEvolutionTree evolutionTree;
    public void SetEvolutionTree(AnimalEvolutionTree tree) => evolutionTree = tree;

    private Material outlineMaterial;

    private AnimalData data;

    private void Awake()
    {
        OnUpgrade += ChangeToUpgrade;

    }

    private void OnEnable()
    {

    }

    public BallStretch ballStretch;
    Rigidbody _rigidbody;

    private void Start()
    {
        ballStretch = GetComponent<BallStretch>();
        _rigidbody = GetComponent<Rigidbody>();

        PolygonCollider2D col = GetComponent<PolygonCollider2D>();
        if (col != null)
        {
            PhysicsMaterial2D bounceMaterial = Resources.Load<PhysicsMaterial2D>("Physics/BouncyMat");


            if (bounceMaterial != null)
            {
                col.sharedMaterial = bounceMaterial;
            }
            else
            {
                Debug.LogWarning("BouncyMaterial not found in Resources folder!");
            }
        }


        // 1. Kiểm tra nếu evolutionTree đã được gán
        if (evolutionTree == null)
        {
            evolutionTree = GameManager.instance.evolutionTree;
            data = evolutionTree.GetLevelData(Level - 1);

            transform.localScale = Vector3.zero;
            transform.DOScale(targetScale, 0.25f);
            transform.GetComponent<Rigidbody2D>().mass = data.scaleRatio;
            // ApplyFixedOutlineWidth();
        }

    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. Nếu đã đang merge → bỏ qua
        if (isMerging) return;

        // 2. Kiểm tra đối tượng va chạm
        if (!collision.gameObject.TryGetComponent(out CircleComponent otherCircle)) return;

        if(isDroping == false)
        {
            contactPoint = collision.GetContact(0).point;
            Vector3 direction = (transform.position - collision.transform.position);
            collision.transform.GetComponent<BallStretch>().Trigger(collision.transform.worldToLocalMatrix.MultiplyPoint( direction ));
            //
            isDroping = true;
        }

        // 3. Nếu cấp khác nhau hoặc đối tượng kia đang merge → bỏ qua
        if (this.Level != otherCircle.Level || otherCircle.isMerging) return;

        // 4. Đảm bảo chỉ 1 trong 2 xử lý
        if (this.GetInstanceID() < otherCircle.GetInstanceID())
        {
            // 5. Đánh dấu đang merge
            isMerging = true;
            otherCircle.isMerging = true;

            // 6. Xác định vị trí merge (lấy điểm tiếp xúc đầu tiên)
            contactPoint = collision.GetContact(0).point;

            // 7. Gọi hàm merge từ GameManager
            OnCircleMerged?.Invoke(this, otherCircle, contactPoint);
        }
    }


    private void ChangeToUpgrade()
    {
        // if (data.next_circle)
        // {
        //     var next_circle = Instantiate(data.next_circle, gameObject.transform.position, new Quaternion());

        //     AfterUpgrade?.Invoke(next_circle);
        // }
    }

    private void OnexplosionRadiusChanged(string value) => explosionRadius = float.Parse(value);

    private void OnexplosionForceChanged(string value) => explosionForce = float.Parse(value);

    private void ApplyFixedOutlineWidth()
    {
        if (outlineMaterial == null)
        {
            var sr = GetComponent<SpriteRenderer>();
            if (sr == null) return;

            outlineMaterial = sr.material;
        }

        Vector3 scale = transform.lossyScale;
        float avgScale = (scale.x + scale.y) * 0.5f;

        float compensated = GameManager.instance.BaseOutlineWidth / avgScale;

        outlineMaterial.SetFloat("_InnerOutlineWidth", compensated);
    }

}
