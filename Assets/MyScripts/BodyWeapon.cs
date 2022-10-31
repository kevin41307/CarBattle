using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class BodyWeapon : Weapon
{
    [SerializeField] private float forceAmount = 2.5f;
    [SerializeField] private float damagePoint = -10;
    private float lastActivateTime = 0;
    private const float defaultActivateTime = 0.2f;
    private Collider myCollider;
    private int maxTouchCount = 5;
    [SerializeField] private float finishMuliplier = 4f;
    public class Cache
    {
        public ulong clientId;
        public float lastTouchTime;
        public int count;
    }

    private Dictionary<GameObject, Cache> touchedDict = new Dictionary<GameObject, Cache>();

    protected override void Awake()
    {
        base.Awake();
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
        touchedDict.Clear();
    }

    private void Update()
    {
        if (!IsServer) return;
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
        //if (!other.TryGetComponent(out NetworkObject no)) return;
        if (other.name.Contains(NetPrometeoCarController.playerCarPrefix))
        {
            Vector3 direction = other.transform.position - transform.position;
            Vector3 force = direction * forceAmount;

            Push(other, force);
            Damage(other, force);
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (!IsServer) return;
        if (!repeatedly) return;
        //if (!other.TryGetComponent(out NetworkObject no)) return;
        if (other.name.Contains(NetPrometeoCarController.playerCarPrefix))
        {
            Vector3 direction = other.transform.position - transform.position;
            Vector3 force = direction * forceAmount;
            if (!touchedDict.ContainsKey(other.gameObject))
            {
                touchedDict.Add(other.gameObject, new Cache {
                    clientId = other.GetComponent<NetworkObject>().OwnerClientId,
                    lastTouchTime = Time.time,
                    count = 1 
                });
                Push(other, force);
                Damage(other, direction);
            }
            else
            {
                if (touchedDict[other.gameObject].count > maxTouchCount) return;
                if (Time.time > touchedDict[other.gameObject].lastTouchTime + defaultActivateTime)
                {
                    Push(other, (force * ((touchedDict[other.gameObject].count == maxTouchCount) ? finishMuliplier : 1)));
                    Damage(other, direction);
                    //ForceMode.Force ~ 100000f À»°h
                    //ForceMode.Force ~ 150000f À»­¸
                    touchedDict[other.gameObject].lastTouchTime = Time.time;
                    touchedDict[other.gameObject].count += 1;
                }
            }
        }
    }
    private void Damage(Collider other, Vector3 direction)
    {
        if (other.TryGetComponent(out Damageable damageable))
        {
            var msg = new Damageable.DamageMessage(OwnerClientId, damagePoint, direction);
            damageable.ApplyDamage(msg);
        }
    }

    private void Push(Collider other, Vector3 force)
    {
        if(touchedDict.TryGetValue(other.gameObject, out Cache cache))
        {
            PushClientRpc(cache.clientId, force);
        }
        else
        {
            PushClientRpc(other.GetComponent<NetworkObject>().OwnerClientId, force);
        }
        /*
        if (other.TryGetComponent(out Rigidbody rb))
        {

            //rb.AddExplosionForce(force.magnitude, transform.position, radius, 1f, ForceMode.VelocityChange);
            rb.AddForceAtPosition(force, transform.position, ForceMode.VelocityChange);
        }
        */
    }
    [ClientRpc]
    private void PushClientRpc(ulong targetId, Vector3 force)
    {
        if((NetworkManager.Singleton as MyNetworkManager).playerCars.TryGetValue( targetId, out GameObject targetCar))
        {
            if( targetCar.TryGetComponent(out Rigidbody rb))
            {
                Debug.Log("ApplyForce at " + targetCar.name);
                rb.AddForceAtPosition(force, transform.position, ForceMode.VelocityChange);
            }
        }
        
    }

    public void StartDetect()
    {
        vfx.GetNew(attackAction.settings[index].activateTime, pivot);
        if(IsServer)
        {
            enabled = true;
        }
    }
    public override void Use()
    {
        StartDetect();
    }
}
