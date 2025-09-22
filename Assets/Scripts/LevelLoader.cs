using UnityEngine;   

public class LevelLoaderAuto : MonoBehaviour
{
    [Tooltip("Assign prefabs for indices 0..8. If you want index 0 to be empty, either assign an empty prefab or leave it null and the script will skip index 0.")]
    public GameObject[] tilePrefabs; 

    [Tooltip("World units per tile. Set to 0.5 for half-sized map.")]
    public float tileSize = 0.25f; 

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
        FitCameraToLevel();
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

                if (tileIndex == 0) continue;

                if (tilePrefabs == null || tileIndex < 0 || tileIndex >= tilePrefabs.Length)
                {
                    Debug.LogWarning($"No prefab for tile {tileIndex} at {x},{y}");
                    continue;
                }

                PlaceTile(tileIndex, x, y);                                   
                PlaceTile(tileIndex, (cols * 2 - 1) - x, y);                  
                PlaceTile(tileIndex, x, (rows * 2 - 1) - y);                  
                PlaceTile(tileIndex, (cols * 2 - 1) - x, (rows * 2 - 1) - y); 
            }
        }
    }

    void PlaceTile(int index, int gridX, int gridY)
    {
        Vector3 pos = new Vector3(gridX * tileSize, -gridY * tileSize, 0f);
        GameObject prefab = tilePrefabs[index];
        if (prefab == null)
        {
            Debug.LogWarning($"Prefab null for index {index}");
            return;
        }

        GameObject inst = Instantiate(prefab, pos, Quaternion.identity, transform);

        SpriteRenderer sr = inst.GetComponent<SpriteRenderer>();
        if (sr != null && sr.sprite != null)
        {
            Vector2 spriteSize = sr.sprite.bounds.size; 
            if (spriteSize.x != 0 && spriteSize.y != 0)
            {
                float sx = tileSize / spriteSize.x;
                float sy = tileSize / spriteSize.y;
                inst.transform.localScale = new Vector3(sx, sy, 1f);
            }
        }

        if (sr != null) sr.sortingOrder = -(int)(pos.y * 100);
    }

    void FitCameraToLevel()
    {
        int rows = levelMap.GetLength(0);
        int cols = levelMap.GetLength(1);
        int totalCols = cols * 2;
        int totalRows = rows * 2;

        float worldWidth = totalCols * tileSize;
        float worldHeight = totalRows * tileSize;

        Camera cam = Camera.main;
        if (cam == null) return;
        if (!cam.orthographic)
        {
            Debug.LogWarning("Main camera should be orthographic for 2D.");
            cam.orthographic = true;
        }

        float centerX = (totalCols - 1) * tileSize / 2f;
        float centerY = -((totalRows - 1) * tileSize / 2f);

        cam.transform.position = new Vector3(centerX, centerY, -10f);

        float padding = 0.25f; 
        float sizeByHeight = worldHeight / 2f + padding;
        float sizeByWidth = (worldWidth / cam.aspect) / 2f + padding;
        cam.orthographicSize = Mathf.Max(sizeByHeight, sizeByWidth);
    }
}

