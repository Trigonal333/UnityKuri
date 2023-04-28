using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaf : MonoBehaviour
{
    private float speed = 30f;
    public GameObject warn;
    private float startTime = 2f;
    private float reverseTime;
    private float disappearTime;
    private float elapsedTime;
    DestroyEvent destroyEvent;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        reverseTime = startTime + 1f;
        disappearTime = reverseTime + 1.2f;
        var tmp = Instantiate(warn, transform.position + Vector2.up.Rotate(transform.rotation.eulerAngles.z).ToVec3() * 5, Quaternion.identity, this.transform.parent) as GameObject;
        yield return new WaitForSeconds(startTime);
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
        other.gameObject.GetComponentInParent<CreatureParent>().Attacked(30f);
    }
}
