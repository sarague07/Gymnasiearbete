using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RespawnUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject respawnCanvas; 
    [SerializeField] private Button lastCheckpointButton;
    [SerializeField] private Button startButton;
    [SerializeField] private Button mainMenuButton;

    [Header("Scenes")]
    [SerializeField] private string mainMenuScene = "MainMenu";

    private Player player;

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
        if (player == null)

        if (lastCheckpointButton != null)
            lastCheckpointButton.onClick.AddListener(OnLastCheckpointPressed);
     
        if (startButton != null)
            startButton.onClick.AddListener(OnStartPressed);
     

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(OnMainMenuPressed);
    
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

    private void OnLastCheckpointPressed()
    {
       

        if (Checkpoint.TryGetLastCheckpointPosition(out Vector3 checkpointPos))
        {

            var controller = player.GetComponent<CharacterController>();
            if (controller != null)
                controller.enabled = false;

            player.transform.position = checkpointPos;

            if (controller != null)
                controller.enabled = true;
        }
        else
        {
            player.RespawnToInitial();
        }

        CloseUI();
    }

    private void OnStartPressed()
    {

        player.RespawnToInitial();

        CloseUI();
    }

    private void OnMainMenuPressed()
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
}
