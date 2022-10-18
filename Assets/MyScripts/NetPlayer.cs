using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class NetPlayer : NetworkBehaviour
{
    [SerializeField] private GameObject carPrefab;

    public override void OnNetworkSpawn()
    {
        Debug.Log("OnNetworkSpawn" + NetworkManager.Singleton.LocalClientId);
        
        base.OnNetworkSpawn();
        if( IsOwner )
        {
            var car = Instantiate(carPrefab);
            car.transform.position = Vector3.zero + Random.insideUnitSphere * 2f + Vector3.up *2f;

            car.GetComponent<NetworkObject>().Spawn();

        }
    }

}
