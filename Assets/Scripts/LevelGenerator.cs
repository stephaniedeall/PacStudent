using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameObject wallPrefab;
    public GameObject pelletPrefab;
    public GameObject powerPelletPrefab;
    public GameObject cornerPrefab;
    public GameObject tJunctionPrefab;
    public GameObject emptyPrefab; 

    int[,] levelMap = new int[,]
    {
        {1, 1, 1, 1, 1},
        {1, 0, 2, 0, 1},
        {1, 2, 3, 2, 1},
        {1, 0, 2, 0, 1},
        {1, 1, 1, 1, 1}
    };

    void Start()
    {
        GameObject oldLevel = GameObject.Find("Level 01");
        if (oldLevel != null)
        {
            Destroy(oldLevel);
        }

        GenerateLevel();
        MirrorLevel();
        AdjustCamera();
    }

    void GenerateLevel()
    {
        int rows = levelMap.GetLength(0);
        int cols = levelMap.GetLength(1);

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                int tile = levelMap[y, x];
                Vector3 position = new Vector3(x, -y, 0);

                GameObject prefab = GetPrefabForTile(tile);
                if (prefab != null)
                {
                    GameObject piece = Instantiate(prefab, position, Quaternion.identity, transform);
                    piece.name = $"Tile_{x}_{y}";

                    RotatePiece(piece, x, y);
                }

            }
        }
    }

    GameObject GetPrefabForTile(int tile)
    {
        switch (tile)
        {
            case 0: return null; 
            case 1: return wallPrefab;
            case 2: return pelletPrefab;
            case 3: return cornerPrefab;
            case 4: return tJunctionPrefab;
            case 5: return powerPelletPrefab;
            default: return emptyPrefab;
        }
    }

    void RotatePiece(GameObject piece, int x, int y)
    {
        int rows = levelMap.GetLength(0);
        int cols = levelMap.GetLength(1);

        bool up    = (y > 0) && (levelMap[y - 1, x] != 0);
        bool down  = (y < rows - 1) && (levelMap[y + 1, x] != 0);
        bool left  = (x > 0) && (levelMap[y, x - 1] != 0);
        bool right = (x < cols - 1) && (levelMap[y, x + 1] != 0);

        if (piece.CompareTag("Wall"))
        {
            if (up || down) 
                piece.transform.rotation = Quaternion.identity;
            if (left || right) 
                piece.transform.rotation = Quaternion.Euler(0, 0, 90);
        }

        if (piece.CompareTag("Corner"))
        {
            if (up && right) piece.transform.rotation = Quaternion.identity;
            else if (right && down) piece.transform.rotation = Quaternion.Euler(0, 0, -90);
            else if (down && left) piece.transform.rotation = Quaternion.Euler(0, 0, 180);
            else if (left && up) piece.transform.rotation = Quaternion.Euler(0, 0, 90);
        }

        if (piece.CompareTag("TJunction"))
        {
            if (!up) piece.transform.rotation = Quaternion.identity;
            else if (!right) piece.transform.rotation = Quaternion.Euler(0, 0, 90);
            else if (!down) piece.transform.rotation = Quaternion.Euler(0, 0, 180);
            else if (!left) piece.transform.rotation = Quaternion.Euler(0, 0, -90);
        }
    }

    void MirrorLevel()
    {
        int rows = levelMap.GetLength(0);
        int cols = levelMap.GetLength(1);

        Transform[] tiles = GetComponentsInChildren<Transform>();

        foreach (Transform tile in tiles)
        {
            if (tile == transform) continue;

            Vector3 pos = tile.position;
            Quaternion rot = tile.rotation;

            Instantiate(tile.gameObject, new Vector3(-pos.x, pos.y, pos.z), rot, transform);
            Instantiate(tile.gameObject, new Vector3(pos.x, -pos.y, pos.z), rot, transform);
            Instantiate(tile.gameObject, new Vector3(-pos.x, -pos.y, pos.z), rot, transform);
        }
    }

    void AdjustCamera()
    {
        int rows = levelMap.GetLength(0) * 2;
        int cols = levelMap.GetLength(1) * 2;

        Camera cam = Camera.main;
        cam.orthographic = true;
        cam.orthographicSize = Mathf.Max(rows, cols) / 2f + 2;
        cam.transform.position = new Vector3(cols / 2f, -rows / 2f, -10f);
    }
}


