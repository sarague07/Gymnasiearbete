using UnityEngine;
using UnityEngine.SceneManagement;

public class PillsInteraction : MonoBehaviour
{
    public LayerMask RedPillLayer;
    public LayerMask BluePillLayer;
    public LayerMask VoidPillLayer;
    public LayerMask RememberPillLayer;

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

        if ((RedPillLayer.value & (1 << otherLayer)) != 0)
        {
            var collector = Object.FindAnyObjectByType<CapsuleCollector>();
            if (collector != null)
            {
                collector.SaveCapsuleCount();
            }

            SceneManager.LoadScene("Level1");
            return;
        }

        if ((BluePillLayer.value & (1 << otherLayer)) != 0)
        {
            PlayerPrefs.SetInt(CapsulesKey, 0);
            PlayerPrefs.Save();

            SceneManager.LoadScene("Level2");
            return;
        }


        if ((VoidPillLayer.value & (1 << otherLayer)) != 0)
        {
            PlayerPrefs.SetInt(CapsulesKey, 0);
            PlayerPrefs.Save();

            SceneManager.LoadScene("Level3");
            return;
        }

        if ((RememberPillLayer.value & (1 << otherLayer)) != 0)
        {
            PlayerPrefs.SetInt(CapsulesKey, 0);
            PlayerPrefs.Save();

            SceneManager.LoadScene("Level4");
            return;
        }
    }
}
