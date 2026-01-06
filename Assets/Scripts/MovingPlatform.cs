using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Path")]
    [Tooltip("If enabled you can assign two transforms directly instead of an array.")]
    public bool useTwoPoints = false;
    [Tooltip("First point when using two-point mode.")]
    public Transform pointA;
    [Tooltip("Second point when using two-point mode.")]
    public Transform pointB;

    [Tooltip("Transforms used as waypoints. If empty the platform will not move.")]
    public Transform[] waypoints;

    [Header("Movement")]
    [Tooltip("Speed in units per second.")]
    public float speed = 2f;

    [Tooltip("Time in seconds to wait on each waypoint.")]
    public float waitTime = 0f;

    [Tooltip("If true the platform will ping-pong between endpoints. If false it will loop.")]
    public bool pingPong = true;

    int currentIndex = 0;
    int direction = 1; 
    float waitTimer = 0f;

    BoxCollider box3D;
    BoxCollider2D box2D;

    void Start()
    {
        if (useTwoPoints)
        {
            if (pointA != null && pointB != null)
            {
                waypoints = new Transform[] { pointA, pointB };
                currentIndex = 0;
                direction = 1;
            }
            else
            {
                Debug.LogWarning("MovingPlatform: useTwoPoints is enabled but pointA or pointB is not assigned.");
            }
        }

        box3D = GetComponent<BoxCollider>();
        box2D = GetComponent<BoxCollider2D>();

        if (box3D == null && box2D == null)
        {
            Debug.LogWarning("MovingPlatform: No BoxCollider or BoxCollider2D found. Parenting via triggers will not work. Consider adding a BoxCollider and enabling 'Is Trigger'.");
        }
    }

    void FixedUpdate()
    {
        if (waypoints == null || waypoints.Length == 0)
            return;

        Transform target = waypoints[currentIndex];
        if (target == null)
            return;

        Vector3 currentPosition = transform.position;
        Vector3 targetPosition = target.position;

        Vector3 next = Vector3.MoveTowards(currentPosition, targetPosition, speed * Time.fixedDeltaTime);

        transform.position = next;

        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            if (waitTime > 0f)
            {
                waitTimer += Time.fixedDeltaTime;
                if (waitTimer < waitTime)
                    return;
                waitTimer = 0f;
            }
            AdvanceIndex();
        }
    }

    void AdvanceIndex()
    {
        if (pingPong)
        {
            currentIndex += direction;
            if (currentIndex >= waypoints.Length)
            {
                currentIndex = Mathf.Max(0, waypoints.Length - 2);
                direction = -1;
            }
            else if (currentIndex < 0)
            {
                currentIndex = 1;
                direction = 1;
            }
        }
        else
        {
            currentIndex = (currentIndex + 1) % waypoints.Length;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        TryParentIfOnTopByCollision(collision.collider.transform, collision.contacts);
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.transform.parent == transform)
            collision.collider.transform.SetParent(null);
    }

    void OnCollisionEnter(Collision collision)
    {
        TryParentIfOnTopByCollision(collision.transform, collision.contacts);
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.transform.parent == transform)
            collision.transform.SetParent(null);
    }

    void OnTriggerEnter(Collider other)
    {
        TryParentIfOnTopByTrigger(other.transform, other);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.transform.parent == transform)
            other.transform.SetParent(null);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        TryParentIfOnTopByTrigger2D(other.transform, other);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform.parent == transform)
            other.transform.SetParent(null);
    }

    void TryParentIfOnTopByCollision(Transform other, ContactPoint[] contacts)
    {
        if (contacts == null || contacts.Length == 0)
            return;

        foreach (var c in contacts)
        {
            if (c.normal.y > 0.5f)
            {
                other.SetParent(transform);
                return;
            }
        }
    }

    void TryParentIfOnTopByCollision(Transform other, ContactPoint2D[] contacts)
    {
        if (contacts == null || contacts.Length == 0)
            return;

        foreach (var c in contacts)
        {
            if (c.normal.y > 0.5f)
            {
                other.SetParent(transform);
                return;
            }
        }
    }

    void TryParentIfOnTopByTrigger(Transform other, Collider otherCollider)
    {
        if (box3D == null)
            return;

        float platformTop = box3D.bounds.max.y;
        float otherCenterY = otherCollider.bounds.center.y;

        if (otherCenterY >= platformTop - 0.1f)
        {
            other.SetParent(transform);
        }
    }

    void TryParentIfOnTopByTrigger2D(Transform other, Collider2D otherCollider)
    {
        if (box2D == null)
            return;

        float platformTop = box2D.bounds.max.y;
        float otherCenterY = otherCollider.bounds.center.y;

        if (otherCenterY >= platformTop - 0.1f)
        {
            other.SetParent(transform);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        if (useTwoPoints && pointA != null && pointB != null)
        {
            Gizmos.DrawSphere(pointA.position, 0.1f);
            Gizmos.DrawSphere(pointB.position, 0.1f);
            Gizmos.DrawLine(pointA.position, pointB.position);
        }
        else if (waypoints != null && waypoints.Length > 0)
        {
            for (int i = 0; i < waypoints.Length; i++)
            {
                if (waypoints[i] == null) continue;
                Gizmos.DrawSphere(waypoints[i].position, 0.1f);
                if (i > 0 && waypoints[i - 1] != null)
                    Gizmos.DrawLine(waypoints[i - 1].position, waypoints[i].position);
            }
        }
    }
}
