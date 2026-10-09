using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider))]
public class LevelDoor : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "Nivel1";
    [SerializeField] private AudioClip doorSound;

    private bool used;

    private void OnTriggerEnter(Collider other)
    {
        if (used || !other.CompareTag("Player")) return;
        used = true;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(doorSound);

        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneToLoad);
    }
}
