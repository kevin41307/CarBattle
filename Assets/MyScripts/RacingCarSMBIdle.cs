using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kogaine.Helpers;
public class RacingCarSMBIdle : SceneLinkedSMB<NetPrometeoCarController>
{
    public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnSLStateEnter(animator, stateInfo, layerIndex);
        m_MonoBehaviour.AnimEnterIdle();
    }



}
