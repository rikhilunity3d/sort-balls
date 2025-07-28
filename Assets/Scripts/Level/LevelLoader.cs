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

    private Transform[] layoutPoints;
    private List<TubeController> tubes = new List<TubeController>();
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
        #region 🧹 Step 1: Clear Old Tubes
        foreach (var tube in tubes)
        {
            if (tube != null)
                Destroy(tube.gameObject);
        }
        tubes.Clear();
        #endregion

        #region 📦 Step 2: Load LevelData
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
        #endregion

        #region 🧪 Step 3: Create Tubes and Balls
        for (int i = 0; i < tubeColors.Length; i++)
        {
            var colors = tubeColors[i];

            if (colors == null)
            {
                Debug.LogWarning($"⚠️ Tube {i} is NULL. Skipping.");
                continue;
            }

            // Instantiate tube without parent (avoid prefab parenting bug)
            TubeController tube = Instantiate(tubePrefab);

            if (tube == null)
            {
                Debug.LogError($"❌ Failed to instantiate tube {i}");
                continue;
            }

            // ✅ Set parent if valid scene object
            if (tubeParent != null && tubeParent.gameObject.scene.IsValid())
            {
                tube.transform.SetParent(tubeParent, false);
            }
            else
            {
                Debug.LogWarning("⚠️ tubeParent is not valid or not assigned.");
            }

            // ✅ Use layout point or fallback
            if (i < layoutPoints.Length && layoutPoints[i] != null)
            {
                tube.transform.position = layoutPoints[i].position;
            }
            else
            {
                tube.transform.position = new Vector3(i * 2f, 0, 0);
                Debug.LogWarning($"⚠️ Missing layout for tube {i}, fallback used.");
            }

            tubes.Add(tube);

            // 🎨 Spawn balls
            for (int j = 0; j < colors.Length; j++)
            {
                BallColorType color = colors[j];
                if (color == BallColorType.None) continue; //Skip empty slots

                BallController ball = Instantiate(ballPrefab, tube.transform);
                
                if (ball == null)
                {
                    Debug.LogError($"❌ Failed to instantiate ball {j} in tube {i}");
                    continue;
                }

                ball.SetColor(color);

                Vector3 pos = tube.transform.position + new Vector3(0, 0.55f * j, 0);
                ball.transform.position = pos;

                tube.AddBall(ball);
            }
        }
        #endregion

        #region ✅ Step 4: Register Tubes in GameManager
        GameManager.Instance?.SetTubes(tubes);
        Debug.Log($"✅ Level {levelIndex} loaded with {tubes.Count} tubes.");
        #endregion
    }

    //
    public int GetCurrentLevelScore()
    {
        if (currentLevelIndex < 0 || currentLevelIndex >= levelDataList.Count)
            return 0;

        return levelDataList[currentLevelIndex].scoreReward;
    }

}
