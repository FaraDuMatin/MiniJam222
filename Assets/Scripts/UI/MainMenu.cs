using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject _controlsPanel;
    [SerializeField] string _gameScene = "Scene_Heist";

    public void Play()
    {
        SceneManager.LoadScene(_gameScene);
    }

    public void ShowControls(bool show)
    {
        _controlsPanel.SetActive(show);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
