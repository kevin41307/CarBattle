using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


[CreateAssetMenu(fileName = "NewCollideWeapon", menuName = "Custom/NewCollideWeapon")]
[InlineEditor]
public class CollideWeapon : ScriptableObject
{
    [SerializeField] private GameObject prefab;

    [Header("Override")]
    public float radius;
    public Vector3 size;
    public float forceAmount;
    public float damagePoint = -10;
    public float activeTime = 0.2f;
    public float finishMultiplier = 1;
    [Range(1,5)]
    public int maxRepeatCount = 1;
    
}
