using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor.Animations;

public class animator : MonoBehaviour
{
    [SerializeField]
    private Animator anim;
    [SerializeField]
    private AnimatorController ac;
    private AnimatorStateTransition transit;

    public void Start() {
        GetComponent<CreatureParent>().animationEvent.AddListener(Blink);
        
        foreach (ChildAnimatorState state_wrapper in ac.layers[0].stateMachine.states)
        {
            if(transit!=null) break;
            AnimatorState state = state_wrapper.state;
            foreach (AnimatorStateTransition transition in state.transitions)
            {
                if(transit!=null) break;
                if (null != transition.destinationState)
                {
                    if(transition.name == "BlinkToDefault")
                    {
                        transit = transition;
                        break;
                    }
                }
            }
        }
    }

    public void Blink(float freq, float duration, bool startOrStop)
    {
        if(startOrStop)
        {
            anim.SetFloat("frequency", freq);
            anim.SetTrigger("Start");
        }
        else
        {
            anim.SetTrigger("Stop");
        }
    }
}
