using UnityEngine;

[CreateAssetMenu(fileName = "CatForm", menuName = "Scriptable Objects/CatForm")]
public class CatFormSO : AbilitySO
{
    public override void ExecuteAbility(PlayerCharacter playerCharacter)
    {
        playerCharacter.TransformToCat(true);
    }
}
