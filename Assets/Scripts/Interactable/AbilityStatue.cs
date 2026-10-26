using UnityEngine;

public class AbilityStatue : Interactable
{
    [Header("Ability Info")]
    [SerializeField] AbilitySO _ability;
    [SerializeField] string _abilityName;
    [SerializeField, TextArea] string _description;
    [SerializeField] Sprite _image;

    protected override void CompleteInteract()
    {
        AbilityModal.Instance.Show(_ability, _abilityName, _description, _image);
        StopInteract();
    }
}
