using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public enum mouseState
{
    notChoosing,
    DestroyChoosing,
    UpgradeChoosing,
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static event Action MouseNotChoosing;

    public static event Action SetDragging;

    public static mouseState MouseState { get; private set; } = mouseState.notChoosing;

    [SerializeField]
    private UnityEngine.Object[] Circles;

    private AnimalData nextCircleAData;
    private AnimalData draggingCircleAData;

    private GameObject nextCircleGO;
    private GameObject draggingCircleGO;

    [SerializeField]
    private Transform nextCirclePos;

    private bool hasrun = false;
    public static bool isPausing = false;

    private List<CircleComponent> waitingColliders = new List<CircleComponent>();

    private Vector3 OriginScale;
    [SerializeField] public AnimalEvolutionTree evolutionTree;
    [SerializeField] private float baseOutlineWidth = 0.03f;
    public float BaseOutlineWidth => baseOutlineWidth;

    private int Scores = 0;

    private int HighScore;

    [SerializeField]
    private TextMeshProUGUI ScoreText;

    [SerializeField]
    private TextMeshProUGUI HighScoreText;

    [SerializeField]
    private TextMeshProUGUI YourScoreText;

    [SerializeField]
    private GameObject GameOverObj;

    [SerializeField]
    private GameObject TopCenter;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else Destroy(this);

        Application.targetFrameRate = 60;

        if (evolutionTree == null)
        {
            evolutionTree = Resources.Load<AnimalEvolutionTree>("AnimalEvolutionTreeData");
            if (evolutionTree == null)
            {
                Debug.LogError("AnimalEvolutionTree not found!");
            }
        }
    }

    private void OnEnable()
    {
        MoveCircle.Setup += DelaySpawnCircles;
        Booster.boosTer1 += Destroy_Smallest;
        Booster.booster2 += ChangeDestroyMouseState;
        Booster.booster3 += ChangeUpgradeMouseState;
        MouseNotChoosing += ChangeNotChoosingMouseState;
        CircleComponent.AddCircleQueueToDestroy += ReportCollision;
        CircleComponent.OnCircleMerged += MergeCircles;
        GameOverLine.GameOVer += GameOver;
    }

    private void OnDisable()
    {
        MoveCircle.Setup -= DelaySpawnCircles;
        Booster.boosTer1 -= Destroy_Smallest;
        Booster.booster2 -= ChangeDestroyMouseState;
        Booster.booster3 -= ChangeUpgradeMouseState;
        MouseNotChoosing -= ChangeNotChoosingMouseState;
        CircleComponent.AddCircleQueueToDestroy -= ReportCollision;
        CircleComponent.OnCircleMerged -= MergeCircles;
        GameOverLine.GameOVer -= GameOver;
    }

    void Start()
    {
        MouseState = mouseState.notChoosing;
        GameInit();
        Application.targetFrameRate = 60;

        HighScore = PlayerPrefs.GetInt("highscore", 0);
        HighScoreText.SetText(HighScore.ToString());
    }

    private void GameInit() // Tạo 2 circle khi bắt đầu game
    {
        AnimalData data = evolutionTree.GetLevelData();
        if (data != null && data.prefab != null)
        {
            SpawnDraggingCircle(data); // setup dragging circle
            SpawnNextCircle(); // setup next circle
        }
    }

    private void GameOver()
    {
        if (HighScore < Scores) PlayerPrefs.SetInt("highscore", Scores);

        TopCenter.SetActive(false);
        GameOverObj.SetActive(true);
        YourScoreText.SetText("Score: " + Scores);
        Destroy(nextCircleGO);
    }

    private void HandleSpawnCircles()
    {
        SpawnDraggingCircle(nextCircleAData);
        Destroy(nextCircleGO);
        SpawnNextCircle();
        isPausing = false;
        SetDragging?.Invoke();
    }

    private void SpawnDraggingCircle(AnimalData data)
    {
        if (data == null || data.prefab == null) return;

        var hookPos = GameObject.Find("Hook").transform.position;
        var spawnPos = new Vector3(hookPos.x, hookPos.y - 0.3f, 0f);
        draggingCircleGO = Instantiate(data.prefab, spawnPos, Quaternion.identity);
        draggingCircleGO.GetComponent<CircleComponent>().SetTargetScale(data.prefab.transform.localScale * data.scaleRatio);

        draggingCircleGO.GetComponent<Rigidbody2D>().gravityScale = 0;
        draggingCircleGO.GetComponent<MoveCircle>().isReady = true;
        draggingCircleGO.GetComponent<LineRenderer>().enabled = true;

        // Khoảng có thể chuyển của Circle
        float radius = draggingCircleGO.GetComponent<CircleCollider2D>().radius * draggingCircleGO.transform.localScale.x;
        Vector3 clampedPos = ScreenUtils.ClampInsideScreen(spawnPos, radius);
        draggingCircleGO.transform.position = clampedPos;
    }

    private void SpawnNextCircle()
    {
        nextCircleAData = evolutionTree.GetLevelData();
        if (nextCircleAData != null && nextCircleAData.prefab != null)
        {
            Vector3 spawnPos = new Vector3(nextCirclePos.position.x, nextCirclePos.position.y, 0f);
            nextCircleGO = Instantiate(nextCircleAData.prefab, spawnPos, Quaternion.identity);
            nextCircleGO.GetComponent<CircleComponent>().SetTargetScale(new Vector3(0.1f, 0.1f, 0.1f));
            nextCircleGO.GetComponent<Rigidbody2D>().gravityScale = 0;
            nextCircleGO.GetComponent<MoveCircle>().isReady = false;
            nextCircleGO.GetComponent<LineRenderer>().enabled = false;
        }
    }

    public GameObject SpawnAnimalAtLevel(int level, Vector3 position)
    {
        AnimalData data = evolutionTree.GetLevelData(level - 1);
        if (data == null) return null;

        GameObject obj = Instantiate(data.prefab, position, Quaternion.identity);
        // obj.transform.localScale = data.prefab.transform.localScale * data.scaleRatio;
        obj.GetComponent<CircleComponent>().SetTargetScale(data.prefab.transform.localScale * data.scaleRatio);

        return obj;
    }
    private void MergeCircles(CircleComponent c1, CircleComponent c2, Vector3 spawnPos)
    {
        // Hiệu ứng GlowBurst
        GameObject glowFx = Resources.Load<GameObject>("MergeEffect");
        if (glowFx != null)
        {
            GameObject vfx1 = Instantiate(glowFx, spawnPos, Quaternion.identity);
            Destroy(vfx1, 2f);
        }

        // Hiệu ứng SparkleBurst
        GameObject sparkleFx = Resources.Load<GameObject>("MergeEffect1");
        if (sparkleFx != null)
        {
            GameObject vfx2 = Instantiate(sparkleFx, spawnPos, Quaternion.identity);
            Destroy(vfx2, 2f);
        }

        // Spawn con vật cấp tiếp theo
        int nextLevel = c1.Level + 1;
        if (nextLevel <= evolutionTree.GetMaxLevel() + 1)  //vẫn trong mảng circle có thể next được
        {
            GameObject newObj = SpawnAnimalAtLevel(nextLevel, spawnPos);
            if (newObj != null)
            {
                // Gán parent nếu cần
                newObj.transform.SetParent(GameObject.Find("Circles").transform);

                // Vô hiệu hóa LineRenderer nếu có
                LineRenderer lr = newObj.GetComponent<LineRenderer>();
                if (lr != null) lr.enabled = false;

                // Tùy chọn: Tạm tắt điều khiển (nếu cần delay)
                MoveCircle mv = newObj.GetComponent<MoveCircle>();
                if (mv != null) mv.enabled = false;
            }
        }

        //set scores
        SetScore(nextLevel);

        // Add force nổ
        Vector2 explosionPos = spawnPos; // ví dụ: transform.position
        float radius = 1f;
        float force = 3f;
        Explode(explosionPos, radius, force);

        // Huỷ 2 object cũ
        Destroy(c1.gameObject);
        Destroy(c2.gameObject);
    }

    private void SetScore(int score)
    {
        Scores += score;
        ScoreText.SetText(Scores.ToString());
    }


    private UnityEngine.Object Find_Smallest_Fruit()
    {
        Transform parent = GameObject.Find("Circles").transform;
        int min_index = 100;
        UnityEngine.Object smallest = null;

        foreach (var circle in parent)
        {
            string name = (circle as Transform).gameObject.name.Replace("(Clone)", "");

            for (int i = 0; i < Circles.Length; i++)
            {
                if (Circles[i].name == name)
                {
                    if (i <= min_index)
                    {
                        min_index = i;
                        smallest = (circle as Transform).gameObject;
                    }
                }
            }
        }
        return smallest;
    }

    private void Destroy_Smallest()
    {
        if (!Find_Smallest_Fruit().GameObject()) return;

        Destroy(Find_Smallest_Fruit().GameObject());
    }

    private void ReportCollision(CircleComponent circle)
    {
        if (!waitingColliders.Contains(circle))
        {
            waitingColliders.Add(circle);
        }

        if (waitingColliders.Count >= 3)
        {
            HandleThreeCollisions();
            return;
        }

        Invoke("ClearwaitingColliders", 0.00001f);
    }

    private void HandleThreeCollisions()
    {
        // Lấy 2 con đầu để "biến mất"
        for (int i = 0; i < 2; i++)
        {
            Destroy(waitingColliders[i].gameObject);
        }

        Debug.Log("2 đối tượng đã bị huỷ, giữ lại: " + waitingColliders[2].name);

        // Reset danh sách
        waitingColliders.Clear();
    }

    private void ClearwaitingColliders()
    {
        for (int i = 0; i < waitingColliders.Count; i++)
        {
            Destroy(waitingColliders[i].gameObject);
        }
        waitingColliders.Clear();
    }

    public void Explode(UnityEngine.Vector2 center, float radius, float explosionForce)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, radius);

        foreach (Collider2D hit in hits)
        {
            if (hit.attachedRigidbody != null && hit.gameObject != this.gameObject)
            {
                Rigidbody2D rb = hit.attachedRigidbody;

                // Tính hướng từ tâm nổ ra đối tượng
                Vector2 direction = (hit.transform.position - (Vector3)center).normalized;

                // Áp lực tỷ lệ ngược với khoảng cách (tùy chỉnh nếu muốn)
                float distance = Vector2.Distance(center, hit.transform.position);
                float distanceFactor = Mathf.Clamp01(1 - distance / radius);

                // Thêm force
                rb.AddForce(direction * explosionForce * distanceFactor, ForceMode2D.Impulse);
            }
        }
    }

    void ResetFlag() => hasrun = false;

    public static void TriggerMouseNotChoosing() => MouseNotChoosing?.Invoke();
    private void ChangeNotChoosingMouseState() => MouseState = mouseState.notChoosing;
    private void ChangeDestroyMouseState() => MouseState = mouseState.DestroyChoosing;

    private void DelaySpawnCircles() => Invoke("HandleSpawnCircles", .5f);

    private void ChangeUpgradeMouseState() => MouseState = mouseState.UpgradeChoosing;
}
