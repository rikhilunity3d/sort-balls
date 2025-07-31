using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Refs")]
    [SerializeField] private LevelManager levelManager;

    [Header("Stacking")]
    [SerializeField] private float slotSpacingY = 0.55f;  // Must match LevelLoader
    [SerializeField] private float travelArcHeight = 1.5f; // Base arc height for ball travel
    [SerializeField] private float minArcHeight = 1.0f;    // Minimum arc, even for flat transfers
    [SerializeField] private float arcBoostMultiplier = 1.2f; // How much extra arc for low → high jump

    private List<TubeController> allTubes;
    private TubeController selectedTube = null;

    private int totalScore = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SetTubes(List<TubeController> tubes)
    {
        allTubes = tubes;
        selectedTube = null;
    }

    public void ClearTubes()
    {
        if (allTubes != null) allTubes.Clear();
        selectedTube = null;
    }

    public void OnTubeClicked(TubeController clickedTube)
    {
        if (clickedTube == null || allTubes == null) return;

        if (selectedTube == null)
        {
            if (!clickedTube.IsEmpty())
            {
                selectedTube = clickedTube;
                HighlightTube(selectedTube, true);

                float tubeTopY = selectedTube.transform.position.y + selectedTube.GetTubeHeight();
                float liftTargetY = tubeTopY + 0.30f;
                BallController topBall = selectedTube.GetTopBall();
                topBall?.LiftToWorldY(liftTargetY);
            }
            return;
        }

        if (clickedTube == selectedTube)
        {
            BallController topBall = selectedTube.GetTopBall();
            topBall?.ReturnToOriginal();
            HighlightTube(selectedTube, false);
            selectedTube = null;
            return;
        }

        BallController movingBall = selectedTube.GetTopBall();
        if (movingBall != null && clickedTube.CanReceiveBall(movingBall))
        {
            selectedTube.RemoveTopBall();
            clickedTube.AddBall(movingBall);

            movingBall.transform.SetParent(clickedTube.transform, true);

            int targetIndex = clickedTube.GetBallCount() - 1;
            Vector3 localTarget = new Vector3(0f, slotSpacingY * targetIndex, 0f);

            Vector3 startPos = movingBall.transform.position;
            Vector3 endPos = clickedTube.transform.TransformPoint(localTarget);

            // Dynamically calculate arc height based on Y difference
            float verticalDelta = endPos.y - startPos.y;
            float dynamicArcHeight = Mathf.Max(minArcHeight, travelArcHeight + Mathf.Max(0f, verticalDelta * arcBoostMultiplier));

            float midX = (startPos.x + endPos.x) / 2f;
            float arcY = Mathf.Max(startPos.y, endPos.y) + dynamicArcHeight;
            Vector3 midPos = new Vector3(midX, arcY, startPos.z);

            movingBall.transform.DOKill();
            movingBall.transform.DOPath(new Vector3[] { startPos, midPos, endPos }, 0.5f, PathType.CatmullRom)
                .SetEase(Ease.InOutSine)
                .OnComplete(() =>
                {
                    movingBall.transform.localPosition = localTarget;
                });

            CheckWinCondition();
        }
        else
        {
            movingBall?.ReturnToOriginal();
        }

        HighlightTube(selectedTube, false);
        selectedTube = null;
    }

    private void HighlightTube(TubeController tube, bool highlight)
    {
        if (tube == null) return;
        var sr = tube.GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.color = highlight ? Color.yellow : Color.white;
    }

    private void CheckWinCondition()
    {
        if (allTubes == null) return;

        foreach (var tube in allTubes)
        {
            if (tube == null) continue;

            int count = tube.GetBallCount();
            if (count == 0) continue;
            if (count != tube.GetCapacity()) return;

            BallController top = tube.GetTopBall();
            if (top == null) return;
            BallColorType color = top.GetColor();

            for (int i = 0; i < count; i++)
            {
                var b = tube.GetBallAtIndex(i);
                if (b == null || b.GetColor() != color)
                    return;
            }
        }

        Debug.Log("✅ Level Completed!");
        Invoke(nameof(HandleLevelComplete), 0.5f);
    }

    private void HandleLevelComplete()
    {
        if (levelManager != null)
        {
            totalScore += levelManager.GetCurrentLevelScore();
            Debug.Log($"🏆 Total Score: {totalScore}");
        }

        if (allTubes != null)
        {
            foreach (var tube in allTubes)
                if (tube != null) Destroy(tube.gameObject);
            allTubes.Clear();
        }

        levelManager?.LoadNextLevel();
    }
}
