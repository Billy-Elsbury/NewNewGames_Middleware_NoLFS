using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ObjectScript : NetworkBehaviour
{
    public override void OnNetworkObjectParentChanged(NetworkObject parentNetworkObject)
    {
        print("Snowball parentChangedCalled");

        base.OnNetworkObjectParentChanged(parentNetworkObject);
    }
}
