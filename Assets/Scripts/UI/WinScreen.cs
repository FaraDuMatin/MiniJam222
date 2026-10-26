using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class WinScreen : MonoBehaviour
{
    public static WinScreen Instance { get; private set; }

    [SerializeField] TMP_Text _moneyText;
    [SerializeField] TMP_Text _timeText;
    [SerializeField] string _menuScene = "Scene_MainMenu";

    [SerializeField] SoundSO _winSound;


    void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void Show(int money)
    {
        int seconds = Mathf.FloorToInt(Time.timeSinceLevelLoad);
        _moneyText.text = "Stolen: " + money;
        _timeText.text = "Time: " + (seconds / 60) + ":" + (seconds % 60).ToString("00");

        Time.timeScale = 0f;
        gameObject.SetActive(true);
        if (_winSound != null)
            _winSound.Play();

    }

    public void PlayAgain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(_menuScene);
    }
}
