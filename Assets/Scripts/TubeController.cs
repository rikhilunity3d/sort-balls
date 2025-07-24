using System.Collections.Generic;
using UnityEngine;

public class TubeController : MonoBehaviour
{
    [SerializeField]
    private int tubeMaxCapacity = 4;

    private List<BallController> balls = new List<BallController>();

    void Start()
    {
    }


    // Adds a ball to the tube if it's not full.
    // Returns true if successful, false otherwise.
    public bool AddBall(BallController ball)
    {
        if (IsFull())
            return false;

        balls.Add(ball);
        return true;
    }


    // Removes and returns the top ball from the tube.
    // Returns null if the tube is empty.
    public BallController RemoveTopBall()
    {
        if (IsEmpty())
            return null;

        BallController topBall = balls[balls.Count - 1];
        balls.RemoveAt(balls.Count - 1);
        return topBall;
    }


    // Peeks at the top ball without removing it.
    public BallController GetTopBall()
    {
        if (IsEmpty())
            return null;

        return balls[balls.Count - 1];
    }


    // Returns true if the tube has reached its max capacity.
    public bool IsFull()
    {
        return balls.Count >= tubeMaxCapacity;
    }

    // Returns true if the tube is empty.
    public bool IsEmpty()
    {
        return balls.Count == 0;
    }

    // Determines if this tube can receive a given ball based on color and capacity.
    public bool CanReceiveBall(BallController ball)
    {
        if (IsFull() || ball == null)
            return false;

        if (IsEmpty())
            return true;

        BallController topBall = GetTopBall();
        return topBall != null && topBall.GetColor() == ball.GetColor();
    }


    // Returns the number of balls in the tube.
    public int GetBallCount()
    {
        return balls.Count;
    }

    public int GetCapacity()
    {
        return tubeMaxCapacity;
    }

    //this will give the tube's actual height in world units. 
    // So if sprite of tube changes it will give the updated one.
    public float GetTubeHeight()
    {
        return GetComponent<SpriteRenderer>().bounds.size.y;
    }

    private void OnMouseDown()
    {
        GameManager.Instance.OnTubeClicked(this);
    }

    public BallController GetBallAtIndex(int index)
    {
        if (index >= 0 && index < balls.Count)
            return balls[index];
        return null;
    }

}
