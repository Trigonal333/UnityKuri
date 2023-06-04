using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Virus : CreatureParent
{
    public override CreatureParent Initialize()
    {
        stat = Status.Entity[this.name.Replace("(Clone)", "")].Clone();
        VirusManager.virusMovementEvent.AddListener(SetDestination);
        return this;
    }
    public override void SetDestination(Dictionary<int, Vector3> dict)
    {
        if(dict.TryGetValue(gameObject.GetInstanceID(), out destinationVector))
        {
        // Debug.Log(destinationVector);
            if(destinationVector.sqrMagnitude > .01f)
            {
                Move();
                ETA = destinationVector.magnitude / stat.speed;
            }
        }
    }
}
