using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public LayerMask obstacleLayer;
    public List<Vector2> availableDirections { get; private set; } 

    private void Start()
    {
        this.availableDirections = new List<Vector2>();

        CheckAvailableDirection(Vector2.up);    
        CheckAvailableDirection(Vector2.down);  
        CheckAvailableDirection(Vector2.left);  
        CheckAvailableDirection(Vector2.right); 
    }

    private void CheckAvailableDirection(Vector2 direction) 
    {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, Vector2.one * 0.5f, 0.0f, direction, 1.0f, this.obstacleLayer);

        if (hit.collider == null) {
            this.availableDirections.Add(direction);
        }
    }

    private void OnDrawGizmos()
{
    if (availableDirections != null)
    {
        Gizmos.color = Color.green;
        foreach (Vector2 dir in availableDirections)
        {
            Gizmos.DrawRay(transform.position, dir * 0.5f);
        }
    }
}
}
