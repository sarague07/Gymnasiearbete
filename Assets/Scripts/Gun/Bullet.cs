using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float life = 3f;

    private void Awake()
    {
        Destroy(gameObject, life);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject);
        }

        Destroy(gameObject);
    }
}
