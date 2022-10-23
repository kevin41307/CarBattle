using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


[CreateAssetMenu(fileName = "NewAttackAction", menuName = "Custom/NewAttackAction")]
[InlineEditor]
public class AttackAction : ScriptableObject
{
    public List<AttackActionSettings> settings = new List<AttackActionSettings>();
}
