using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporaryObj : MonoBehaviour, ITemped<TemporaryObj> 
{
    [HideInInspector]
    public float expiredTime = -1;
    public int poolID { set; get; }
    public ObjectPooler<TemporaryObj> pool { set; get; }
    public bool autoGiveBack = true;
    WaitForSeconds givebackWaitForSeconds;

    private void Awake()
    {
        Setup();
    }

    public void Setup()
    {
        if (TryGetComponent(out ParticleSystem ps))
        {
            expiredTime = ps.main.duration;
            givebackWaitForSeconds = new WaitForSeconds(expiredTime);
        }
    }

    private void OnEnable()
    {
        if (autoGiveBack)
            StartCoroutine(GiveBack());
    }
    public void SetAutoDisableWhenExpired()
    {
        autoGiveBack = true;
    }

    IEnumerator GiveBack()
    {
        yield return givebackWaitForSeconds;
        pool.Free(this);
    }
}