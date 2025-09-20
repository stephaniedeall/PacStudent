using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    public GameObject[] tilePrefabs; 
    public float tileSize = 1f;      

    int[,] levelMap = {
        {1,2,2,2,2,2,2,2,2,2,2,2,2,7},
        {2,5,5,5,5,5,5,5,5,5,5,5,5,4},
        {2,5,3,3,5,3,3,3,3,3,5,3,3,4},
        {2,6,4,4,5,4,4,4,4,4,5,4,4,3},
        {2,5,3,3,5,3,3,3,3,3,5,3,3,4},
        {2,5,5,5,5,5,5,5,5,5,5,5,5,4},
        {2,5,3,3,5,3,3,5,3,3,5,3,3,4},
        {2,5,4,4,5,4,4,8,4,4,5,4,4,3},
        {2,5,3,3,5,3,3,3,3,3,5,3,3,4},
        {2,5,5,5,5,5,5,5,5,5,5,5,5,4},
        {2,5,3,3,5,3,3,3,3,3,5,3,3,4},
        {2,5,4,4,5,4,4,4,4,4,5,4,4,3},
        {2,5,5,5,5,5,5,5,5,5,5,5,5,4},
        {2,2,2,2,2,2,2,2,2,2,2,2,2,0}
    };

    void Start()
    {
        BuildFullLevel();
    }

    void BuildFullLevel()
    {
        int rows = levelMap.GetLength(0);
        int cols = levelMap.GetLength(1);

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                int tileIndex = levelMap[y, x];
                if (tileIndex < 0 || tileIndex >= tilePrefabs.Length) continue;

                PlaceTile(tileIndex, x, y);

                PlaceTile(tileIndex, (cols * 2 - 1) - x, y);

                PlaceTile(tileIndex, x, (rows * 2 - 1) - y);

                PlaceTile(tileIndex, (cols * 2 - 1) - x, (rows * 2 - 1) - y);
            }
        }
    }

    void PlaceTile(int index, int x, int y)
    {
        Vector2 position = new Vector2(x * tileSize, -y * tileSize);
        Instantiate(tilePrefabs[index], position, Quaternion.identity, transform);
    }
}
