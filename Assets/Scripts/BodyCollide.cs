using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

public class BodyCollide : MonoBehaviour
{
    CreatureParent ParentComponent;

    private void Start()
    {
        ParentComponent = transform.parent.GetComponent<CreatureParent>();
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("SelectArea"))
        {
            ParentComponent.EnterLine(other);
        }
    }

    public void OnCollisionEnter2D(Collision2D other)
    {
        ParentComponent.HitObject(other);
    }

    public void OnCollisionExit2D(Collision2D other)
    {
        ParentComponent.LeaveObject(other);
    }

    public void OnCollisionStay2D(Collision2D other)
    {
        ParentComponent.HitObject(other);
    }
}
