using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using System;

public class BOID
{
    float searchRadius;
    float minDist, maxDist;
    float separateWeight, cohesionWeight, attackWeight, runWeight;
    int maximumGroup;

    public BOID(float _searchRadius, float _minDist, float _maxDist, float _separateWeight, float _cohesionWeight, float _attackWeight, int _maximumGroup = 30)
    {
        searchRadius = _searchRadius;
        minDist = _minDist;
        maxDist = _maxDist;
        separateWeight = _separateWeight;
        cohesionWeight = _cohesionWeight;
        attackWeight = _attackWeight;
        maximumGroup = _maximumGroup;
    }

    public Vector3 Move(GameObject subject, string ally, string enemy, string target)
    {
        int layerMask = LayerMask.GetMask(ally, enemy, target);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(subject.transform.position, searchRadius, layerMask);
        Dictionary<string, List<(Collider2D, float)>> colliderDict = new Dictionary<string, List<(Collider2D, float)>>();
        Dictionary<string, Tuple<int, Vector3, Vector3>> stats = new Dictionary<string, Tuple<int, Vector3, Vector3>>();// count, center, closest
        foreach(Collider2D collider in colliders)
        {
            string layerName = LayerMask.LayerToName(collider.gameObject.layer);
            if (!colliderDict.ContainsKey(layerName))
            {
                colliderDict[layerName] = new List<(Collider2D, float)>();
            }
            colliderDict[layerName].Add((collider, (subject.transform.position - collider.transform.position).sqrMagnitude));
        }

        foreach(string key in colliderDict.Keys)
        {
            colliderDict[key].Sort((x, y) => x.Item2.CompareTo(y.Item2));
            Vector3 tmp = Vector3.zero;
            foreach(var v in colliderDict[key])
            {
                tmp += v.Item1.transform.position;
            }
            stats.Add(key, Tuple.Create(colliderDict[key].Count, tmp/colliderDict[key].Count, colliderDict[key][0].Item1.transform.position));
        }


        Vector3 retval = Vector3.zero;

        bool result = false; // 攻撃のために止まっているのか障害物などの関係で止まってるのかのフラグ, true=移動済みか攻撃用に待機 false=障害物とかで動かないだけ
        if(stats.ContainsKey(target))
        {
            result = AttackObj(ref retval, subject.transform.position , colliderDict[target][0].Item1.transform.position);
        }
        
        if(stats.ContainsKey(enemy) && !result)
        {
            if(stats[ally].Item1*3>stats[enemy].Item1*2)
            {
                result = AttackEnemy(ref retval, subject.transform.position, colliderDict[enemy][0].Item1.transform.position);
            }
            else
            {
                result = Run(ref retval, subject.transform.position, stats[enemy].Item2);
            }
        }

        if(stats.ContainsKey(ally) && !result)
        {
            result = Separate(ref retval, subject.transform.position, colliderDict[ally][0].Item1.transform.position);
            // Cohesion(retval, subject.transform.position, stats[ally].Item2);
        }

        if(!result)
        {
            Isolated(ref retval);
        }

        return retval;
    }

    bool AttackObj(ref Vector3 retval, Vector3 subject, Vector3 nearestObj)
    {
        retval = Vector3.zero;
        RaycastHit2D tmp = Physics2D.CircleCast(subject.ToVec2()+(nearestObj-subject).ToVec2().normalized*AllyManager.spaceForMulti*2, AllyManager.spaceForMulti*0.8f, (nearestObj-subject).ToVec2(), 0, LayerMask.GetMask("Enemy"));

        if((subject-nearestObj).magnitude > AllyManager.spaceForMulti * 2)
        {
            if(!tmp.collider){
                retval += (nearestObj-subject) * attackWeight;
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return true;
        }

    }

    bool AttackEnemy(ref Vector3 retval, Vector3 subject, Vector3 nearestEnemy)
    {
        retval = Vector3.zero;
        RaycastHit2D tmp = Physics2D.CircleCast(subject.ToVec2()+(nearestEnemy-subject).ToVec2().normalized*AllyManager.spaceForMulti*2, AllyManager.spaceForMulti*0.8f, (nearestEnemy-subject).ToVec2(), 0, LayerMask.GetMask("Enemy"));
        
        if((subject-nearestEnemy).magnitude > AllyManager.spaceForMulti * 2)
        {
            if(!tmp.collider){
                retval += (nearestEnemy-subject) * attackWeight;
                return true;
            }
            else 
            {
                return false;
            }
        }
        else
        {
            return true;
        }
    }

    bool Run(ref Vector3 retval, Vector3 subject, Vector3 enemyCenter)
    {
        retval += (enemyCenter-subject) * attackWeight;
        return true;
    }

    bool Separate(ref Vector3 retval, Vector3 subject, Vector3 nearestAlly)
    {
        float dist = (subject - nearestAlly).magnitude;
        if(dist<minDist || dist>maxDist)
        {
            retval += (nearestAlly-subject).normalized * separateWeight;
            retval = dist > minDist ? retval : retval * -1;
        }
        return true;
    }

    bool Cohesion(ref Vector3 retval, Vector3 subject, Vector3 allyCenter)
    {
        float dist = (subject - allyCenter).magnitude;
        if(dist<minDist || dist>maxDist)
        {
            retval += (allyCenter-subject).normalized * separateWeight;
            retval = dist > minDist ? retval : retval * -1;
        }
        return true;
    }

    void Isolated(ref Vector3 retval)
    {
        retval = Vector2.right.Rotate(UnityEngine.Random.value*2*Mathf.PI) * maxDist;
    }
}
