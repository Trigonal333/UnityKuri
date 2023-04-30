using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class Platelet : CreatureParent
{
    protected override void Attack() // Woundを回復
    {
        attackCandidate.RemoveAll(c => c == null);
        if(attackCandidate.Count()>0){
            Collider2D minCol = attackCandidate.OrderBy(collider => Vector2.Distance(collider.ClosestPoint(rigid2D.position), rigid2D.position)).FirstOrDefault();
            var tmp = minCol.gameObject.GetComponent<IDamageable>();
            if(tmp != null) tmp.Attacked(-stat.atk);
        }
    }
}
