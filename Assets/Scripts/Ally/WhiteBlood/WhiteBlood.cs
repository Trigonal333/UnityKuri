using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class WhiteBlood : CreatureParent
{
    public int maxBuffNum = 10;
    public float maxBuffMul = 3f;
    protected override void Attack()
    {
        attackCandidate.RemoveAll(c => c == null); // 攻撃時周囲のRedBloodの数に応じて攻撃力アップ
        if(attackCandidate.Count()>0){
            Collider2D minCol = attackCandidate.OrderBy(collider => Vector2.Distance(collider.ClosestPoint(rigid2D.position), rigid2D.position)).FirstOrDefault();
            float buff = 1.0f;
            buff = Mathf.SmoothStep(1.0f, maxBuffMul, Mathf.Clamp(DetectAround("RedBlood(Clone)", 3.0f), 0f, maxBuffNum)/maxBuffNum);
            minCol.gameObject.GetComponentInParent<CreatureParent>().Attacked(stat.atk*buff);
        }
    }
}
