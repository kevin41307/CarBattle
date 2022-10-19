using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class NetCar : NetworkBehaviour
{

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        transform.name = $"Car {GetComponent<NetworkObject>().OwnerClientId}";
        if(IsOwner)
        {
            transform.position = Vector3.zero + Random.insideUnitSphere * 10f;
            transform.position = new Vector3(transform.position.x, 3f, transform.position.z);
            var follower = Camera.main.GetComponent<CameraFollow>();
            if(follower)
            {
                follower.Setup(transform);
            }
        }
    }
}
