using Kogaine.Helpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacingCarSMBAttack : SceneLinkedSMB<NetPrometeoCarController>
{

    [Range(0,1)]
    [SerializeField] private float vfxTriggerTime;
    public int combo = 0;
    private bool isTrigger;

    public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        isTrigger = false;
    }

    public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime % 1 >= vfxTriggerTime && !isTrigger)
        {
            m_MonoBehaviour.TriggerAttackVFX(combo);
            isTrigger = true;
        }
        else if (stateInfo.normalizedTime % 1 < vfxTriggerTime && isTrigger) isTrigger = false;
    }

    public override void OnSLTransitionFromStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
    public override void OnSLTransitionToStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
