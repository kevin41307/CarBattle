using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private float forceAmount;
    [SerializeField] private float damagePoint = -10;
    private void OnTriggerEnter(Collider other)
    {
        if(other.name.Contains(NetPrometeoCarController.playerCarPrefix))
        {
            Vector3 direction = other.transform.position - transform.position;
            Vector3 force = direction * forceAmount;
            if (other.TryGetComponent(out Rigidbody rb))
            {
                Debug.Log("ApplyForce at " + other.name);
                rb.AddForceAtPosition(force, transform.position, ForceMode.Force);
            }

            if (other.TryGetComponent(out Damageable damageable))
            {
                var msg = new Damageable.DamageMessage(0, damagePoint, direction);
                damageable.ApplyDamage(msg);
            }
            //ForceMode.Force ~ 100000f À»°h
            //ForceMode.Force ~ 150000f À»­¸
        }
    }
}
