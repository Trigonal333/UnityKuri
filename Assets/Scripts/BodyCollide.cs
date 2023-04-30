using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

public class BodyCollide : MonoBehaviour, IDamageable
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
        if (other.transform.CompareTag("Ally") || other.transform.CompareTag("Enemy"))
        ParentComponent.HitObject(other);
    }

    public void OnCollisionExit2D(Collision2D other)
    {
        if (other.transform.CompareTag("Ally") || other.transform.CompareTag("Enemy"))
        ParentComponent.LeaveObject(other);
    }

    public void OnCollisionStay2D(Collision2D other)
    {
        if (other.transform.CompareTag("Ally") || other.transform.CompareTag("Enemy"))
        ParentComponent.HitObject(other);
    }
    
    public void Attacked(float atk) // 攻撃を「受けた」関数、防御力とか追加したい時用
    {
        ParentComponent.Attacked(atk);
    }
}
