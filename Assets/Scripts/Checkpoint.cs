using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Transform playerTransform;

    public Color activatedColor = Color.green;

    public float activationRadius = 3f;

    private bool activated;

    private static Vector3 lastActivatedPosition = Vector3.zero;
    private static bool hasActivatedCheckpoint = false;

    private void Update()
    {
        if (activated) return;

        if (playerTransform == null)
        {
            var player = Object.FindFirstObjectByType<Player>();
            if (player != null)
                playerTransform = player.transform;
        }

        if (playerTransform != null)
        {
            float sqrDist = (playerTransform.position - transform.position).sqrMagnitude;
            if (sqrDist <= activationRadius * activationRadius)
                ActivateCheckpoint();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (activated) return;
        if (other == null) return;

        if (playerTransform == null)
        {
            if (other.GetComponent<Player>() != null)
                ActivateCheckpoint();
            return;
        }

        if (other.transform == playerTransform || other.transform.IsChildOf(playerTransform))
            ActivateCheckpoint();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activated) return;
        if (other == null) return;

        if (playerTransform == null)
        {
            if (other.GetComponent<Player>() != null)
                ActivateCheckpoint();
            return;
        }

        if (other.transform == playerTransform || other.transform.IsChildOf(playerTransform))
            ActivateCheckpoint();
    }

    private void ActivateCheckpoint()
    {
        if (activated) return;
        activated = true;

        RespawnManager.SetCheckpoint(transform.position);

        lastActivatedPosition = transform.position;
        hasActivatedCheckpoint = true;

        var sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.color = activatedColor;

        Debug.Log($"Checkpoint set to {transform.position}");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, activationRadius);

        if (playerTransform != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, playerTransform.position);
        }
    }

    public static bool TryGetLastCheckpointPosition(out Vector3 position)
    {
        if (hasActivatedCheckpoint)
        {
            position = lastActivatedPosition;
            return true;
        }
        position = Vector3.zero;
        return false;
    }
}

public static class RespawnManager
{
    private static Vector3 lastCheckpoint = Vector3.zero;
    private static bool hasCheckpoint = false;

    public static void SetCheckpoint(Vector3 position)
    {
        lastCheckpoint = position;
        hasCheckpoint = true;
    }

    public static Vector3 GetRespawnPosition(Vector3 fallback)
    {
        return hasCheckpoint ? lastCheckpoint : fallback;
    }

    public static void Respawn(GameObject player, Vector3 fallbackPosition)
    {
        if (player == null) return;
        var respawnPos = GetRespawnPosition(fallbackPosition);
        player.transform.position = respawnPos;
    }

    public static bool TryGetLastCheckpoint(out Vector3 position)
    {
        if (hasCheckpoint)
        {
            position = lastCheckpoint;
            return true;
        }
        position = Vector3.zero;
        return false;
    }
}