using UnityEngine;

public class Item : Interactable
{
    [SerializeField] int _moneyValue = 1;

    protected override void CompleteInteract()
    {
        LevelManager.Instance.AddMoney(_moneyValue);
        gameObject.SetActive(false);
    }
}
