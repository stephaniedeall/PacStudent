using UnityEngine;
using System.Collections;

public class PacStudentController : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed = 3.0f;
    public LayerMask obstacleLayer;
    public bool showDebug = true;

    [Header("Audio")]
    public AudioSource movementAudio;
    public AudioClip eatingClip, movingClip;
    public AudioSource sfxSource;
    public AudioClip wallClip;

    [Header("Visuals")]
    public ParticleSystem wallBumpParticlesPrefab;

    [Header("References")]
    public GameManager gameManager;
    public Rigidbody2D rb;
    public Collider2D bodyCollider;
    public Transform ghostsParent;

    public Vector2 lastInput { get; private set; } = Vector2.right;
    public Vector2 currentInput { get; private set; }

    private Vector3 startPosition;
    private Vector3 startPos;     
    private Vector3 targetPos;
    private Vector2 lastLerpPosition;

    private float lerpProgress;
    private bool isLerping;
    private bool isMoving;

    private void Start()
    {
        gameManager ??= FindFirstObjectByType<GameManager>();
        ghostsParent ??= GameObject.Find("Ghosts")?.transform;

        if (rb == null) rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        if (!GetComponent<Collider2D>())
        {
            gameObject.AddComponent<CircleCollider2D>();
            Debug.Log("Added CircleCollider2D to PacStudent");
        }

        targetPos = transform.position;
        startPosition = transform.position;
        movementAudio?.Stop();
    }

    private void Update()
    {
        HandleInput();
        HandleMovement();
        UpdateRotation();
        HandleAudio();
    }

    private void HandleInput()
    {
        Vector2 input = Vector2.zero;
        if (Input.GetKey(KeyCode.W)) input = Vector2.up;
        else if (Input.GetKey(KeyCode.S)) input = Vector2.down;
        else if (Input.GetKey(KeyCode.A)) input = Vector2.left;
        else if (Input.GetKey(KeyCode.D)) input = Vector2.right;

        if (input != Vector2.zero)
            lastInput = input;
    }

    private void HandleMovement()
{
    if (isLerping)
    {
        lastLerpPosition = transform.position; 
        lerpProgress += moveSpeed * Time.deltaTime;
        transform.position = Vector3.Lerp(startPos, targetPos, lerpProgress);

        if (lerpProgress >= 1f)
        {
            transform.position = targetPos;
            isLerping = false;
            CheckForPellets();
        }

        return;
    }

    if (TryMove(lastInput) || TryMove(currentInput)) return;
}

    private bool TryMove(Vector2 dir)
    {
        if (dir == Vector2.zero || !IsWalkable(transform.position, dir)) return false;

        currentInput = dir;
        startPos = GetGridAlignedPosition(transform.position);
        targetPos = GetGridAlignedPosition(startPos + (Vector3)dir);
        isLerping = true;
        lerpProgress = 0f;
        return true;
    }

    private bool IsWalkable(Vector3 pos, Vector2 dir)
    {
        Vector3 origin = GetGridAlignedPosition(pos);
        RaycastHit2D hit = Physics2D.BoxCast(origin, Vector2.one * 0.8f, 0f, dir, 0.5f, obstacleLayer);

        if (showDebug)
        {
            Debug.DrawLine(origin, origin + (Vector3)dir, hit ? Color.red : Color.green, 0.1f);
            if (hit) Debug.Log($"Blocked by {hit.collider.name} in {dir}");
        }

        return !hit;
    }

    private Vector3 GetGridAlignedPosition(Vector3 pos)
        => new(Mathf.Round(pos.x), Mathf.Round(pos.y), pos.z);

    private void UpdateRotation()
    {
        if (currentInput != Vector2.zero)
        {
            float angle = Mathf.Atan2(currentInput.y, currentInput.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    private void HandleAudio()
    {
        if (!movementAudio) return;

        if (isLerping)
        {
            AudioClip targetClip = IsNextPositionHasPellet() ? eatingClip : movingClip;
            if (movementAudio.clip != targetClip)
            {
                movementAudio.clip = targetClip;
                movementAudio.Play();
            }
        }
        else if (movementAudio.isPlaying)
        {
            movementAudio.Stop();
        }
    }

    private bool IsNextPositionHasPellet()
    {
        foreach (var c in Physics2D.OverlapCircleAll(targetPos, 0.3f))
            if (c.GetComponent<Pellet>() || c.GetComponent<PowerPellet>())
                return true;
        return false;
    }

    private void CheckForPellets()
    {
        foreach (var c in Physics2D.OverlapCircleAll(transform.position, 0.3f))
        {
            if (c.TryGetComponent(out Pellet pellet))
                gameManager?.PelletEaten(pellet);
            else if (c.TryGetComponent(out PowerPellet powerPellet))
                gameManager?.PowerPelletEaten(powerPellet);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            HandleWallCollision(collision.GetContact(0).point);
        }
    }

    private void HandleWallCollision(Vector2 contactPoint)
    {
        transform.position = lastLerpPosition;

        if (wallBumpParticlesPrefab != null)
        {
            ParticleSystem p = Instantiate(wallBumpParticlesPrefab, contactPoint, Quaternion.identity);
            p.Play();
            Destroy(p.gameObject, 1.5f);
        }

        if (sfxSource != null && wallClip != null)
        {
            sfxSource.PlayOneShot(wallClip);
        }

        isLerping = false;
        currentInput = Vector2.zero;
    }

    private void HandlePellet(GameObject pellet)
    {
        gameManager?.AddScore(10);
        Destroy(pellet);
    }

    private void HandleCherry(GameObject cherry)
    {
        gameManager?.AddScore(100);
        Destroy(cherry);
    }

    private void HandlePowerPill(GameObject pill)
    {
        gameManager?.AddScore(50);
        Destroy(pill);
        gameManager?.StartGhostScaredMode(10f);
    }

    private void HandleTeleporter(Teleporter tele)
    {
        if (tele == null) return;
        Vector3 dest = tele.GetPairedDestination();
        transform.position = dest;
    }

    private void HandleGhostCollision(GhostController ghost)
    {
        if (ghost == null) return;

        if (ghost.CurrentState == GhostController.GhostState.Normal)
        {
            gameManager?.PacStudentEaten();
        }
        else if (ghost.CurrentState == GhostController.GhostState.Scared ||
                 ghost.CurrentState == GhostController.GhostState.Recovering)
        {
            gameManager?.AddScore(300);
            ghost.KillGhost();
            gameManager?.OnGhostEaten();
        }
    }

    public void ResetState()
    {
        transform.position = GetGridAlignedPosition(startPosition);
        targetPos = transform.position;
        isLerping = false;
        lerpProgress = 0f;
        if (movementAudio) movementAudio.Stop();

        lastInput = Vector2.right;
        currentInput = Vector2.zero;
    }
}












