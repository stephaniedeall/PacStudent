using UnityEngine;

public class Teleporter : MonoBehaviour
{
    public Teleporter pairedTeleporter;
    public Vector2 outboundDirectionAfterTeleport = Vector2.right; 

    public Vector3 GetPairedDestination()
    {
        if (pairedTeleporter == null) return transform.position;
        return pairedTeleporter.transform.position + (Vector3)(outboundDirectionAfterTeleport.normalized * 0.5f);
    }
}

