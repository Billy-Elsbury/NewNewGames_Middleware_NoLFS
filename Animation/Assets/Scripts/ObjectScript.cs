using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ObjectScript : NetworkBehaviour
{
    public void setParent(Transform rightHandGrip)
    {
        NetworkObject.TrySetParent(rightHandGrip, false);

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