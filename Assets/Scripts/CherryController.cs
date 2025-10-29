using UnityEngine;
using System.Collections;

public class CherryController : MonoBehaviour
{
    public GameObject cherryPrefab;
    public float moveDuration = 10f;  
    public float respawnDelay = 5f;

    private LevelGenerator levelGen;
    private float timer;
    private bool isMoving = false;
    private Vector3 startPos, endPos;
    private GameObject currentCherry;
        void Start()
    {
            levelGen = FindFirstObjectByType<LevelGenerator>();
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(respawnDelay);

        while (true)
        {
            SpawnCherry();
            yield return new WaitForSeconds(moveDuration + respawnDelay);
        }
    }
        void SpawnCherry()
{
    if (isMoving) return; 
    
    bool fromLeft = Random.value > 0.5f;
    float offset = 2f;

    startPos = fromLeft
        ? new Vector3(-offset, 0, 0)
        : new Vector3(levelGen.levelWidth + offset, 0, 0);

    endPos = fromLeft
        ? new Vector3(levelGen.levelWidth + offset, 0, 0)
        : new Vector3(-offset, 0, 0);

    currentCherry = Instantiate(cherryPrefab, startPos, Quaternion.identity);
    StartCoroutine(MoveCherry());
}
    IEnumerator MoveCherry()
    {
        float t = 0f;
        isMoving = true;

        while (t < 1f)
        {
            if (currentCherry == null) yield break;

            currentCherry.transform.position = Vector3.Lerp(startPos, endPos, t);
            t += Time.deltaTime / moveDuration;
            yield return null;
        }

        Destroy(currentCherry);
        isMoving = false;
    }

}

