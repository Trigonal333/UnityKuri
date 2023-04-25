using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

public class BodyCollide : MonoBehaviour
{
    UnityEvent<Collider2D> OnEnterLine = new UnityEvent<Collider2D>();
    UnityEvent<Collision2D> OnHitObject = new UnityEvent<Collision2D>();
    UnityEvent<Collision2D> OnLeaveObject = new UnityEvent<Collision2D>();
    CreatureParent ParentComponent;

    private void Start()
    {
        ParentComponent = transform.parent.GetComponent<CreatureParent>();
        OnEnterLine.AddListener(BypassEvent);
        OnHitObject.AddListener(BypassHitEvent);
        OnLeaveObject.AddListener(BypassLeaveEvent);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("SelectArea"))
        {
            OnEnterLine.Invoke(other);
        }
    }

    public void OnCollisionEnter2D(Collision2D other)
    {
        OnHitObject.Invoke(other);
    }

    public void OnCollisionExit2D(Collision2D other)
    {
        OnLeaveObject.Invoke(other);
    }

    public void OnCollisionStay2D(Collision2D other)
    {
        OnHitObject.Invoke(other);
    }

    public void BypassEvent(Collider2D other)
    {
        ParentComponent.EnterLine(other);
    }

    public void BypassHitEvent(Collision2D other)
    {
        ParentComponent.HitObject(other);
    }

    public void BypassLeaveEvent(Collision2D other)
    {
        ParentComponent.LeaveObject(other);
    }
}
