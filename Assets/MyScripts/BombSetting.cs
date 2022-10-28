using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


[CreateAssetMenu(fileName = "NewBombSetting", menuName = "Custom/NewBombSetting")]
[InlineEditor]
public class BombSetting : ScriptableObject
{
    [SerializeField] private float forceAmount;
    [SerializeField] private float damagePoint = -10;
    [SerializeField] private GameObject prefab;
}
