using UnityEngine;

public class LevelExit : Interactable
{
    protected override void CompleteInteract()
    {
        if (!LevelManager.Instance.HasEnoughMoney)
        {
            AbilityModal.Instance.ShowMessage("Getaway Driver", "You need " + LevelManager.Instance.TargetMoney + " coins or I'm not driving off!");
            StopInteract();
            return;
        }

        WinScreen.Instance.Show(LevelManager.Instance.CurrentMoney);

    }
}
