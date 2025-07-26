using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    [Header("🔧 Prefabs & References")]
    [SerializeField] private BallController ballPrefab;
    [SerializeField] private TubeController tubePrefab;
    [SerializeField] private Transform tubeParent;

    [Header("📦 Level Data")]
    [SerializeField] private List<LevelData> levelDataList;

    [Header("🧱 Tube Layout")]
    [SerializeField] private Transform tubeLayoutPoints; // Parent with Point0...Point9

    private Transform[] layoutPoints;
    private List<TubeController> tubes = new List<TubeController>();
    public List<TubeController> Tubes => tubes;

    public int LevelCount => levelDataList.Count;

    #region Unity Lifecycle

    private void Awake()
    {
        // ✅ Layout point validate
        if (tubeLayoutPoints == null)
        {
            Debug.LogError("❌ tubeLayoutPoints not assigned!");
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

    #endregion

    public void LoadLevel(int levelIndex)
    {
        #region 🧹 STEP 1: Clear old level

        foreach (var tube in tubes)
        {
            if (tube != null)
                Destroy(tube.gameObject);
        }
        tubes.Clear();
        #endregion

        #region 📦 STEP 2: Load level data
        if (levelIndex < 0 || levelIndex >= levelDataList.Count)
        {
            Debug.LogError($"❌ Invalid level index: {levelIndex}");
            return;
        }

        LevelData level = levelDataList[levelIndex];
        BallColorType[][] tubeColors = level.GetTubes();

        if (tubeColors == null)
        {
            Debug.LogError($"❌ Level {levelIndex} data is NULL.");
            return;
        }
        #endregion

        #region 🧪 STEP 3: Instantiate tubes and balls
        for (int i = 0; i < tubeColors.Length; i++)
        {
            var colors = tubeColors[i];

            if (colors == null)
            {
                Debug.LogWarning($"⚠️ Tube {i} is NULL. Skipping.");
                continue;
            }

            // ✅ Step 1: Instantiate tube
            // ✅ Tube banate hain
            //TubeController tube = Instantiate(tubePrefab, tubeParent);
            TubeController tube = Instantiate(tubePrefab);

            if (tube == null)
            {
                Debug.LogError($"❌ Failed to instantiate tube {i}");
                continue;
            }

            // ✅ Step 2: Set parent only if it's NOT a prefab
            if (tubeParent != null && tubeParent.gameObject.scene.IsValid())
            {
                tube.transform.SetParent(tubeParent, false);
            }
            else
            {
                Debug.LogWarning("⚠️ tubeParent is not a scene object or is null.");
            }

            // ✅ Step 3: Set position using layout or fallback
            if (i < layoutPoints.Length && layoutPoints[i] != null)
            {
                tube.transform.position = layoutPoints[i].position;
            }
            else
            {
                tube.transform.position = new Vector3(i * 2f, 0, 0); // fallback
                Debug.LogWarning($"⚠️ Missing layout for tube {i}, used fallback.");
            }

            // ✅ Step 4: Register
            tubes.Add(tube);


            // ⚪ Ball instantiate
            for (int j = 0; j < colors.Length; j++)
            {
                var color = colors[j];
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

        #region ✅ STEP 4: Register in GameManager
        GameManager.Instance?.SetTubes(tubes);
        Debug.Log($"✅ Level {levelIndex} loaded with {tubes.Count} tubes.");
        #endregion
    }
}
