using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 障害物本体のクラス
public class Leaf : MonoBehaviour
{
    public GameObject warn;

    private float speed = 30f;
    private float startTime = 2f; // 移動開始時間
    private float reverseTime;
    private float disappearTime;
    private float elapsedTime;
    private float damage = 20f;

    DestroyEvent destroyEvent;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        reverseTime = startTime + 0.3f; // ある程度進んだら逆に進む
        disappearTime = reverseTime + 0.5f; // 画面外に戻ったら消滅
        var tmp = Instantiate(warn, transform.position + Vector2.up.Rotate(transform.rotation.eulerAngles.z).ToVec3() * 5, Quaternion.identity, this.transform.parent) as GameObject;
        yield return new WaitForSeconds(startTime); // 出現前に警告を表示
        Destroy(tmp);
    }

    // Update is called once per frame
    void Update()
    {
        if(elapsedTime>disappearTime)
        {
            destroyEvent.Invoke(gameObject.GetInstanceID());
        }
        else if(elapsedTime>reverseTime)
        {
            transform.position -= Vector2.up.Rotate(transform.rotation.eulerAngles.z).ToVec3() * speed * Time.deltaTime;
        }
        else if(elapsedTime>startTime)
        {
            transform.position += Vector2.up.Rotate(transform.rotation.eulerAngles.z).ToVec3() * speed * Time.deltaTime;
        }
        elapsedTime += Time.deltaTime;
    }

    public Leaf RegisterEvent(in DestroyEvent _destroyEvent)
    {
        destroyEvent=_destroyEvent;
        return this;
    }

    public void OnCollisionEnter2D(Collision2D other)
    {
        other.gameObject.GetComponentInParent<CreatureParent>().Attacked(damage);
    }
}
