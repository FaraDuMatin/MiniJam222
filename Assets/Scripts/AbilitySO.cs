using UnityEngine;
using System.Collections.Generic;

public enum AbilityKey
{
    Key1,
    Key2,
    Key3,
}

[CreateAssetMenu(fileName = "AbilitySO", menuName = "Scriptable Objects/AbilitySO")]
public class AbilitySO : ScriptableObject
{
    [SerializeField] private List<AbilityKey> _abilityKeys = new List<AbilityKey>();

    public List<AbilityKey> AbilityKeys => _abilityKeys;


    public virtual void ExecuteAbility(PlayerCharacter playerCharacter)
    {
        
    }
}
