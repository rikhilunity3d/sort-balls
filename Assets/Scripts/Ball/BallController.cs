using DG.Tweening;
using UnityEngine;

public class BallController : MonoBehaviour
{
    Vector3 originalPosition;
    BallColorType currentBallColor;

    public BallColorType GetColor()
    {
        return currentBallColor;
    }

    public void SetColor(BallColorType newColor)
    {
        currentBallColor = newColor;

        var spriteRenderer = GetComponent<SpriteRenderer>();
        switch (newColor)
        {
            case BallColorType.Red:
                spriteRenderer.color = Color.red;
                break;
            case BallColorType.Green:
                spriteRenderer.color = Color.green;
                break;
            case BallColorType.Blue:
                spriteRenderer.color = Color.blue;
                break;
            case BallColorType.Yellow:
                spriteRenderer.color = Color.yellow;
                break;
        }
    }

    public void RememberPosition()
    {
        originalPosition = transform.position;
    }

    public void LiftToWorldY(float targetWorldY)
    {
        RememberPosition();

        Vector3 liftedPos = new Vector3(
            transform.position.x,
            targetWorldY,
            transform.position.z - 0.2f
        );

        transform.DOMove(liftedPos, 0.3f).SetEase(Ease.OutQuad);
       // GetComponent<SpriteRenderer>().sortingOrder = 10;
    }


    public void ReturnToOriginal()
    {
        transform.DOMove(originalPosition, 0.3f).SetEase(Ease.InQuad);
        //GetComponent<SpriteRenderer>().sortingOrder = 0;
    }

}
