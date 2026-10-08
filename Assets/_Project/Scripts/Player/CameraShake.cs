using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;

    [Header("Sacudida")]
    [SerializeField] private float hitTrauma = 0.7f;
    [SerializeField] private float decaySpeed = 1.2f;
    [SerializeField] private float maxAngle = 6f;
    [SerializeField] private float frequency = 25f;

    private float trauma;
    private float seed;

    private void Awake()
    {
        seed = Random.value * 100f;
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
        trauma = Mathf.Clamp01(trauma + hitTrauma);
    }

    private void LateUpdate()
    {
        if (trauma <= 0f) return;

        float shake = trauma * trauma;
        float t = Time.time * frequency;

        float pitch = (Mathf.PerlinNoise(seed, t) - 0.5f) * 2f * maxAngle * shake;
        float yaw   = (Mathf.PerlinNoise(seed + 10f, t) - 0.5f) * 2f * maxAngle * shake;
        float roll  = (Mathf.PerlinNoise(seed + 20f, t) - 0.5f) * 2f * maxAngle * shake;

        transform.localRotation *= Quaternion.Euler(pitch, yaw, roll);

        trauma = Mathf.Max(0f, trauma - decaySpeed * Time.deltaTime);
    }
}
