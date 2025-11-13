using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuEvents : MonoBehaviour
{

    private UIDocument _document;

    private Button _startbutton;
    private Button _quitButton;

    private List<Button> _menuButtons = new List<Button>();

    private AudioSource _audioSource;


    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _document = GetComponent<UIDocument>();

        _startbutton = _document.rootVisualElement.Q("StartButton") as Button;
        _startbutton.RegisterCallback<ClickEvent>(OnPlayGameClick);

        _quitButton = _document.rootVisualElement.Q("QuitButton") as Button;
        if (_quitButton != null)
            _quitButton.RegisterCallback<ClickEvent>(OnQuitClick);

        _menuButtons = _document.rootVisualElement.Query<Button>().ToList();
        for (int i = 0; i < _menuButtons.Count; i++)
        {
            _menuButtons[i].RegisterCallback<ClickEvent>(OnAllButtonsClick);
        }
    }

    private void OnDisable()
    {
        _startbutton.UnregisterCallback<ClickEvent>(OnPlayGameClick);

        if (_quitButton != null)
            _quitButton.UnregisterCallback<ClickEvent>(OnQuitClick);

        for (int i = 0; i < _menuButtons.Count; i++)
        {
            _menuButtons[i].UnregisterCallback<ClickEvent>(OnAllButtonsClick);
        }
    }
    private void OnPlayGameClick(ClickEvent evt)
    {
        Debug.Log("You pressed the Start Button");
        SceneManager.LoadScene("Scene0");
    }

    private void OnAllButtonsClick(ClickEvent evt)
    {
        _audioSource.Play();
    }

    private void OnQuitClick(ClickEvent evt)
    {
        Debug.Log("You pressed the Quit Button");

        Application.Quit();
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#endif
    }

}