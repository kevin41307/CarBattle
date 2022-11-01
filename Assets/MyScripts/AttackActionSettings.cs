using UnityEngine;

[System.Serializable]
public class AttackActionSettings
{
    [Header("Collider")]
    [Range(0, 5)]
    public float activateTime = 0;
    public bool repeatedly = false;
    [Range(1, 10)]
    public int maxRepeatCount = 1;

    [Header("VFX")]
    [Range(1,5)]
    public int combo;
    public TemporaryObj vfxSwingPrefab;
    [Range(0,1)]
    public float triggerTime;
    public float overrideLifetime = -1;
    public Vector3 offsetPosition;
    public Vector3 offsetRotation;
    public bool setParent = false;
    [Space]
    public TemporaryObj vfxHitPrefab;

    [Header("SFX")]
    [Space]
    public AudioClip sfx;

}
