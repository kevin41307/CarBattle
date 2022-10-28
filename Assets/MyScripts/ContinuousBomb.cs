using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinuousBomb : MonoBehaviour
{
    public class Cache
    {
        public float lastTouchTime;
        public int count;
    }
    [SerializeField] private float forceAmount = 2;
    [SerializeField] private float damagePoint = -10;
    [SerializeField] private float finishMuliplier = 10f;

    private Dictionary<GameObject, Cache> touchedDict = new Dictionary<GameObject, Cache>();
    private const float defaultActivateTime = 0.2f;
    public int maxTouchCount = 5;
    public float radius = 2f;

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        touchedDict.Clear();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.name.Contains(NetPrometeoCarController.playerCarPrefix))
        {
            Vector3 direction = other.transform.position - transform.position;
            Vector3 force = direction * forceAmount;
            if(!touchedDict.ContainsKey(other.gameObject))
            {
                touchedDict.Add(other.gameObject, new Cache { lastTouchTime = Time.time, count = 1 });
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
            var msg = new Damageable.DamageMessage(0, damagePoint, direction);
            damageable.ApplyDamage(msg);
        }
    }

    public void Push(Collider other, Vector3 force)
    {
        if (other.TryGetComponent(out Rigidbody rb))
        {
            Debug.Log("ApplyForce at " + other.name);
            rb.AddExplosionForce(force.magnitude, transform.position, radius, 1f, ForceMode.VelocityChange);
        }
    }
}
