using UnityEngine;

[CreateAssetMenu(fileName = "NewTubeData", menuName = "ColorSort/Tube Data", order = 2)]
public class TubeData : ScriptableObject
{
    [Tooltip("Assign up to 4 colors in this tube.")]
    public BallColorType[] balls = new BallColorType[4];
}
