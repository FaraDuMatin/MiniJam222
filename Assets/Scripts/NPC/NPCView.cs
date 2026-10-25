using UnityEngine;

public class NPCView : MonoBehaviour
{
    PlayerCharacter _characterTarget;
    [SerializeField] SpriteRenderer _viewRenderer;
    [SerializeField] LayerMask _obstacleMask;
    [SerializeField] float _viewDistance = 5f;
    bool _flipped = false;
    bool _canSeeIt = false;

    public PlayerCharacter CharacterTarget => _characterTarget;
    public bool CanSeeIt => _canSeeIt;

    void Update()
    {
        float viewDistance = ViewDistance();
        _viewRenderer.size = new Vector2(viewDistance * 2f, _viewRenderer.size.y);

        if(_characterTarget == null)
            return;
        if(_canSeeIt) // So that even if we change as a cat and they saw us then we still remain visible until we leave their view
            return;
        
        _canSeeIt = !_characterTarget.IsInCat;
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.attachedRigidbody != null && other.attachedRigidbody.CompareTag("Player"))
        {
            _characterTarget = other.attachedRigidbody.GetComponent<PlayerCharacter>();
            if(_characterTarget != null)
                _canSeeIt = !_characterTarget.IsInCat;
        }
    }

    void OnTriggerExit2D(Collider2D other) 
    {
        if(other.attachedRigidbody != null && other.attachedRigidbody.CompareTag("Player"))
        {
            _characterTarget = null;
            _canSeeIt = false;
        }
    }

    public void Flip(bool flip)
    {
        this.transform.localScale = new Vector3(flip ? -1 : 1, 1, 1);
        _flipped = flip;
    }

    float ViewDistance()
    {
        Vector2 origin = this.transform.position;
        Vector2 direction = _flipped ? Vector2.left : Vector2.right;
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, _viewDistance, _obstacleMask);
        if(hit.collider != null)
            return hit.distance;
        return _viewDistance;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector2 origin = this.transform.position;
        Vector2 direction = _flipped ? Vector2.left : Vector2.right;
        Gizmos.DrawLine(origin, origin + direction * _viewDistance);
    }
}
