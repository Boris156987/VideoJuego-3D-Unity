using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private AudioClip hurtClip;
    [SerializeField] private AudioClip deathClip;

    private void Awake()
    {
        if (playerHealth == null)
            playerHealth = GetComponent<PlayerHealth>();
    }

    private void OnEnable()
    {
        if (playerHealth == null) return;
        playerHealth.Damaged += OnDamaged;
        playerHealth.Died += OnDied;
    }

    private void OnDisable()
    {
        if (playerHealth == null) return;
        playerHealth.Damaged -= OnDamaged;
        playerHealth.Died -= OnDied;
    }

    private void OnDamaged()
    {
        if (!playerHealth.IsDead && AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(hurtClip);
    }

    private void OnDied()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(deathClip);
    }
}
