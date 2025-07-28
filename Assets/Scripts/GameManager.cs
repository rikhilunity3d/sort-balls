using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private List<TubeController> allTubes;
    private TubeController selectedTube = null;
    [SerializeField] private LevelManager levelManager;

    private int totalScore = 0;




    private void Awake()
    {
        // Singleton pattern setup
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
    }

    public void ClearTubes()
    {
        allTubes.Clear();
        selectedTube = null;
    }

    // Called by TubeController when it is clicked.
    /// Handles selection and transfer logic.

    public void OnTubeClicked(TubeController clickedTube)
    {
        Debug.Log("Clicked on Tube: " + clickedTube.name);

        // First click – select tube
        if (selectedTube == null)
        {
            if (!clickedTube.IsEmpty())
            {
                selectedTube = clickedTube;
                HighlightTube(selectedTube, true);

                float tubeTopY = selectedTube.transform.position.y + selectedTube.GetTubeHeight();
                float liftTargetY = tubeTopY + 0.30f; // or 10–20% higher if needed

                BallController topBall = selectedTube.GetTopBall();

                if (topBall != null)
                {
                    topBall.LiftToWorldY(liftTargetY);
                    // visually lifts the ball
                }

                return;
            }
        }

        // Clicked same tube again → cancel selection
        if (clickedTube == selectedTube)
        {
            BallController topBall = selectedTube.GetTopBall();
            if (topBall != null)
            {
                topBall.ReturnToOriginal(); // return to place
            }

            HighlightTube(selectedTube, false);
            selectedTube = null;
            return;
        }

        // Attempt to move top ball
        BallController ballToMove = selectedTube.GetTopBall();

        if (ballToMove != null && clickedTube.CanReceiveBall(ballToMove))
        {
            selectedTube.RemoveTopBall();
            clickedTube.AddBall(ballToMove);

            // Reparent the ball
            ballToMove.transform.SetParent(clickedTube.transform);

            // Animate move to new position
            float yOffset = 0.71f;
            int targetIndex = clickedTube.GetBallCount() - 1;
            Vector3 newPos = clickedTube.transform.position + Vector3.up * yOffset * targetIndex;
            ballToMove.transform.DOMove(newPos, 0.3f).SetEase(Ease.OutBack);

            CheckWinCondition();
        }
        else
        {
            // Invalid move → return to original
            ballToMove?.ReturnToOriginal();
        }

        HighlightTube(selectedTube, false);
        selectedTube = null;
    }

    // Add highlight effect to selected tube.

    private void HighlightTube(TubeController tube, bool highlight)
    {
        var renderer = tube.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.color = highlight ? Color.yellow : Color.white;
        }
    }


    // Check win logic.
    private void CheckWinCondition()
    {
        foreach (var tube in allTubes)
        {
            int count = tube.GetBallCount();

            if (count == 0)
                continue;

            if (count != tube.GetCapacity())
                return;

            BallColorType firstColor = tube.GetTopBall().GetColor();

            for (int i = 0; i < count; i++)
            {
                BallController ball = tube.GetBallAtIndex(i);
                if (ball == null || ball.GetColor() != firstColor)
                    return;
            }
        }

        Debug.Log("✅ Level Completed!");

        // ✅ Delay to allow effects (optional)
        Invoke(nameof(HandleLevelComplete), 0.5f);
    }

    private void HandleLevelComplete()
    {
        // Destroy all tubes
        foreach (var tube in allTubes)
        {
            if (tube != null)
                Destroy(tube.gameObject);
        }

        // ✅ Clear internal list
        allTubes.Clear();

        // ✅ Next Level
        levelManager.LoadNextLevel();
    }


}
