using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    [Space, Header("VFXs")]
    //[SerializeField] private AttackAction attackAction;
    [SerializeField] private Transform vfxPivotParent;

    private List<Transform> attackPivots = new List<Transform>();
    //TODO Object Pooler manager?
    Weapon[] weapons = null;

    private void Awake()
    {
        /*
        for (int i = 0; i < attackAction.settings.Count; i++)
        {
            GameObject go = new GameObject($"VFX_attackPivot{i + 1}");
            go.transform.SetParent(vfxPivotParent);
            go.transform.localPosition = attackAction.settings[i].offsetPosition;
            go.transform.localRotation = Quaternion.Euler(attackAction.settings[i].offsetRotation);
            attackPivots.Add(go.transform);
        }
        */
        weapons = GetComponentsInChildren<Weapon>();
        foreach (var weapon in weapons)
        {
            weapon.Setup(vfxPivotParent);
        }
    }

    public void DisableWeapons()
    {
        foreach (var weapon in weapons)
        {
            weapon.enabled = false;
        }
    }

    public void Use(int index, bool continuously)
    {
        weapons[index].Use(continuously);
    }
}
