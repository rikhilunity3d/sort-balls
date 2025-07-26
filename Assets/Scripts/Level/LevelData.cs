using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "ColorSort/Level Data", order = 1)]
public class LevelData : ScriptableObject
{
    public BallColorType[] tube0;
    public BallColorType[] tube1;
    public BallColorType[] tube2;
    public BallColorType[] tube3;
    public BallColorType[] tube4;
    public BallColorType[] tube5;

    public BallColorType[][] GetTubes()
    {
        return new BallColorType[][]
        {
            tube0, tube1, tube2, tube3, tube4, tube5
        };
    }
}
