using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class Wound : MonoBehaviour, IDamageable
{
    private float hp = 500;
    private float maximum = 2000;
    private float reachedMax;
    DestroyOrRemoveEvent destroyEvent;
    public Slider hpBar;
    public Collider2D coll;
    public SpriteRenderer sprite;
    // Start is called before the first frame update
    void Start()
    {
        sprite.color = Utility.ConvertHEXA2Color("#8D5AC0");
        hpBar.maxValue = maximum;
        hpBar.value = hp;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LateUpdate()
    {
        if(hp>maximum) // 回復しきったら消滅
        {
            destroyEvent.Invoke(gameObject.GetInstanceID(), true);
        }
        else if(hp<0) // ダメージを受けたら障害物として残る
        {
            gameObject.tag = "Obstacle";
            coll.isTrigger = false;
            coll.forceSendLayers =  LayerMask.GetMask("Ally");
            hpBar.gameObject.SetActive(false);
            sprite.color = Utility.ConvertHEXA2Color("#FCFD00");
            destroyEvent.Invoke(gameObject.GetInstanceID(), false);
        }
    }

    public Wound RegisterEvent(in DestroyOrRemoveEvent _destroyEvent)
    {
        destroyEvent=_destroyEvent;
        return this;
    }

    public void Attacked(float atk)
    {
        hp -= atk;
        hpBar.value = hp;
    }
}
