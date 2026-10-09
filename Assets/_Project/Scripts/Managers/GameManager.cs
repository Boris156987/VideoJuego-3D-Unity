using UnityEngine;
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
    }

    private void Start()
    {
        if (playerHealth == null)
            playerHealth = FindFirstObjectByType<PlayerHealth>();

        if (playerHealth != null)
            playerHealth.Died += Lose;

        if (messageText != null)
            messageText.text = "";

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
            if (messageTimer <= 0f && messageText != null && !GameEnded)
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
        if (GameEnded) return;
        GameEnded = true;
        if (messageText != null) messageText.text = "VICTORIA: escapaste del manicomio";
        Debug.Log("VICTORIA: el jugador escapó del manicomio");
    }

    public void Lose()
    {
        if (GameEnded) return;
        GameEnded = true;
        if (messageText != null) messageText.text = "DERROTA: los zombis te atraparon";
        Debug.Log("DERROTA: el jugador fue atrapado");
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
