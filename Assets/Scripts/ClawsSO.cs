using UnityEngine;

[CreateAssetMenu(fileName = "Claws", menuName = "Scriptable Objects/Claws")]
public class ClawsSO : AbilitySO
{
    [SerializeField] float _duration = 10f;

    public override void ExecuteAbility(PlayerCharacter playerCharacter)
    {
        playerCharacter.ApplyClaws(_duration);
    }
}
