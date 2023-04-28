using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

public class AttackArea : MonoBehaviour
{
    CreatureParent ParentComponent;

    private void Start()
    {
        ParentComponent = transform.parent.GetComponent<CreatureParent>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        ParentComponent.AddCandidate(other);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        ParentComponent.RemoveCandidate(other);
    }
}
