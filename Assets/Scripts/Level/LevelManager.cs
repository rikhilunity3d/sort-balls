using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private LevelLoader levelLoader;

    private int currentLevelIndex = 0;

    private void Start()
    {
        LoadLevel(currentLevelIndex);
    }

    public void LoadLevel(int index)
    {
        currentLevelIndex = index;
        levelLoader.LoadLevel(currentLevelIndex);
    }

    public void LoadNextLevel()
{
    int nextIndex = (currentLevelIndex + 1) % levelLoader.LevelCount;
    LoadLevel(nextIndex);
}
}
