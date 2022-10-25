using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BodyWeapon : NetworkBehaviour
{
    [SerializeField] private AttackAction attackAction;
    [Range(0,5)]
    [SerializeField] private int index;
    private bool repeatedly = false;
    private float lastActivateTime = 0;
    private const float defaultActivateTime = 0.2f;
    private Collider myCollider;
    public class TouchData
    {
        public float lastTouchTime = 0;
        public int count = 0;
    }

    Dictionary<ulong, TouchData> cache = new Dictionary<ulong, TouchData>();

    private void Awake()
    {
        myCollider = GetComponent<Collider>();
    }
    private void OnEnable()
    {
        myCollider.enabled = true;
        lastActivateTime = Time.time;
    }

    private void OnDisable()
    {
        myCollider.enabled = false;
        lastActivateTime = 0f;
        cache.Clear();
    }

    private void Update()
    {

        if (!repeatedly)
        {
            if(Time.time > lastActivateTime + defaultActivateTime )
            {
                enabled = false;
            }
        }
        else
        {
            if (Time.time > lastActivateTime + attackAction.settings[index].activateTime)
            {
                enabled = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if (!IsServer) return;
        if (repeatedly) return;

        if (other.TryGetComponent(out NetworkObject no) )
        {
            if(no.OwnerClientId == OwnerClientId)
            {
                Debug.Log("Enter other" + other.name);
                enabled = false;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!IsServer) return;
        if (!repeatedly) return;
        Debug.Log("Stay other" + other.name);
        if (other.TryGetComponent(out NetworkObject no))
        {
            ulong otherId = no.OwnerClientId;
           
            if (cache.ContainsKey(otherId) )
            {
                int count = cache[otherId].count;
                if (count > attackAction.settings[index].maxRepeatCount) return;

                if (Time.time > cache[otherId].lastTouchTime + defaultActivateTime)
                {

                    cache[otherId].count = cache[otherId].count + 1;
                    cache[otherId].lastTouchTime = Time.time;
                    Debug.Log("Stay other" + other.name + cache[otherId].count);
                }
            }
            else
            {
                cache.Add(otherId, new TouchData { lastTouchTime = Time.time, count = 1 });
            }
        }
    }

    public void StartDetect(bool repeatedly)
    {
        this.repeatedly = repeatedly;
        enabled = true;  
    }
    public void StartDetectForAWhile()
    {
        //TODO
        enabled = true;
        repeatedly = true;

    }

}
