using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Kogaine.Helpers;

public abstract class Weapon : NetworkBehaviour
{
    [SerializeField] protected AttackAction attackAction;
    [Range(0, 5)]
    [SerializeField] protected int index;
    protected ObjectPooler<TemporaryObj> vfx;
    protected TemporaryObj vfxPrefab;
    protected Transform pivot;
    
    public abstract void Use(bool continuously);

    protected virtual void Awake()
    {
        vfx = new ObjectPooler<TemporaryObj>();
        vfx.Initialize(2, vfxPrefab);
    }

    public void Setup(Transform vfxPivotsParent)
    {
        GameObject go = new GameObject($"VFX_attackPivot{index + 1}");
        go.transform.SetParent(vfxPivotsParent);
        go.transform.localPosition = attackAction.settings[index].offsetPosition;
        go.transform.localRotation = Quaternion.Euler(attackAction.settings[index].offsetRotation);
        pivot = go.transform;
        vfxPrefab = attackAction.settings[index].vfxPrefab;
    }
}
