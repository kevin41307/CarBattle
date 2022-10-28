using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


[CreateAssetMenu(fileName = "NewContinuousBombSetting", menuName = "Custom/NewContinuousBombSetting")]
[InlineEditor]
public class ContinuousBombSetting : ScriptableObject
{
    [SerializeField] private float forceAmount;
    [SerializeField] private float damagePoint = -10;
    [SerializeField] private GameObject prefab;
}
