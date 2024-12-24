using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject snowBallCloneTemplate;

    public GameObject SpawnSnowBall()
    {
        GameObject snowBallInstance = Instantiate(snowBallCloneTemplate);

        NetworkObject networkObject = snowBallInstance.GetComponent<NetworkObject>();
        if (networkObject != null)
        {
            networkObject.Spawn();
        }
        else
        {
            Debug.LogError("snowball prefab missing NetworkObject component");
        }

        return snowBallInstance;
    }
}
