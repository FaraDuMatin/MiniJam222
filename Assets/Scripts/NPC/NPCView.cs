using UnityEngine;

public class NPCView : MonoBehaviour
{
    PlayerCharacter _characterTarget;

    public PlayerCharacter CharacterTarget => _characterTarget;

    void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.attachedRigidbody != null && other.attachedRigidbody.CompareTag("Player"))
        {
            _characterTarget = other.attachedRigidbody.GetComponent<PlayerCharacter>();
        }
    }

    void OnTriggerExit2D(Collider2D other) 
    {
        if(other.attachedRigidbody != null && other.attachedRigidbody.CompareTag("Player"))
        {
            _characterTarget = null;
        }
    }
}
