using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Refs")]
    [SerializeField] private LevelManager levelManager;

    [Header("Stacking")]
    [Tooltip("Vertical distance between stacked balls (must match LevelLoader).")]
    [SerializeField] private float slotSpacingY = 0.55f; // MUST match LevelLoader

    [Header("Movement")]
    [Tooltip("Time it takes to move a ball to a new tube.")]
    [SerializeField] private float moveDuration = 0.35f;
    [Tooltip("Minimum jump height for arc movement.")]
    [SerializeField] private float minJumpPower = 0.6f;
    [Tooltip("Maximum jump height for arc movement.")]
    [SerializeField] private float maxJumpPower = 2.0f;
    [Tooltip("Scales jump height by horizontal distance.")]
    [SerializeField] private float jumpPowerPerUnitX = 0.25f;

    private List<TubeController> allTubes;
    private TubeController selectedTube = null;

    // (Optional) score tracking
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

    /// <summary>
    /// Called by TubeController.OnMouseDown() — selection & transfer.
    /// </summary>
    public void OnTubeClicked(TubeController clickedTube)
    {
        if (clickedTube == null || allTubes == null) return;

        // First click → select source tube (must have at least one ball)
        if (selectedTube == null)
        {
            if (!clickedTube.IsEmpty())
            {
                selectedTube = clickedTube;
                HighlightTube(selectedTube, true);

                // Visual lift in world space
                float tubeTopY = selectedTube.transform.position.y + selectedTube.GetTubeHeight();
                float liftTargetY = tubeTopY + 0.30f;
                BallController topBall = selectedTube.GetTopBall();
                topBall?.LiftToWorldY(liftTargetY);
            }
            return;
        }

        // Click again on same tube → cancel selection
        if (clickedTube == selectedTube)
        {
            BallController topBall = selectedTube.GetTopBall();
            topBall?.ReturnToOriginal();

            HighlightTube(selectedTube, false);
            selectedTube = null;
            return;
        }

        // Attempt transfer: selectedTube → clickedTube
        BallController movingBall = selectedTube.GetTopBall();
        if (movingBall != null && clickedTube.CanReceiveBall(movingBall))
        {
            // Update tube stacks (data)
            selectedTube.RemoveTopBall();
            clickedTube.AddBall(movingBall);

            // Reparent first (keep world position)
            movingBall.transform.SetParent(clickedTube.transform, true);

            // Compute local target slot & convert to world for DOJump arc
            int targetIndex = clickedTube.GetBallCount() - 1; // last slot index
            Vector3 localTarget = new Vector3(0f, slotSpacingY * targetIndex, 0f);
            Vector3 targetWorldPos = clickedTube.transform.TransformPoint(localTarget);

            // Dynamic jump height based on horizontal distance
            float distX = Mathf.Abs(targetWorldPos.x - movingBall.transform.position.x);
            float jumpPower = Mathf.Clamp(minJumpPower + distX * jumpPowerPerUnitX, minJumpPower, maxJumpPower);

            // Animate arc jump in world space
            movingBall.transform.DOKill();
            movingBall.transform
                      .DOJump(targetWorldPos, jumpPower, 1, moveDuration)
                      .SetEase(Ease.OutQuad);

            // Check win after a valid move
            CheckWinCondition();
        }
        else
        {
            // Invalid move → return the lifted ball
            movingBall?.ReturnToOriginal();
        }

        // Deselect source tube
        HighlightTube(selectedTube, false);
        selectedTube = null;
    }

    private void HighlightTube(TubeController tube, bool highlight)
    {
        if (tube == null) return;
        var sr = tube.GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = highlight ? Color.yellow : Color.red; // as per your current choice
    }

    /// <summary>
    /// All non-empty tubes must be full and same color.
    /// </summary>
    private void CheckWinCondition()
    {
        if (allTubes == null) return;

        foreach (var tube in allTubes)
        {
            if (tube == null) continue;

            int count = tube.GetBallCount();
            if (count == 0) continue;                // empty tube is fine
            if (count != tube.GetCapacity()) return; // not full → not win

            BallController top = tube.GetTopBall();
            if (top == null) return;
            BallColorType color = top.GetColor();

            for (int i = 0; i < count; i++)
            {
                var b = tube.GetBallAtIndex(i);
                if (b == null || b.GetColor() != color)
                    return; // mixed → not win
            }
        }

        // All checks passed → Win!
        Debug.Log("✅ Level Completed!");
        Invoke(nameof(HandleLevelComplete), 0.5f);
    }

    private void HandleLevelComplete()
    {
        // Optional: accumulate score
        if (levelManager != null)
        {
            totalScore += levelManager.GetCurrentLevelScore();
            Debug.Log($"🏆 Total Score: {totalScore}");
        }

        // Cleanup current tubes
        if (allTubes != null)
        {
            foreach (var tube in allTubes)
                if (tube != null) Destroy(tube.gameObject);
            allTubes.Clear();
        }

        // Next level
        levelManager?.LoadNextLevel();
    }
}
