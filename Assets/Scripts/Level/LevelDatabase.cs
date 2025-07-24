using System.Collections.Generic;

public static class LevelDatabase
{
    public static List<LevelData> Levels = new List<LevelData>()
    {
        new LevelData
        {
            tubes = new BallColorType[][]
            {
                new BallColorType[] { BallColorType.Red, BallColorType.Green, BallColorType.Red, BallColorType.Green },
                new BallColorType[] { BallColorType.Green, BallColorType.Red, BallColorType.Green, BallColorType.Red },
                new BallColorType[] { } // Empty
            }
        },
        new LevelData
        {
            tubes = new BallColorType[][]
            {
                new BallColorType[] { BallColorType.Blue, BallColorType.Yellow, BallColorType.Blue, BallColorType.Yellow },
                new BallColorType[] { BallColorType.Yellow, BallColorType.Blue, BallColorType.Yellow, BallColorType.Blue },
                new BallColorType[] { }, new BallColorType[] { }
            }
        }
    };
}
