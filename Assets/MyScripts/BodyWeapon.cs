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
    private float finishMuliplier = 3f;

    private Vector3 drawLineStart;
    private Vector3 drawLineEnd;
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
        if (repeatedly) return;
        //if (!other.TryGetComponent(out NetworkObject no)) return;
        if (other.name.Contains(NetPrometeoCarController.playerCarPrefix))
        {
            Vector3 direction = other.transform.position - transform.TransformPoint(Vector3.zero);
            //Vector3 direction = other.transform.position - transform.position;
            drawLineStart = other.transform.position;
            drawLineEnd = transform.position;
            Vector3 force = direction * forceAmount;
            force.y = 0;
            Push(other, force);
            Damage(other, force);
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (!repeatedly) return;
        //if (!other.TryGetComponent(out NetworkObject no)) return;
        if (other.name.Contains(NetPrometeoCarController.playerCarPrefix))
        {
            Vector3 direction = other.transform.position - transform.TransformPoint(Vector3.zero);         
            drawLineStart = other.transform.position;
            drawLineEnd = transform.position;
            Vector3 force = direction * forceAmount;
            force.y = 0;
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
                if (touchedDict[other.gameObject].count > maxRepeatCount) return;
                if (Time.time > touchedDict[other.gameObject].lastTouchTime + defaultActivateTime)
                {
                    Push(other, (force * ((touchedDict[other.gameObject].count == maxRepeatCount) ? finishMuliplier : 1)));
                    Damage(other, direction);
                    //ForceMode.Force ~ 100000f À»°h
                    //ForceMode.Force ~ 150000f À»­¸
                    touchedDict[other.gameObject].lastTouchTime = Time.time;
                    touchedDict[other.gameObject].count += 1;
                }
            }
        }

        //TODO
    }
    private void Damage(Collider other, Vector3 direction)
    {
        if (IsServer && other.TryGetComponent(out Damageable damageable))
        {
            var msg = new Damageable.DamageMessage(OwnerClientId, damagePoint, direction);
            damageable.ApplyDamage(msg);
        }
        //PlayHitVFXClientRpc();
        PlayVFX(vfx_hitPool, other.ClosestPointOnBounds(transform.position), Quaternion.identity);
    }

    private void Push(Collider other, Vector3 force)
    {
        /*
        if(touchedDict.TryGetValue(other.gameObject, out Cache cache))
        {
            PushClientRpc(cache.clientId, force);
        }
        else
        {
            PushClientRpc(other.GetComponent<NetworkObject>().OwnerClientId, force);
        }
        */
        Vector3 pos = transform.position;
        pos.y = 0.5f;


        if (other.TryGetComponent(out Rigidbody rb))
        {

            //rb.AddExplosionForce(force.magnitude, transform.position, radius, 1f, ForceMode.VelocityChange);
            rb.AddForceAtPosition(force, pos, ForceMode.VelocityChange);
        }

        if ((NetworkManager.Singleton as MyNetworkManager).playerCars.TryGetValue(OwnerClientId, out GameObject myCar))
        {
            if (myCar.TryGetComponent(out Rigidbody myRb))
            {
                myRb.AddForceAtPosition(force * 0.8f, pos, ForceMode.VelocityChange);
            }
        }

    }
    [ClientRpc]
    private void PlayHitVFXClientRpc(Vector3 position, Quaternion rotation)
    {
        var vfx = vfx_hitPool.GetNew();
        if (vfx)
        {
            vfx.transform.position = position;
            vfx.transform.rotation = rotation;
        }
    }

    private void PlayVFX<T>(ObjectPooler<T> pool, Vector3 position, Quaternion rotation) where T : MonoBehaviour, ITemped<T>
    {
        var vfx = pool.GetNew();
        if (vfx)
        {
            vfx.transform.position = position;
            vfx.transform.rotation = rotation;
        }
    }
    private void PlayVFX<T>(ObjectPooler<T> pool, Transform parent) where T : MonoBehaviour, ITemped<T>
    {
        pool.GetNew(parent);
    }

    /*
    [ClientRpc]
    private void PushClientRpc(ulong targetId, Vector3 force)
    {
        if((NetworkManager.Singleton as MyNetworkManager).playerCars.TryGetValue( targetId, out GameObject targetCar))
        {
            if(targetId != OwnerClientId && targetCar.TryGetComponent(out Rigidbody rb) )
            {
                Debug.Log("ApplyForce at " + targetCar.name);
                rb.AddForceAtPosition(force, transform.position, ForceMode.VelocityChange);
            }
        }

        if ((NetworkManager.Singleton as MyNetworkManager).playerCars.TryGetValue(OwnerClientId, out GameObject myCar))
        {
            if (myCar.TryGetComponent(out Rigidbody rb))
            {
                rb.AddForceAtPosition(force * 0.5f, transform.position, ForceMode.VelocityChange);
            }
        }

    }
    */
    public void StartDetect()
    {
        PlayVFX(vfx_swingPool, pivot);


        enabled = true;

    }
    public override void Use()
    {
        StartDetect();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(drawLineStart, drawLineEnd);
    }
}
