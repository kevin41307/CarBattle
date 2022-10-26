using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private float forceAmount;
    private void OnTriggerEnter(Collider other)
    {
        if(other.name.Contains(NetPrometeoCarController.playerCarPrefix))
        {
            Debug.Log("ApplyForce at " + other.name);
            Rigidbody rb = other.GetComponent<Rigidbody>();
            Vector3 force = (other.transform.position - transform.position) * forceAmount;
            rb.AddForceAtPosition(force, transform.position, ForceMode.Force);
            //ForceMode.Force ~ 100000f À»°h
            //ForceMode.Force ~ 150000f À»­¸
        }
    }
}
