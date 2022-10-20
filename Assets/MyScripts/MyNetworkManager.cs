using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class MyNetworkManager : NetworkManager
{
    public bool DEBUG_MODE = false;


    private void Awake()
    {
        NetworkObjectPool.Instance.InitializePool();
    }

#if UNITY_EDITOR 
    private void Start()
    {
        if(DEBUG_MODE)
        {
            Debug_StartHost();
        }
    }

    async private void Debug_StartHost()
    {
        // this allows the UnityMultiplayer and UnityMultiplayerRelay scene to work with and without
        // relay features - if the Unity transport is found and is relay protocol then we redirect all the 
        // traffic through the relay, else it just uses a LAN type (UNET) communication.
        if (RelayManager.Instance.IsRelayEnabled)
            await RelayManager.Instance.SetupRelay();

        if (StartHost())
            Logger.Instance.LogInfo("Host started...");
        else
            Logger.Instance.LogInfo("Unable to start host...");

    }

#endif
}
