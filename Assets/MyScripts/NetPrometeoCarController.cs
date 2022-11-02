using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Kogaine.Helpers;

public class NetPrometeoCarController : PrometeoCarController
{
    /*
    [Space, Header("VFXs")]
    [SerializeField] private Transform vfxPivotParent;
    [SerializeField] private AttackAction attackAction;
    ObjectPooler<TemporaryObj> swordSlashMiniWhites;
    ObjectPooler<TemporaryObj> swordWhirlwindWhites;
    //TODO Object Pooler manager?
    private List<Transform> attackPivots = new List<Transform>();

    [Space, Header("Weapons")]
    [SerializeField] private Weapon frontBodyWeapon;
    [SerializeField] private Weapon selfBodyWeapon;
    */
    [Header("Weapon")]
    [SerializeField] private WeaponHandler weaponHandler;
    [SerializeField] private UltimateCircularHealthBar healthBar; 
    public const string playerCarPrefix = "PlayerCar";

    private float velocity;
    protected void Awake()
    {
        SceneLinkedSMB<NetPrometeoCarController>.Initialise(anim, this);
        /*
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
        */
        healthBar = MyUIManager.Instance.healthBar;
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        transform.name = $"{playerCarPrefix} {GetComponent<NetworkObject>().OwnerClientId}";


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
            (NetworkManager.Singleton as MyNetworkManager).playerCars.Add(OwnerClientId, gameObject);
        }
        else
        {
            //gameObject.tag = "ConnectedPlayerCar";
        }
        //frontBodyWeapon.enabled = false;
        //selfBodyWeapon.gameObject.SetActive(false);
        weaponHandler.DisableWeapons();

    }

    public override void OnNetworkDespawn()
    {
        (NetworkManager.Singleton as MyNetworkManager).playerCars.Remove(OwnerClientId);
    }

    public void TriggerAttackVFX(int combo)
    {
        switch (combo)
        {
            case 1:
            case 2:
            case 3:
                weaponHandler.Use(combo - 1);
                break;
            default:
                Debug.Log($"Combo {combo} VFX has not been set!");
                break;
        }
    }
    protected override void Update()
    {
        if ((IsServer || IsHost || IsClient) && !IsOwner) return;
        base.Update();
    }



    private void FixedUpdate()
    {
        CarSpeedBar();
    }
    public void CarSpeedBar()
    {
        if (useUI && healthBar)
        {

            float count = healthBar.SegmentCount - 1;
            float percent = ((1 - carSpeed / maxSpeed) * count ) + 1;
            healthBar.RemovedSegments = Mathf.SmoothDamp(healthBar.RemovedSegments, percent, ref velocity, 10 * Time.fixedDeltaTime);
        }
    }

    //Animation Events
    public void AnimEnterIdle()
    {
        anim.SetInteger("AttackCount", 0);
        attackCount = 0;
    }

}
