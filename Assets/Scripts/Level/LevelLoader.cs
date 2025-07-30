using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    [Header("🔧 Prefabs & References")]
    [SerializeField] private BallController ballPrefab;
    [SerializeField] private TubeController tubePrefab;
    [SerializeField] private Transform tubeParent;

    [Header("📦 Level Data List")]
    [SerializeField] private List<LevelData> levelDataList;

    [Header("🧱 Tube Layout")]
    [SerializeField] private Transform tubeLayoutPoints; // Parent with Point0...Point9

    [Header("🎯 Stacking Settings")]
    [SerializeField] private float slotSpacingY = 0.55f; // distance between stacked balls (local units)

    private Transform[] layoutPoints;
    private readonly List<TubeController> tubes = new List<TubeController>();
    public List<TubeController> Tubes => tubes;
    public int LevelCount => levelDataList.Count;

    private int currentLevelIndex = 0;

    private void Awake()
    {
        if (tubeLayoutPoints == null)
        {
            Debug.LogError("❌ tubeLayoutPoints is not assigned!");
            return;
        }

        int count = tubeLayoutPoints.childCount;
        layoutPoints = new Transform[count];

        for (int i = 0; i < count; i++)
        {
            layoutPoints[i] = tubeLayoutPoints.GetChild(i);
            if (layoutPoints[i] == null)
                Debug.LogWarning($"⚠️ Layout point {i} is NULL!");
        }

        Debug.Log($"📍 Loaded {layoutPoints.Length} layout points.");
    }

    public void LoadLevel(int levelIndex)
    {
        // 🧹 Step 1: Clear old tubes
        foreach (var tube in tubes)
        {
            if (tube != null)
                Destroy(tube.gameObject);
        }
        tubes.Clear();

        // 📦 Step 2: Validate and fetch data
        if (levelIndex < 0 || levelIndex >= levelDataList.Count)
        {
            Debug.LogError($"❌ Invalid level index: {levelIndex}");
            return;
        }

        LevelData level = levelDataList[levelIndex];
        BallColorType[][] tubeColors = level.GetTubes();
        currentLevelIndex = levelIndex;

        if (tubeColors == null)
        {
            Debug.LogError($"❌ Level {levelIndex} data is NULL.");
            return;
        }

        Debug.Log($"🧠 Difficulty: {level.difficulty}, Filled: {level.GetFilledTubeCount()}, Empty: {level.GetEmptyTubeCount()}, Score: {level.scoreReward}");

        // 🧪 Step 3: Create tubes and balls
        for (int i = 0; i < tubeColors.Length; i++)
        {
            var colors = tubeColors[i];

            // Instantiate tube (no parent to avoid prefab parenting warning, then set)
            TubeController tube = Instantiate(tubePrefab);
            if (tubeParent != null && tubeParent.gameObject.scene.IsValid())
                tube.transform.SetParent(tubeParent, false);

            // Position tube via layout or fallback
            if (i < layoutPoints.Length && layoutPoints[i] != null)
                tube.transform.position = layoutPoints[i].position;
            else
                tube.transform.position = new Vector3(i * 2f, 0f, 0f);

            tubes.Add(tube);

            // Spawn balls using LOCAL coordinates, skipping None
            if (colors == null || colors.Length == 0)
                continue;

            for (int j = 0; j < colors.Length; j++)
            {
                BallColorType color = colors[j];
                if (color == BallColorType.None) continue; // skip empty slots

                // The next visual slot is the current count
                int visualIndex = tube.GetBallCount();

                BallController ball = Instantiate(ballPrefab, tube.transform);
                ball.SetColor(color);

                // ✅ Local-positioned stacking (origin = tube bottom pivot)
                ball.transform.localPosition = new Vector3(0f, slotSpacingY * visualIndex, 0f);

                tube.AddBall(ball);
            }
        }

        // ✅ Step 4: Register in GameManager
        GameManager.Instance?.SetTubes(tubes);
        Debug.Log($"✅ Level {levelIndex} loaded with {tubes.Count} tubes.");
    }

    public int GetCurrentLevelScore()
    {
        if (currentLevelIndex < 0 || currentLevelIndex >= levelDataList.Count)
            return 0;

        return levelDataList[currentLevelIndex].scoreReward;
    }
}
