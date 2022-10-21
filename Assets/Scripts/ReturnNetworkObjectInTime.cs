using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class ReturnNetworkObjectInTime : MonoBehaviour
{
    [SerializeField] GameObject objectPrefab;
    [Header("Set trigger time")]
    [SerializeField] float triggerTime = 0f;
    [Header("or use")]
    [SerializeField] bool usePSDuration = false;
    ParticleSystem ps;


    private void Awake()
    {
        if (usePSDuration)
        {
            ps = this.GetComponent<ParticleSystem>();
            var main = ps.main;
            triggerTime = main.duration;
        }
    }

    private void OnEnable()
    {
        Invoke("ReturnObject", triggerTime);
    }

    private void ReturnObject()
    {
        NetworkObjectPool.Instance.ReturnNetworkObject(GetComponent<NetworkObject>(), gameObject);
    }
}
