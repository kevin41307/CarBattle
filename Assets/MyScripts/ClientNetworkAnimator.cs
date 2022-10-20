using Unity.Netcode.Components;
using Unity.Netcode;
using UnityEngine;


[DisallowMultipleComponent]
public class ClientNetworkAnimator : NetworkAnimator
{
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}
