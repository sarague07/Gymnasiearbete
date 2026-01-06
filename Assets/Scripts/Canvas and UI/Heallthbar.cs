using UnityEngine;
using UnityEngine.UI;

public class Heallthbar : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private RectTransform healthRect;
    [SerializeField] private Image healthImage;

    private Vector2 originalAnchoredPos;
    private Vector2 originalSizeDelta;
    private float originalAnchoredPosY;

    private void Start()
    {
        if (player == null)
        {
            player = Object.FindAnyObjectByType<Player>();
            if (player == null)
                Debug.LogWarning("Heallthbar: No Player found in scene. Assign the Player reference.");
        }

        if (healthRect == null)
        {
            var t = transform.Find("Health");
            if (t != null)
                healthRect = t.GetComponent<RectTransform>();

            if (healthRect == null)
                Debug.LogWarning("Heallthbar: No RectTransform assigned and no child named 'Health' was found.");
        }

        if (healthImage == null && healthRect != null)
        {
            healthImage = healthRect.GetComponent<Image>();
        }

        if (healthRect != null)
        {
     
            originalAnchoredPos = healthRect.anchoredPosition;
            originalSizeDelta = healthRect.sizeDelta;
            originalAnchoredPosY = originalAnchoredPos.y;

            healthRect.pivot = new Vector2(healthRect.pivot.x, 0f);
            healthRect.anchorMin = new Vector2(healthRect.anchorMin.x, 0f);
            healthRect.anchorMax = new Vector2(healthRect.anchorMax.x, 0f);

            var ap = healthRect.anchoredPosition;
            ap.y = originalAnchoredPosY;
            healthRect.anchoredPosition = ap;
        }
    }

    private void Update()
    {
        if (player == null || healthRect == null) return;

        float current = player.CurrentHealth;
        float max = player.MaxHealth;
        float ratio = (max > 0f) ? Mathf.Clamp01(current / max) : 0f;

        var size = healthRect.sizeDelta;
        size.y = originalSizeDelta.y * ratio;
        healthRect.sizeDelta = size;

        var anchored = healthRect.anchoredPosition;
        anchored.y = originalAnchoredPosY;
        healthRect.anchoredPosition = anchored;
    }
}
