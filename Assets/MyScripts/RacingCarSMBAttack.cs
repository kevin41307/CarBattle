using Kogaine.Helpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacingCarSMBAttack : SceneLinkedSMB<NetPrometeoCarController>
{
    [SerializeField] private AttackAction attackAction;
    [SerializeField, Range(0, 5)] int index;

    private bool isTrigger;


    public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        isTrigger = false;
    }

    public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime % 1 >= attackAction.settings[index].triggerTime && !isTrigger)
        {
            m_MonoBehaviour.TriggerAttackVFX(attackAction.settings[index].combo);
            isTrigger = true;
        }
        else if (stateInfo.normalizedTime % 1 < attackAction.settings[0].triggerTime && isTrigger) isTrigger = false;
    }
}
