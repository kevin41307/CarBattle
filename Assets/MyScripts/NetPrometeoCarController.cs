using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Kogaine.Helpers;

public class NetPrometeoCarController : PrometeoCarController
{
    [Space, Header("VFXs")]
    [SerializeField] private Transform vfxPivotParent;
    [SerializeField] private AttackAction attackAction;
    ObjectPooler<TemporaryObj> swordSlashMiniWhites;
    ObjectPooler<TemporaryObj> swordWhirlwindWhites;
    //TODO Object Pooler manager?
    private List<Transform> attackPivots = new List<Transform>();

    [Space, Header("Weapons")]
    [SerializeField] private BodyWeapon frontBodyWeapon;
    [SerializeField] private BodyWeapon selfBodyWeapon;


    protected void Awake()
    {
        SceneLinkedSMB<NetPrometeoCarController>.Initialise(anim, this);
        for (int i = 0; i < attackAction.settings.Count; i++)
        {
            GameObject go = new GameObject($"VFX_attackPivot{i+1}");
            go.transform.SetParent(vfxPivotParent);
            go.transform.localPosition = attackAction.settings[i].offsetPosition;
            go.transform.localRotation = Quaternion.Euler(attackAction.settings[i].offsetRotation);
            attackPivots.Add(go.transform);
        }
        swordSlashMiniWhites = new ObjectPooler<TemporaryObj>();
        swordSlashMiniWhites.Initialize(2, attackAction.settings[0].vfxPrefab);

        swordWhirlwindWhites = new ObjectPooler<TemporaryObj>();
        swordWhirlwindWhites.Initialize(2, attackAction.settings[2].vfxPrefab);

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
        switch (combo)
        {
            case 1:
                swordSlashMiniWhites.GetNew(attackAction.settings[0].activateTime, attackPivots[0]);
                if(IsServer)
                    EnableFrontBodyCollider();
                break;
            case 2:
                swordSlashMiniWhites.GetNew(attackAction.settings[1].activateTime, attackPivots[1]);
                if (IsServer)
                    EnableFrontBodyCollider();
                break;
            case 3:
                swordWhirlwindWhites.GetNew(attackAction.settings[2].activateTime, attackPivots[2]);
                if (IsOwner)
                    SpinAttackServerRpc();
                break;
            default:
                Debug.Log($"Combo {combo} VFX has not been set!");
                break;
        }
    }

    public void EnableFrontBodyCollider()
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
