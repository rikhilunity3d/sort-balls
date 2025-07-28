using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "ColorSort/Level Data", order = 1)]
public class LevelData : ScriptableObject
{
    [Header("🧩 Difficulty Settings")]
    public LevelDifficulty difficulty;

    [Tooltip("Score awarded on completing this level.")]
    public int scoreReward = 50;

    [Header("🔢 Tube Counts")]
    [Tooltip("How many tubes should be filled with colored balls.")]
    public int filledTubeCount = 3;

    [Tooltip("How many tubes should be empty at start.")]
    public int emptyTubeCount = 2;

    [Header("🧪 Tube Setup")]
    [Tooltip("Assign only filled tubes here. Empty tubes will be added automatically.")]
    public List<TubeData> tubes;

    /// <summary>
    /// Returns full list of tubes including auto-added empty tubes.
    /// </summary>
    public BallColorType[][] GetTubes()
    {
        List<BallColorType[]> result = new List<BallColorType[]>();

        for (int i = 0; i < filledTubeCount; i++)
        {
            if (i < tubes.Count && tubes[i] != null)
                result.Add(tubes[i].balls);
            else
                result.Add(new BallColorType[0]); // fallback for missing tube
        }

        for (int i = 0; i < emptyTubeCount; i++)
        {
            result.Add(new BallColorType[0]); // empty tubes
        }

        return result.ToArray();
    }

    /// <summary>
    /// Total tubes = filled + empty
    /// </summary>
    public int GetTotalTubes() => filledTubeCount + emptyTubeCount;

    /// <summary>
    /// Returns how many tubes are filled.
    /// </summary>
    public int GetFilledTubeCount() => filledTubeCount;

    /// <summary>
    /// Returns how many tubes are empty.
    /// </summary>
    public int GetEmptyTubeCount() => emptyTubeCount;
}
