using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;

public class BodyWeapon : NetworkBehaviour
{
    [SerializeField] private AttackAction attackAction;
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
        Debug.Log("SDfdfsfs");
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

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (!IsServer) return;
        if (repeatedly) return;

        if (other.CompareTag("ConnectedPlayerCar") )
        {
            Debug.Log("Enter other" + other.name);
            enabled = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!IsServer) return;
        if (!repeatedly) return;
        Debug.Log("Stay other" + other.name);
        if (other.CompareTag("ConnectedPlayerCar"))
        {
            ulong otherId = other.GetComponent<NetworkObject>().OwnerClientId;

            if (cache.ContainsKey(otherId) )
            {
                if(cache[otherId].lastTouchTime > Time.time + defaultActivateTime)
                {
                    cache[otherId].count = cache[otherId].count + 1;
                    cache[otherId].lastTouchTime = Time.time;
                }
            }
            else
            {
                cache.Add(otherId,
                new TouchData { lastTouchTime = Time.time, count = 1 });
            }
        }
    }

    public void StartDetect(bool repeatedly)
    {
        this.repeatedly = repeatedly;
        enabled = true;
        
    }


}
