using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private BallController ballPrefab;
    [SerializeField] private TubeController tubePrefab;
    [SerializeField] private Transform tubeParent;

    [HideInInspector] public List<TubeController> tubes = new List<TubeController>();
    public List<TubeController> Tubes => tubes;

    public void LoadLevel(int levelIndex)
    {
        // Clear previous level (optional for replaying)
        foreach (Transform child in tubeParent)
        {
            Destroy(child.gameObject);
        }

        tubes.Clear();

        var levelData = LevelDatabase.Levels[levelIndex];

        for (int i = 0; i < levelData.tubes.Length; i++)
        {
            TubeController tube = Instantiate(tubePrefab, tubeParent);
            tube.transform.position = new Vector3(i , 0, 0); // simple horizontal layout
            //tube.transform.position = new Vector3(i * 2f, 0, 0); // simple horizontal layout
            tubes.Add(tube);

            var colors = levelData.tubes[i];
            for (int j = 0; j < colors.Length; j++)
            {
                BallController ball = Instantiate(ballPrefab, tube.transform);
                ball.SetColor(colors[j]);

                Vector3 pos = tube.transform.position + new Vector3(0, 0.71f * j, 0);
                ball.transform.position = pos;

                tube.AddBall(ball);
            }
        }

        GameManager.Instance.SetTubes(tubes); // pass to GameManager
    }
}
