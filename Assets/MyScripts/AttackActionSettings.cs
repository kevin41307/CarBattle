using UnityEngine;

[System.Serializable]
public class AttackActionSettings
{
    [Range(1,5)]
    public int combo;
    [Space]
    public GameObject vfxPrefab;
    [Range(0,1)]
    public float triggerTime;
    public Vector3 offsetPosition;
    public Vector3 offsetRotation;
    [Space]
    public AudioClip sfx;

}
