using DilmerGames.Core.Singletons;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnerControl : NetworkSingleton<SpawnerControl>
{

    public class SpawnedObj
    {
        public GameObject prefab;
        public NetworkObject no;
        public float startTime;
        public float lifeTime;
        public bool disposed = false;
    }

    List<SpawnedObj> tempSpawnedObjList = new List<SpawnedObj>();

    private void Start()
    {
        NetworkManager.OnServerStarted += () =>
        {
            if(IsServer)
                NetworkObjectPool.Instance.InitializePool();
        };
        
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
                    NetworkObjectPool.Instance.ReturnNetworkObject(temp.no, temp.prefab);
                    if(temp.no.IsSpawned)
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

    //Error need to redesign seperate local and networkobject
    public void SpawnTemporaryObject(GameObject objectPrefab, Vector3 pos, Quaternion rot, int count = 1, float lifetime = 0, Transform parent = null)
    {
        if (!IsServer) return;
        for (int i = 0; i < count; i++)
        {
            var no = NetworkObjectPool.Instance.GetNetworkObject(objectPrefab);

            //no.transform.localPosition = pos;
            //no.transform.rotation = rot;
            //Debug.Log("Parent" + parent);
            no.transform.localPosition = pos;
            no.transform.rotation = rot;
            if( parent)
            {
                no.Spawn();
                no.transform.SetParent(parent,false);
            }

            lifetime = lifetime <= 0 ? no.GetComponent<ParticleSystem>().main.duration : lifetime;
            tempSpawnedObjList.Add(new SpawnedObj { prefab = objectPrefab, no = no, startTime = Time.time, lifeTime = lifetime });

            
        }

    }

}

