using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Gun attachments")]
    public Transform bulletSpawnPoint;
    public GameObject bulletPrefab;
    public float bulletSpeed = 10f;

    [Header("Audio")]
    public AudioClip shotClip;
    [Range(0f, 1f)] public float shotVolume = 1f;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            var bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            var rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
                rb.linearVelocity = bulletSpawnPoint.forward * bulletSpeed;

            if (shotClip != null)
                audioSource.PlayOneShot(shotClip, shotVolume);
        }
    }
}