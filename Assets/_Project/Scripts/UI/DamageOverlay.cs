using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DamageOverlay : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;

    [Header("Destello al recibir daño")]
    [SerializeField] private float flashAlpha = 0.5f;
    [SerializeField] private float fadeSpeed = 1.5f;

    [Header("Vida baja")]
    [SerializeField] private float lowHealthThreshold = 0.5f;
    [SerializeField] private float lowHealthMaxAlpha = 0.3f;

    private Image image;
    private float flash;

    private void Awake()
    {
        image = GetComponent<Image>();
        image.raycastTarget = false;
        SetAlpha(0f);
    }

    private void OnEnable()
    {
        if (playerHealth == null)
            playerHealth = FindFirstObjectByType<PlayerHealth>();

        if (playerHealth != null)
            playerHealth.Damaged += OnDamaged;
    }

    private void OnDisable()
    {
        if (playerHealth != null)
            playerHealth.Damaged -= OnDamaged;
    }

    private void OnDamaged()
    {
        flash = flashAlpha;
    }

    private void Update()
    {
        flash = Mathf.MoveTowards(flash, 0f, fadeSpeed * Time.deltaTime);

        float lowHealthAlpha = 0f;
        if (playerHealth != null && playerHealth.HealthRatio < lowHealthThreshold)
        {
            float t = playerHealth.HealthRatio / lowHealthThreshold;
            lowHealthAlpha = Mathf.Lerp(lowHealthMaxAlpha, 0f, t);
        }

        SetAlpha(Mathf.Max(flash, lowHealthAlpha));
    }

    private void SetAlpha(float alpha)
    {
        image.color = new Color(0.7f, 0f, 0f, alpha);
    }
}
