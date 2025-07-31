using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private LevelManager levelManager;

    [Header("Ball Movement Settings")]
    [SerializeField] private float slotSpacingY = 0.55f;        // Vertical ball spacing
    [SerializeField] private float travelArcHeight = 1.5f;      // Arc height between tubes
    [SerializeField] private float sideOffsetX = 1.5f;          // Side detour to avoid tube overlaps

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

        // First click – select tube
        if (selectedTube == null)
        {
            if (!clickedTube.IsEmpty())
            {
                selectedTube = clickedTube;
                HighlightTube(selectedTube, true);

                float liftY = selectedTube.transform.position.y + selectedTube.GetTubeHeight() + 0.3f;
                selectedTube.GetTopBall()?.LiftToWorldY(liftY);
            }
            return;
        }

        // Clicked again → cancel selection
        if (clickedTube == selectedTube)
        {
            selectedTube.GetTopBall()?.ReturnToOriginal();
            HighlightTube(selectedTube, false);
            selectedTube = null;
            return;
        }

        // Attempt move
        BallController movingBall = selectedTube.GetTopBall();

        if (movingBall != null && clickedTube.CanReceiveBall(movingBall))
        {
            // Update tube data
            selectedTube.RemoveTopBall();
            clickedTube.AddBall(movingBall);

            // Set parent for correct positioning
            movingBall.transform.SetParent(clickedTube.transform, true);

            int targetIndex = clickedTube.GetBallCount() - 1;
            Vector3 localTarget = new Vector3(0f, slotSpacingY * targetIndex, 0f);

            // Build travel path
            Vector3 startPos = movingBall.transform.position;
            Vector3 endPos = clickedTube.transform.TransformPoint(localTarget);

            float heightDelta = endPos.y - startPos.y;
            float dynamicArcY = Mathf.Max(startPos.y, endPos.y) + travelArcHeight + Mathf.Max(0, heightDelta * 0.6f);

            float direction = Mathf.Sign(endPos.x - startPos.x);

            Vector3 liftPos     = new Vector3(startPos.x, dynamicArcY, startPos.z);
            Vector3 sideArcPos  = new Vector3(startPos.x + direction * sideOffsetX, dynamicArcY + 0.5f, startPos.z);
            Vector3 aboveTarget = new Vector3(endPos.x, dynamicArcY + 0.5f, startPos.z);

            Vector3[] path = new Vector3[]
            {
                startPos,
                liftPos,
                sideArcPos,
                aboveTarget,
                endPos
            };

            movingBall.transform.DOKill();
            movingBall.transform
                .DOPath(path, 0.6f, PathType.CatmullRom)
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
        var renderer = tube.GetComponent<SpriteRenderer>();
        if (renderer != null)
            renderer.color = highlight ? Color.yellow : Color.white;
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

            BallColorType targetColor = tube.GetTopBall()?.GetColor() ?? BallColorType.None;

            for (int i = 0; i < count; i++)
            {
                BallController ball = tube.GetBallAtIndex(i);
                if (ball == null || ball.GetColor() != targetColor)
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
            {
                if (tube != null)
                    Destroy(tube.gameObject);
            }
            allTubes.Clear();
        }

        levelManager?.LoadNextLevel();
    }
}
