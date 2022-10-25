using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporaryObj : MonoBehaviour, ITemped<TemporaryObj> 
{
    [HideInInspector]
    public float expiredTime = 0;
    public int poolID { set; get; }
    public ObjectPooler<TemporaryObj> pool { set; get; }
}