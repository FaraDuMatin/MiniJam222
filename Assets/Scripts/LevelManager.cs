using UnityEngine;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { private set; get; } 

    [Header("Money Settings")]
    [SerializeField] int _targetMoney = 100;
    int _currentMoney = 0;
    [SerializeField] TMP_Text _moneyText;

    void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        UpdateMoneyVisual();
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
}
