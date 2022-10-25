using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Kogaine.Helpers;
using Unity.Netcode.Components;

public class NetPrometeoCarController : PrometeoCarController
{
    [Space, Header("VFXs")]
    [SerializeField] private Transform vfxPivotParent;
    [SerializeField] private GameObject attackVFXPrefab;
    [SerializeField] private AttackAction attackAction;
    private List<Transform> attackPivots = new List<Transform>();

    [SerializeField] private BodyWeapon frontBodyWeapon;
    [SerializeField] private BodyWeapon selfBodyWeapon;


    protected void Awake()
    {
        SceneLinkedSMB<NetPrometeoCarController>.Initialise(anim, this);
        for (int i = 0; i < attackAction.settings.Count; i++)
        {
            GameObject go = new GameObject($"VFX_attackPivot{i+1}");
            go.transform.SetParent(vfxPivotParent);
            go.transform.position = attackAction.settings[i].offsetPosition;
            go.transform.rotation = Quaternion.Euler(attackAction.settings[i].offsetRotation);
            attackPivots.Add(go.transform);
        }
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        transform.name = $"Car {GetComponent<NetworkObject>().OwnerClientId}";
        
        if (IsOwner)
        {
            transform.position = Vector3.zero + Random.insideUnitSphere * 10f;
            transform.position = new Vector3(transform.position.x, 3f, transform.position.z);
            var follower = Camera.main.GetComponent<CameraFollow>();
            if (follower)
            {
                follower.Setup(transform);
            }
            if(MyUIManager.Instance.speedText != null)
            {
                useUI = true;
                carSpeedText = MyUIManager.Instance.speedText;
            }
            //gameObject.tag = "MainPlayerCar";
        }
        else
        {
            //gameObject.tag = "ConnectedPlayerCar";
        }
        frontBodyWeapon.enabled = false;
        //selfBodyWeapon.gameObject.SetActive(false);
    }

    
    

    public void TriggerAttackVFX(int combo)
    {
        
        /*
        switch (combo)
        {
            case 1:
                SpawnerControl.Instance.SpawnTemporaryObject(attackAction.settings[0].vfxPrefab, 
                    attackPivots[0].position, 
                    attackPivots[0].transform.rotation);
                if(IsOwner)
                    EnableFrontBodyColliderServerRpc();
                break;
            case 2:
                SpawnerControl.Instance.SpawnTemporaryObject(attackAction.settings[1].vfxPrefab, 
                    attackPivots[1].position, 
                    attackPivots[1].transform.rotation);
                if (IsOwner)
                    EnableFrontBodyColliderServerRpc();
                break;
            case 3:
                SpawnerControl.Instance.SpawnTemporaryObject(attackAction.settings[2].vfxPrefab,
                    attackAction.settings[2].offsetPosition, 
                    attackPivots[2].transform.rotation, 
                    1, 
                    attackAction.settings[2].overrideLifetime, 
                    transform);
                if (IsOwner)
                    SpinAttackServerRpc();


                break;
            default:
                Debug.Log($"Combo {combo} VFX has not been set!");
                break;
        }
        */

    }

    [ServerRpc]
    public void SpawnVFXServerRpc()
    {
        

    }

    [ServerRpc]
    public void EnableFrontBodyColliderServerRpc()
    {
        if (!IsServer) return;
        frontBodyWeapon.StartDetect(false);
    }
    [ServerRpc]
    public void SpinAttackServerRpc()
    {
        if (!IsServer) return;
        selfBodyWeapon.StartDetectForAWhile();
    }

    protected override void Update()
    {
        if ((IsServer || IsHost || IsClient) && !IsOwner) return;
        base.Update();
    }


    //Animation Events
    public void AnimEnterIdle()
    {
        anim.SetInteger("AttackCount", 0);
        attackCount = 0;
    }

}
