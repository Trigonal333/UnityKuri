using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animator : MonoBehaviour
{
    [SerializeField]
    private Animator anim;
    
    public void Start() {
        GetComponent<CreatureParent>().animationEvent.AddListener(Blink);
        anim.SetBool("selected", false);
    }

    public void Blink(float freq, float duration, bool startOrStop) // 被ダメージの場合は時間指定、時間指定が無ければ止めるまで永続
    {
        if(startOrStop)
        {
            anim.SetFloat("frequency", freq);
            anim.SetTrigger("Start");
            if(duration>0)
            {
                StartCoroutine(DelayCoroutine(anim, duration));
            }
            else
            {
                anim.SetBool("selected", true);
            }
        }
        else
        {
            anim.SetBool("selected", false);
            anim.SetTrigger("Stop");
        }
    }

    private IEnumerator DelayCoroutine(Animator anim, float duration)
    {
        yield return new WaitForSeconds(duration);
        anim.SetTrigger("Stop");
    }
}
