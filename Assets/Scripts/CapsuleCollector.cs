using TMPro;
using UnityEngine;

public class CapsuleCollector : MonoBehaviour
{

    [Header("Resets Capsules count to 0")]
    private int Capsules = 0;

    [Header("Capsules: text in Canvas")]
    public TextMeshProUGUI CapsulesText;

    [Header("Proximity collection")]
    public float collectRadius = 2f;

    [Header("Time Check")]
    public float checkInterval = 0f;
    private float checkTimer = 0f;

    private void Update()
    {
        if (checkInterval > 0f)
        {
            checkTimer -= Time.deltaTime;
            if (checkTimer > 0f) return;
            checkTimer = checkInterval;
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, collectRadius);
        for (int i = 0; i < hits.Length; i++)
        {
            Collider other = hits[i];
            if (other != null && other.CompareTag("Capsules"))
            {
                Destroy(other.gameObject);
                Capsules++;
                if (CapsulesText != null)
                    CapsulesText.text = "Capsules: " + Capsules.ToString();
                Debug.Log(Capsules);
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 0.5f, 1f, 0.25f);
        Gizmos.DrawSphere(transform.position, collectRadius);
        Gizmos.color = new Color(0f, 0.8f, 1f, 1f);
        Gizmos.DrawWireSphere(transform.position, collectRadius);
    }
}
