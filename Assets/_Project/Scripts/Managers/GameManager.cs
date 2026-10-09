using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Objetivo")]
    [SerializeField] private int keysRequired = 3;

    [Header("Interfaz")]
    [SerializeField] private TMP_Text keysText;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private float messageDuration = 2.5f;

    [Header("Pantalla final")]
    [SerializeField] private GameObject endPanel;
    [SerializeField] private TMP_Text endTitle;
    [SerializeField] private string menuSceneName = "MenuPrincipal";

    [SerializeField] private PlayerHealth playerHealth;

    public int KeysCollected { get; private set; }
    public bool HasAllKeys => KeysCollected >= keysRequired;
    public bool GameEnded { get; private set; }

    private float messageTimer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Time.timeScale = 1f;
    }

    private void Start()
    {
        if (playerHealth == null)
            playerHealth = FindFirstObjectByType<PlayerHealth>();

        if (playerHealth != null)
            playerHealth.Died += Lose;

        if (messageText != null) messageText.text = "";
        if (endPanel != null) endPanel.SetActive(false);

        UpdateKeysUI();
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
            playerHealth.Died -= Lose;
    }

    private void Update()
    {
        if (messageTimer > 0f)
        {
            messageTimer -= Time.deltaTime;
            if (messageTimer <= 0f && messageText != null)
                messageText.text = "";
        }
    }

    public void AddKey()
    {
        KeysCollected++;
        UpdateKeysUI();
        ShowMessage(HasAllKeys ? "Tienes todas las llaves. Busca la salida" : "Llave recogida");
    }

    public void TryExit()
    {
        if (GameEnded) return;

        if (HasAllKeys)
            Win();
        else
            ShowMessage($"La salida está bloqueada. Faltan {keysRequired - KeysCollected} llaves");
    }

    public void Win()
    {
        EndGame("VICTORIA\nEscapaste del manicomio");
    }

    public void Lose()
    {
        EndGame("DERROTA\nLos zombis te atraparon");
    }

    private void EndGame(string title)
    {
        if (GameEnded) return;
        GameEnded = true;

        if (messageText != null) messageText.text = "";
        if (endTitle != null) endTitle.text = title;
        if (endPanel != null) endPanel.SetActive(true);

        var controller = FindFirstObjectByType<FirstPersonController>();
        if (controller != null) controller.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }

    private void ShowMessage(string text)
    {
        if (messageText == null) return;
        messageText.text = text;
        messageTimer = messageDuration;
    }

    private void UpdateKeysUI()
    {
        if (keysText != null)
            keysText.text = $"Llaves: {KeysCollected}/{keysRequired}";
    }
}
