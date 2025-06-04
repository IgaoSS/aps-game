using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class Spawn : MonoBehaviourPunCallbacks
{
    //public GameObject prefab;
    public List<GameObject> prefabList;

    public float spawnTime;
    private float timeCount;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    [PunRPC]
    void Update()
    {
        timeCount += Time.deltaTime;

        if(timeCount >= spawnTime)
        {
            int randomIndex = Random.Range(0, prefabList.Count);
            GameObject selectedPrefab = prefabList[randomIndex];
            GameObject go = Instantiate(selectedPrefab, transform.position, transform.rotation);

            Destroy(go, 5f);
            timeCount = 0;
        }
    }
}
