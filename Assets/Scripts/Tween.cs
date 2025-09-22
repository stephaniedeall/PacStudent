using UnityEngine;
using System.Collections;

public class Tween
{
    public static IEnumerator Move(Transform transform, Vector3 startPos, Vector3 endPos, float duration, System.Action onComplete = null)
    {
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
    
    public static IEnumerator MoveCircular(Transform transform, Vector3[] path, float durationPerSegment, System.Action onComplete = null)
    {
        int currentIndex = 0;
        
        while (true) 
        {
            Vector3 startPos = path[currentIndex];
            Vector3 endPos = path[(currentIndex + 1) % path.Length];
            
            yield return Move(transform, startPos, endPos, durationPerSegment);
            
            currentIndex = (currentIndex + 1) % path.Length;
        }
    }
}
