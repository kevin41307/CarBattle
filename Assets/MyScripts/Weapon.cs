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
    protected ObjectPooler<TemporaryObj> vfx_swingPool;
    protected TemporaryObj vfxSwingPrefab;
    protected ObjectPooler<TemporaryObj> vfx_hitPool;
    protected TemporaryObj vfxHitPrefab;
    protected Transform pivot = null;
    protected bool repeatedly;
    protected int maxRepeatCount = 5;

    public abstract void Use();

    protected virtual void Awake()
    {
        if (vfxSwingPrefab)
        {
            vfx_swingPool = new ObjectPooler<TemporaryObj>();
            vfx_swingPool.Initialize(2, vfxSwingPrefab);

            foreach (var vfx in vfx_swingPool.instances)
            {
                if (attackAction.settings[index].overrideLifetime > 0)
                    vfx.Setup(attackAction.settings[index].overrideLifetime);
                else
                    vfx.Setup();
            }
        }
        if (vfxHitPrefab)
        {
            vfx_hitPool = new ObjectPooler<TemporaryObj>();
            int count = maxRepeatCount > 1 ? maxRepeatCount : 2;
            vfx_hitPool.Initialize(count, vfxHitPrefab);
            foreach (var vfx in vfx_hitPool.instances)
            {
                vfx.Setup();
            }
        }
    }

    public void Setup(Transform vfxPivotsParent)
    {
        GameObject go = new GameObject($"VFX_attackPivot{index + 1}");
        go.transform.SetParent(vfxPivotsParent);
        go.transform.localPosition = attackAction.settings[index].offsetPosition;
        go.transform.localRotation = Quaternion.Euler(attackAction.settings[index].offsetRotation);
        pivot = go.transform;
        vfxSwingPrefab = attackAction.settings[index].vfxSwingPrefab;
        repeatedly = attackAction.settings[index].repeatedly;
        vfxHitPrefab = attackAction.settings[index].vfxHitPrefab;
        maxRepeatCount = attackAction.settings[index].maxRepeatCount;
    }
}
