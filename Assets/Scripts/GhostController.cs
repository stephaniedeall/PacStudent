using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GhostController : MonoBehaviour
{
    [Header("Settings")]
    public int ghostNumber;
    public float normalSpeed = 2.7f, scaredSpeed = 1.35f, recoverySpeed = 1.8f;
    public LayerMask wallLayer;

    [Header("References")]
    public Transform pacStudent;

    [Header("Gameplay")]
    public int points = 200;
    public bool showDebug = true;

    public GhostState CurrentState = GhostState.Normal;

    private Vector3 targetPos, startLerpPos, startingPos;
    private Vector2 currentDir = Vector2.right, prevDir, nextDir;
    private float lerpT;
    private bool isLerping;
    private SpriteRenderer spriteRenderer;

    private static readonly Vector2[] Directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
    private static readonly Vector2[] Clockwise = { Vector2.right, Vector2.down, Vector2.left, Vector2.up };

    public enum GhostState { Normal, Scared, Recovering, Dead }

    void Start()
    {
        startingPos = transform.position;
        targetPos = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update() => Move();

    void Move()
    {
        if (isLerping) ContinueLerp();
        else
        {
            ChooseNextDir();
            StartLerp(nextDir);
        }
    }

    void StartLerp(Vector2 dir)
    {
        if (dir == Vector2.zero) return;
        startLerpPos = RoundToGrid(transform.position);
        targetPos = RoundToGrid(startLerpPos + (Vector3)dir);
        lerpT = 0f;
        isLerping = true;
        nextDir = dir;
    }

    void ContinueLerp()
    {
        lerpT += GetSpeed() * Time.deltaTime;
        transform.position = Vector3.Lerp(startLerpPos, targetPos, lerpT);

        if (lerpT >= 1f)
        {
            transform.position = targetPos;
            isLerping = false;
            prevDir = currentDir;
            currentDir = nextDir;
            lerpT = 0f;
        }
    }

    void ChooseNextDir()
    {
        var valid = GetValidDirs();
        if (valid.Count == 0)
        {
            nextDir = -currentDir;
            return;
        }

        nextDir = CurrentState switch
        {
            GhostState.Normal => ChooseNormalDir(valid),
            GhostState.Scared or GhostState.Recovering => valid[Random.Range(0, valid.Count)],
            _ => nextDir
        };
    }

    Vector2 ChooseNormalDir(List<Vector2> dirs)
    {
        return ghostNumber switch
        {
            1 => FindDirToward(dirs, pacStudent.position, true),
            2 => FindDirToward(dirs, pacStudent.position, false),
            3 => dirs[Random.Range(0, dirs.Count)],
            4 => ChooseClockwise(dirs),
            _ => dirs[Random.Range(0, dirs.Count)]
        };
    }

    Vector2 FindDirToward(List<Vector2> dirs, Vector3 target, bool chase)
    {
        Vector2 best = dirs[0];
        float bestDist = Vector3.Distance(transform.position + (Vector3)best, target);

        foreach (var dir in dirs)
        {
            float dist = Vector3.Distance(transform.position + (Vector3)dir, target);
            if ((chase && dist < bestDist) || (!chase && dist > bestDist))
            {
                best = dir;
                bestDist = dist;
            }
        }
        return best;
    }

    Vector2 ChooseClockwise(List<Vector2> dirs)
    {
        int idx = System.Array.IndexOf(Clockwise, currentDir);
        for (int i = 0; i < Clockwise.Length; i++)
        {
            var dir = Clockwise[(idx + i) % Clockwise.Length];
            if (dirs.Contains(dir) && dir != -prevDir) return dir;
        }
        return dirs[Random.Range(0, dirs.Count)];
    }

    List<Vector2> GetValidDirs()
    {
        var valid = new List<Vector2>();
        foreach (var dir in Directions)
        {
            if (dir == -prevDir) continue;
            if (CurrentState != GhostState.Dead && !IsWalkable(transform.position, dir)) continue;
            valid.Add(dir);
        }
        return valid;
    }

    bool IsWalkable(Vector3 pos, Vector2 dir)
    {
        if (CurrentState == GhostState.Dead) return true;
        var hit = Physics2D.BoxCast(RoundToGrid(pos), new Vector2(0.8f, 0.8f), 0, dir, 0.5f, wallLayer);
        if (showDebug && hit.collider) Debug.DrawLine(pos, hit.point, Color.red, 1f);
        return !hit.collider;
    }

    float GetSpeed() => CurrentState switch
    {
        GhostState.Normal => normalSpeed,
        GhostState.Scared => scaredSpeed,
        GhostState.Recovering => recoverySpeed,
        GhostState.Dead => normalSpeed * 2f,
        _ => normalSpeed
    };

    public void SetState(GhostState newState)
    {
        CurrentState = newState;
        if (newState is not (GhostState.Scared or GhostState.Recovering))
            StopAllCoroutines();

        Debug.Log($"Ghost {ghostNumber} state changed to: {newState}");
    }

    public void EnableFrightenedMode(float duration)
    {
        if (CurrentState == GhostState.Dead) return;
        StopAllCoroutines();
        StartCoroutine(FrightenedRoutine(duration));
    }

    IEnumerator FrightenedRoutine(float duration)
    {
        CurrentState = GhostState.Scared;
        yield return new WaitForSeconds(duration * 0.8f);
        CurrentState = GhostState.Recovering;
        yield return new WaitForSeconds(duration * 0.2f);
        if (CurrentState != GhostState.Dead) CurrentState = GhostState.Normal;
    }

    public void KillGhost()
    {
        if (CurrentState == GhostState.Dead) return;
        CurrentState = GhostState.Dead;
        StopAllCoroutines();
    }

    public void ResetState()
    {
        transform.position = startingPos;
        CurrentState = GhostState.Normal;
        isLerping = false;
        lerpT = 0f;
        currentDir = Vector2.right;
        prevDir = nextDir = Vector2.zero;
        StopAllCoroutines();

        Debug.Log($"Ghost {ghostNumber} state reset");
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (CurrentState == GhostState.Dead) return;
        if (!col.TryGetComponent<PacStudentController>(out var pac)) return;

        if (CurrentState is GhostState.Scared or GhostState.Recovering)
            KillGhost();
        else
            FindFirstObjectByType<GameManager>()?.PacStudentEaten();
    }

    static Vector3 RoundToGrid(Vector3 pos) =>
        new(Mathf.Round(pos.x), Mathf.Round(pos.y), pos.z);
}

