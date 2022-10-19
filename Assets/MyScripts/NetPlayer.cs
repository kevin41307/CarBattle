using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class NetPlayer : NetworkBehaviour
{
    [SerializeField] private GameObject carPrefab;
    private GameObject myCar;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Debug.Log($"OwnerClientId :{OwnerClientId} LocalID:{NetworkManager.Singleton.LocalClientId}");
        transform.name = $"NetPlayer {OwnerClientId}";
        if (!IsOwner) return;
        SpawnMyCarServerRpc();
    }

    [ServerRpc]
    void SpawnMyCarServerRpc()
    {
        if (!IsServer) return;
        myCar = Instantiate(carPrefab);

        myCar.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId);
    }

}
