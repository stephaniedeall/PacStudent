using UnityEngine;
using System.Collections;

public class Tweener : MonoBehaviour
{
    private Coroutine currentTween;
    
    public void StartMove(Transform transform, Vector3 startPos, Vector3 endPos, float duration, System.Action onComplete = null)
    {
        if (currentTween != null)
        {
            StopCoroutine(currentTween);
        }
        
        currentTween = StartCoroutine(MoveCoroutine(transform, startPos, endPos, duration, onComplete));
    }
    
    public void StartCircularMovement(Transform transform, Vector3[] path, float durationPerSegment)
    {
        if (path == null || path.Length < 2)
        {
            Debug.LogError("Path must contain at least 2 points!");
            return;
        }
        
        if (currentTween != null)
        {
            StopCoroutine(currentTween);
        }
        
        currentTween = StartCoroutine(MoveCircularCoroutine(transform, path, durationPerSegment));
    }
    
    public void StopTween()
    {
        if (currentTween != null)
        {
            StopCoroutine(currentTween);
            currentTween = null;
        }
    }
    
    private IEnumerator MoveCoroutine(Transform transform, Vector3 startPos, Vector3 endPos, float duration, System.Action onComplete)
    {
        if (transform == null) yield break;
        
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        transform.position = endPos;
        onComplete?.Invoke(); 
    }
    
    private IEnumerator MoveCircularCoroutine(Transform transform, Vector3[] path, float durationPerSegment)
    {
        if (transform == null) yield break;
        
        int currentIndex = 0;
        
        while (true) 
        {
            Vector3 startPos = path[currentIndex];
            Vector3 endPos = path[(currentIndex + 1) % path.Length];
            
            yield return MoveCoroutine(transform, startPos, endPos, durationPerSegment, null);
            
            currentIndex = (currentIndex + 1) % path.Length;
        }
    }
}
