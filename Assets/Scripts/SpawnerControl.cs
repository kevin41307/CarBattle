using DilmerGames.Core.Singletons;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnerControl : NetworkSingleton<SpawnerControl>
{

    public class SpawnedObj
    {
        public NetworkObject no;
        public float startTime;
        public float lifeTime;
        public bool disposed = false;
    }

    List<SpawnedObj> tempSpawnedObjList = new List<SpawnedObj>();

    private void Start()
    {
        NetworkObjectPool.Instance.InitializePool();
    }

    private void FixedUpdate()
    {
        if(tempSpawnedObjList.Count > 0)
        {
            foreach (var temp in tempSpawnedObjList)
            {
                if (temp.disposed) continue;
                if (Time.time > temp.startTime + temp.lifeTime)
                {
                    temp.no.Despawn();
                    temp.disposed = true;
                }
            }
            tempSpawnedObjList.RemoveAll(x => x.disposed == true);
        }
        
    }
    public void SpawnObject(GameObject objectPrefab, Vector3 pos, Quaternion rot, int count = 1)
    {
        if (!IsServer) return;
        for (int i = 0; i < count; i++)
        {
            var go = NetworkObjectPool.Instance.GetNetworkObject(objectPrefab);
            go.transform.position = pos;
            go.transform.rotation = rot;
            go.Spawn();
        }
    }

    public void SpawnTemporaryObject(GameObject objectPrefab, Vector3 pos, Quaternion rot, int count = 1)
    {
        if (!IsServer) return;
        for (int i = 0; i < count; i++)
        {
            var no = NetworkObjectPool.Instance.GetNetworkObject(objectPrefab);
            no.transform.position = pos;
            no.transform.rotation = rot;
            no.Spawn();

            float lifeTime = no.GetComponent<ParticleSystem>().main.duration;
            tempSpawnedObjList.Add(new SpawnedObj { no = no, startTime = Time.time, lifeTime = lifeTime});
        }

    }
}

