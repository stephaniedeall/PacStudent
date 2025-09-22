using UnityEngine;
using System.Collections;

public class PacStudentMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 2f;
    
    [Header("Audio Settings")]
    public AudioClip movementSound;
    
    private Vector3[] pathPoints;
    private Tweener tweener;
    private AudioSource audioSource;
    
    void Start()
    {
        InitializePathPoints();
        InitializeComponents();
        StartMovement();
    }

    private void InitializePathPoints()
    {
        pathPoints = new Vector3[]
        {
            new Vector3(1, 1, 0),      
            new Vector3(3, 1, 0),      
            new Vector3(3, 3, 0),     
            new Vector3(1, 3, 0)      
        };
        
        for (int i = 0; i < pathPoints.Length; i++)
        {
            pathPoints[i] = transform.TransformPoint(pathPoints[i]);
        }
    }
    
    private void InitializeComponents()
    {
        tweener = gameObject.AddComponent<Tweener>();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = movementSound;
        audioSource.loop = true;
    }
    
    private void StartMovement()
    {
        float segmentDistance = Vector3.Distance(pathPoints[0], pathPoints[1]);
        float durationPerSegment = segmentDistance / speed;
        
        tweener.StartCircularMovement(transform, pathPoints, durationPerSegment);
        
        if (movementSound != null)
        {
            audioSource.Play();
        }
    }
        
    private void OnDrawGizmos()
    {
        if (pathPoints == null || pathPoints.Length < 2) return;
        
        Gizmos.color = Color.yellow;
        for (int i = 0; i < pathPoints.Length; i++)
        {
            int nextIndex = (i + 1) % pathPoints.Length;
            Gizmos.DrawLine(pathPoints[i], pathPoints[nextIndex]);
            Gizmos.DrawSphere(pathPoints[i], 0.1f);
        }
    }
    
    public void StopMovement()
    {
        if (tweener != null)
        {
            tweener.StopTween();
        }
        
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
    
    public void RestartMovement()
    {
        StopMovement();
        StartMovement();
    }
} 

