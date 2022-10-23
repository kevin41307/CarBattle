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
        }
        
    }

    public void TriggerAttackVFX(int combo)
    {
        switch (combo)
        {
            case 1:
                SpawnerControl.Instance.SpawnTemporaryObject(attackVFXPrefab, attackPivots[0].position, attackPivots[0].transform.rotation);
                break;
            case 2:
                SpawnerControl.Instance.SpawnTemporaryObject(attackVFXPrefab, attackPivots[1].position, attackPivots[1].transform.rotation);
                break;
            case 3:
                break;
            default:
                Debug.Log($"Combo {combo} VFX has not been set!");
                break;
        }

   }

    protected override void Update()
    {
        if ((IsOwner || IsHost || IsClient) && !IsOwner) return;
        base.Update();
    }


    //Animation Events
    public void AnimEnterIdle()
    {
        anim.SetInteger("AttackCount", 0);
        attackCount = 0;
    }

}
