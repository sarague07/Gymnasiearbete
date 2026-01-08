using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RespawnUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject respawnCanvas;
    [SerializeField] private Button startButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button ButtonButton;

    [Header("Scenes")]
    [SerializeField] private string mainMenuScene = "MainMenu";

    private Player player;
    private Vector3 playerInitialPosition;
    private Quaternion playerInitialRotation; 

    private void Awake()
    {
        if (respawnCanvas == null)
            respawnCanvas = gameObject;

        if (respawnCanvas != null)
            respawnCanvas.SetActive(false);
    }

    private void Start()
    {
        player = Object.FindFirstObjectByType<Player>();

        playerInitialPosition = player.transform.position;
        playerInitialRotation = player.transform.rotation;

        if (startButton != null)
            startButton.onClick.AddListener(OnStartPressed);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(OnMainMenuPressed);

        if (ButtonButton != null)
            ButtonButton.onClick.AddListener(OnButtonButtonPressed);

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ToggleRespawnUI();
        }
    }

    private void ToggleRespawnUI()
    {
        if (respawnCanvas == null) return;

        bool isActive = respawnCanvas.activeSelf;
        respawnCanvas.SetActive(!isActive);

        if (!isActive)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void OnStartPressed()
    {
        if (player != null)
        {
            TeleportPlayerToInitialPosition();
        }

        CloseUI();
    }

    private void OnMainMenuPressed()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene(mainMenuScene);
    }

    private void OnButtonButtonPressed()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene(mainMenuScene);
    }

    private void CloseUI()
    {
        if (respawnCanvas == null) return;
        respawnCanvas.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void TeleportPlayerToInitialPosition()
    {
        if (player == null) return;

        player.transform.position = playerInitialPosition;
        player.transform.rotation = playerInitialRotation;

        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        CharacterController controller = player.GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.enabled = false;
            player.transform.position = playerInitialPosition;
            player.transform.rotation = playerInitialRotation;
            controller.enabled = true;
        }

        Debug.Log("Player teleported to initial position: " + playerInitialPosition);
    }
}