using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CapsuleCollector : MonoBehaviour
{
    [Header("Resets Capsules count to 0")]
    private int Capsules = 0;

    [Header("Capsules: text in Canvas")]
    public TextMeshProUGUI CapsulesText;

    [Header("Proximity collection")]
    public float collectRadius = 1f;

    [Header("Time Check")]
    public float checkInterval = 0f;
    private float checkTimer = 0f;

    [Header("Capsule Effects")]
    [SerializeField] private float healAmount = 10f;
    [SerializeField] private float damageAmount = 10f;

    private const string CapsulesKey = "CapsulesCount";

    private static bool hasLoadedFromSave = false;

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "Level0")
        {
            Capsules = 0;
            hasLoadedFromSave = false;
            PlayerPrefs.SetInt(CapsulesKey, 0);
            PlayerPrefs.Save();
        }
        else if (!hasLoadedFromSave)
        {
            Capsules = PlayerPrefs.GetInt(CapsulesKey, 0);
            hasLoadedFromSave = true;
        }

        UpdateCapsulesText();
    }

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
            if (other == null) continue;

            int healLayer = LayerMask.NameToLayer("Heal");
            int damageLayer = LayerMask.NameToLayer("Damage");
            int objLayer = other.gameObject.layer;

            bool isCapsule = other.CompareTag("Capsules") || objLayer == healLayer || objLayer == damageLayer;
            if (!isCapsule) continue;

            Destroy(other.gameObject);
            Capsules++;

            UpdateCapsulesText();
            Debug.Log(Capsules);

            if (objLayer == healLayer)
            {
                ApplyToPlayer(healAmount, true);
            }
            else if (objLayer == damageLayer)
            {
                ApplyToPlayer(damageAmount, false);
            }
        }
    }

    private void UpdateCapsulesText()
    {
        if (CapsulesText != null)
            CapsulesText.text = "Capsules: " + Capsules.ToString();
    }

    public void SaveCapsuleCount()
    {
        PlayerPrefs.SetInt(CapsulesKey, Capsules);
        PlayerPrefs.Save();
    }

    public void PrepareForSceneChange()
    {
        SaveCapsuleCount();
        hasLoadedFromSave = false;
    }

    private void ApplyToPlayer(float amount, bool isHeal)
    {
        GameObject playerGO = gameObject;
        bool handled = false;

        string[] healMethods = new[] { "Heal", "AddHealth", "ApplyHeal", "ApplyHealth" };
        string[] damageMethods = new[] { "TakeDamage", "ApplyDamage", "Damage" };
        var candidates = isHeal ? healMethods : damageMethods;

        var comp = playerGO.GetComponent("Player") as Component;
        if (comp != null)
        {
            foreach (var methodName in candidates)
            {
                var method = comp.GetType().GetMethod(methodName, new[] { typeof(float) });
                if (method != null)
                {
                    method.Invoke(comp, new object[] { amount });
                    handled = true;
                    break;
                }
            }
        }

        if (!handled)
        {
            var monos = playerGO.GetComponents<MonoBehaviour>();
            foreach (var mb in monos)
            {
                if (mb == null) continue;
                foreach (var methodName in candidates)
                {
                    var method = mb.GetType().GetMethod(methodName, new[] { typeof(float) });
                    if (method != null)
                    {
                        method.Invoke(mb, new object[] { amount });
                        handled = true;
                        break;
                    }
                }
                if (handled) break;
            }
        }

        if (!handled)
        {
            string fallback = isHeal ? "Heal" : "TakeDamage";
            playerGO.SendMessage(fallback, amount, SendMessageOptions.DontRequireReceiver);
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