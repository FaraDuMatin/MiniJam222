using UnityEngine;

[CreateAssetMenu(fileName = "FelineAgility", menuName = "Scriptable Objects/FelineAgility")]
public class FelineAgilitySO : AbilitySO
{
    [SerializeField] float _jumpMultiplier = 1.3f;
    [SerializeField] float _duration = 5f;

    public override void ExecuteAbility(PlayerCharacter playerCharacter)
    {
        playerCharacter.ApplyJumpBoost(_jumpMultiplier, _duration);
    }
}
