using UnityEngine;

public class LevelExit : Interactable
{
    protected override void CompleteInteract()
    {
        if (!LevelManager.Instance.HasEnoughMoney)
        {
            StopInteract();
            return;
        }

        LevelManager.Instance.LoadNextLevel();
    }
}
