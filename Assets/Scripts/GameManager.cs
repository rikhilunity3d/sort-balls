using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Refs")]
    [SerializeField] private LevelManager levelManager;

    [Header("Stacking & Travel")]
    [SerializeField] private float slotSpacingY = 0.55f; // Match LevelLoader
    [SerializeField] private float travelArcHeight = 1.5f; // Height for arc
    [SerializeField] private float sideOffsetX = 1.5f; // Sideway detour distance

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

            // Calculate arc jump based on relative heights
Vector3 startPos = movingBall.transform.position;
Vector3 endPos = clickedTube.transform.TransformPoint(localTarget);

// Adjust arc height if target tube is higher
float dynamicArcY = Mathf.Max(startPos.y, endPos.y) + travelArcHeight;

// Lift up from current tube
Vector3 liftPos = new Vector3(startPos.x, dynamicArcY, startPos.z);

// Side detour to avoid straight line
float direction = Mathf.Sign(endPos.x - startPos.x);
Vector3 sideArcPos = new Vector3(startPos.x + direction * sideOffsetX, dynamicArcY + 0.5f, startPos.z);

// Move directly above target
Vector3 aboveTarget = new Vector3(endPos.x, dynamicArcY + 0.5f, startPos.z);

// Construct smooth path
Vector3[] path = new Vector3[]
{
    startPos,
    liftPos,
    sideArcPos,
    aboveTarget,
    endPos
};

movingBall.transform.DOKill();
movingBall.transform.DOPath(path, 0.6f, PathType.CatmullRom)
    .SetEase(Ease.InOutSine)
    .OnComplete(() =>
    {
        // Final snap for stacking
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
