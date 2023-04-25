using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

public class AttackArea : MonoBehaviour
{
    UnityEvent<Collider2D> OnEnterAttackRange = new UnityEvent<Collider2D>();
    UnityEvent<Collider2D> OnExitAttackRange = new UnityEvent<Collider2D>();
    CreatureParent ParentComponent;

    private void Start()
    {
        ParentComponent = transform.parent.GetComponent<CreatureParent>();
        OnEnterAttackRange.AddListener(BypassEnterEvent);
        OnExitAttackRange.AddListener(BypassExitEvent);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        OnEnterAttackRange.Invoke(other);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        OnExitAttackRange.Invoke(other);
    }
    
    public void BypassEnterEvent(Collider2D other)
    {
        ParentComponent.AddCandidate(other);
    }

    public void BypassExitEvent(Collider2D other)
    {
        ParentComponent.RemoveCandidate(other);
    }
}
