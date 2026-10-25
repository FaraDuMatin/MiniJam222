using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { private set; get; } 

    [Header("Money Settings")]
    [SerializeField] int _targetMoney = 100;
    int _currentMoney = 0;
    [SerializeField] TMP_Text _moneyText;

    public bool HasEnoughMoney => _currentMoney >= _targetMoney;

    void Awake()
    {
        Instance = this;

        UpdateMoneyVisual();
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    void UpdateMoneyVisual()
    {
        _moneyText.text = _currentMoney + " / " + _targetMoney;
    }
    
    public void AddMoney(int amount)
    {
        _currentMoney += amount;
        UpdateMoneyVisual();
    }

    public void LoadNextLevel()
    {
        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
        // Last level loops back to the first one
        if (nextIndex >= SceneManager.sceneCountInBuildSettings)
            nextIndex = 0;
        SceneManager.LoadScene(nextIndex);
    }
}
