using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject snowBallCloneTemplate;

    public GameObject snowBall()
    {
        GameObject g = Instantiate(snowBallCloneTemplate);
        return g;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
