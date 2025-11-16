using TMPro;
using UnityEngine;

public class GemCollector : MonoBehaviour
{

    [Header("Resets Gem count to 0")]
    private int Gem = 0;

    [Header("Gem: text in Canvas")]
    public TextMeshProUGUI GemsText;

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
            if (other != null && other.CompareTag("Gem"))
            {
                Destroy(other.gameObject);
                Gem++;
                if (GemsText != null)
                    GemsText.text = "Gems: " + Gem.ToString();
                Debug.Log(Gem);
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
