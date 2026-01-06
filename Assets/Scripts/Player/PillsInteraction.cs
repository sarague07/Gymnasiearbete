using UnityEngine;
using UnityEngine.SceneManagement;

public class PillsInteraction : MonoBehaviour
{
    public LayerMask redPillLayer;
    public LayerMask bluePillLayer;

    private const string CapsulesKey = "CapsulesCount";

    private void OnTriggerEnter(Collider other)
    {
        HandlePillCollision(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        HandlePillCollision(collision.gameObject);
    }

    private void HandlePillCollision(GameObject other)
    {
        if (other == null) return;

        int otherLayer = other.layer;

        if ((redPillLayer.value & (1 << otherLayer)) != 0)
        {
            var collector = Object.FindAnyObjectByType<CapsuleCollector>();
            if (collector != null)
            {
                collector.SaveCapsuleCount();
            }

            SceneManager.LoadScene("Scene1");
            return;
        }

        if ((bluePillLayer.value & (1 << otherLayer)) != 0)
        {
            PlayerPrefs.SetInt(CapsulesKey, 0);
            PlayerPrefs.Save();

            SceneManager.LoadScene("Scene0");
            return;
        }
    }
}
