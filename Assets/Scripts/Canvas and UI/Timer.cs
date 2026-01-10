using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimerManager : MonoBehaviour
{
    public static TimerManager Instance { get; private set; }

[Header("Timer Settings")]
public float totalTimePlayed = 0f;

[Header("UI References")]
public TextMeshProUGUI timerText;

private bool isInitialized = false;
private bool hasGameStarted = false;

private const string TimeKey = "TotalTimePlayed";
private static bool hasLoadedFromSave = false;

void Awake()
{
    if (Instance == null)
    {
        Instance = this;

        if (transform.parent != null)
        {
            transform.SetParent(null);
        }

        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;

        string currentScene = SceneManager.GetActiveScene().name;

        bool isLevel0 = IsLevel0Scene(currentScene);

        if (isLevel0)
        {
            totalTimePlayed = 0f;
            hasGameStarted = true;
            hasLoadedFromSave = false;

            PlayerPrefs.SetFloat(TimeKey, totalTimePlayed);
            PlayerPrefs.Save();

            Debug.Log("Starting fresh in Level0 - Timer reset to 0");
        }
        else if (!hasLoadedFromSave)
        {
            totalTimePlayed = PlayerPrefs.GetFloat(TimeKey, 0f);
            hasLoadedFromSave = true;
            Debug.Log($"Starting in {currentScene} - Timer loaded: {totalTimePlayed:F2} seconds");
        }
        else
        {
            Debug.Log($"Starting in {currentScene} - Timer continues: {totalTimePlayed:F2} seconds");
        }

        isInitialized = true;
    }
    else
    {
        Destroy(gameObject);
        return;
    }
}

void OnSceneLoaded(Scene scene, LoadSceneMode mode)
{
    Debug.Log($"Scene loaded: {scene.name}");
    Debug.Log($"Total time accumulated: {totalTimePlayed:F2} seconds");

    FindTimerUI();
    UpdateDisplay();
}

void Start()
{
    if (!isInitialized) return;

    FindTimerUI();
    UpdateDisplay();
}

void FindTimerUI()
{
    if (timerText != null && timerText.gameObject != null)
        return;

    string[] possibleNames = { "TimerText", "Timer", "TimeText", "TimeDisplay", "UITimer" };

    foreach (string name in possibleNames)
    {
        GameObject obj = GameObject.Find(name);
        if (obj != null)
        {
            TextMeshProUGUI textComp = obj.GetComponent<TextMeshProUGUI>();
            if (textComp != null)
            {
                timerText = textComp;
                Debug.Log($"Found timer UI: {name}");
                return;
            }
        }
    }

    TextMeshProUGUI[] allTexts = FindObjectsByType<TextMeshProUGUI>(FindObjectsSortMode.None);
    foreach (TextMeshProUGUI text in allTexts)
    {
        if (text.gameObject.activeInHierarchy &&
            (text.name.ToLower().Contains("timer") ||
             text.name.ToLower().Contains("time")))
        {
            timerText = text;
            Debug.Log($"Found timer UI via search: {text.name}");
            return;
        }
    }

    Debug.LogWarning("No timer UI found in this scene");
}

void Update()
{
    if (!isInitialized) return;

    if (hasGameStarted)
    {
        totalTimePlayed += Time.deltaTime;
    }

    UpdateDisplay();

    if (Input.GetKeyDown(KeyCode.R))
    {
        ResetTimer();
    }

    if (Input.GetKeyDown(KeyCode.T))
    {
        DebugTime();
    }
}

void UpdateDisplay()
{
    if (timerText != null)
    {
        int hours = Mathf.FloorToInt(totalTimePlayed / 3600);
        int minutes = Mathf.FloorToInt((totalTimePlayed % 3600) / 60);
        int seconds = Mathf.FloorToInt(totalTimePlayed % 60);

        timerText.text = $"Time: {hours:00}:{minutes:00}:{seconds:00}";
    }
}

public void SaveTimeBeforeSceneChange()
{
    PlayerPrefs.SetFloat(TimeKey, totalTimePlayed);
    PlayerPrefs.Save();
    hasLoadedFromSave = false; 
    Debug.Log($"Saved time before scene change: {totalTimePlayed:F2} seconds");
}

public void DebugTime()
{
    Debug.Log($"=== TIMER DEBUG ===");
    Debug.Log($"Total time: {totalTimePlayed:F2} seconds");
    Debug.Log($"Formatted: {GetFormattedTime()}");
    Debug.Log($"Current scene: {SceneManager.GetActiveScene().name}");
    Debug.Log($"Game started: {hasGameStarted}");
    Debug.Log($"Loaded from save: {hasLoadedFromSave}");
    Debug.Log($"=================");
}

public string GetFormattedTime()
{
    int hours = Mathf.FloorToInt(totalTimePlayed / 3600);
    int minutes = Mathf.FloorToInt((totalTimePlayed % 3600) / 60);
    int seconds = Mathf.FloorToInt(totalTimePlayed % 60);
    return $"{hours:00}:{minutes:00}:{seconds:00}";
}

public void ResetTimer()
{
    totalTimePlayed = 0f;
    PlayerPrefs.SetFloat(TimeKey, totalTimePlayed);
    PlayerPrefs.Save();
    UpdateDisplay();
    Debug.Log("Timer manually reset to 0");
}

public void RestartGame()
{
    totalTimePlayed = 0f;
    hasGameStarted = false;
    hasLoadedFromSave = false;
    PlayerPrefs.SetFloat(TimeKey, totalTimePlayed);
    PlayerPrefs.Save();
    UpdateDisplay();
    Debug.Log("Game restarted - Timer reset to 0");
}

private bool IsLevel0Scene(string sceneName)
{
    return sceneName == "Level0" || sceneName == "level0";
}

void OnDestroy()
{
    if (Instance == this)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Instance = null;
    }
}
}