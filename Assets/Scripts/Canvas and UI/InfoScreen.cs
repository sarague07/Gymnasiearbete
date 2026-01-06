using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InfoScreen : MonoBehaviour
{
    [SerializeField]
    private GameObject infoScreenUI;

    [SerializeField]
    private bool startVisible = false;

    private InputListener inputListener;

    private void Start()
    {
        if (infoScreenUI == null)
        {
            infoScreenUI = GameObject.Find("InfoScreenUI");
        }

        if (infoScreenUI != null)
        {
            infoScreenUI.SetActive(startVisible);
        }

        if (EventSystem.current == null)

        if (infoScreenUI != null && (gameObject == infoScreenUI || transform.IsChildOf(infoScreenUI.transform)))
        {
            EnsureInputListener();
        }
    }

    private void OnDestroy()
    {
        if (inputListener != null)
        {
            Destroy(inputListener.gameObject);
            inputListener = null;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInfoScreen();
        }
    }

    public void ToggleInfoScreen()
    {
        if (infoScreenUI == null) return;
        infoScreenUI.SetActive(!infoScreenUI.activeSelf);
    }

    public void CloseInfoScreen()
    {
        if (infoScreenUI == null) return;
        infoScreenUI.SetActive(false);
    }

    private void EnsureInputListener()
    {
        if (inputListener != null) return;

        var go = new GameObject("InfoScreenInputListener");
        go.transform.SetParent(null);
        inputListener = go.AddComponent<InputListener>();
        inputListener.Initialize(infoScreenUI);
    }

    private class InputListener : MonoBehaviour
    {
        private GameObject ui;

        public void Initialize(GameObject infoScreenUI)
        {
            ui = infoScreenUI;
        }

        private void Update()
        {
            if (ui == null) return;

            if (Input.GetKeyDown(KeyCode.I))
            {
                ui.SetActive(!ui.activeSelf);
            }
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void CreateGlobalInputListener()
    {
        if (GameObject.Find("InfoScreenInputListener_Global") != null) return;
        var go = new GameObject("InfoScreenInputListener_Global");
        DontDestroyOnLoad(go);
        go.AddComponent<GlobalInputListener>();
    }

    private class GlobalInputListener : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                var screens = Resources.FindObjectsOfTypeAll<InfoScreen>();
                foreach (var s in screens)
                {
                    if (s == null) continue;
                    if (s.infoScreenUI == null)
                    {
                        var found = GameObject.Find("InfoScreenUI");
                        if (found != null) s.infoScreenUI = found;
                    }

                    if (s.infoScreenUI != null)
                    {
                        s.infoScreenUI.SetActive(!s.infoScreenUI.activeSelf);
                    }
                }
            }
        }
    }
}