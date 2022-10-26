using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class Health : NetworkBehaviour, IMessageReceiver
{
    private float maxHp = 100;
    private float currentHp;

    private void Start()
    {
        currentHp = maxHp;
    }

    public void SetMaxHp(float hp)
    {
        maxHp = hp;
    }

    public void SetCurrentHp(float hp)
    {
        currentHp = hp;
    }

    public void ChangeCurrentHp(float amount)
    {
        
        if (IsServer)
        { 
            currentHp += amount;
        }
        else if(!IsServer && !IsClient) // LOCAL
        {
            currentHp += amount;
        }
        Debug.Log("AfterChangeCurrentHp" + currentHp);
    }

    public void OnReceiveMessage(MessageType type, object msg)
    {
        switch (type)
        {
            case MessageType.DAMAGED:
                var data = msg as Damageable.DamageMessage;
                ChangeCurrentHp(data.amount);

                break;
            default:
                Debug.Log("Not defined message type: " + type.ToString());
                break;
        }
    }
}
