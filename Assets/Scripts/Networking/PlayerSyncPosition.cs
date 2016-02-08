using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

[NetworkSettings(channel = 0, sendInterval = 0.1f)]
public class PlayerSyncPosition : NetworkBehaviour
{
    [SyncVar(hook = "syncPositionValues")]
    private Vector3 syncPos;
    private float lerpRate;
    private Transform playerTransform;

    public void Start()
    {
        lerpRate = 15;
        playerTransform = GetComponent<Transform>();
    }

    void FixedUpdate()
    {
        if (isServer)
        {
            syncPos = playerTransform.position;
        }
    }

    void Update()
    {       
        if(isLocalPlayer)
        {
            lerpTransform();
        }
    }

    private void lerpTransform()
    {
        playerTransform.position = Vector3.Lerp(playerTransform.position, syncPos, Time.deltaTime * lerpRate);

        //if (syncPosList.Count == 0)
        //    return;

        //myTransform.position = Vector3.Lerp(myTransform.position, syncPosList[0], Time.deltaTime * lerpRate);

        //if (Vector3.Distance(myTransform.position, syncPosList[0]) < closeEnough)
        //{
        //    syncPosList.RemoveAt(0);
        //}

        //if(syncPosList.Count > 10)
        //{
        //    lerpRate = FasterLerpRate;
        //}
        //else
        //{
        //    lerpRate = normalLerpRate;
        //}

        //Debug.Log("pos list count = " + syncPosList.Count);
    }

    [Client]
    private void syncPositionValues(Vector3 latestInput)
    {
        syncPos = latestInput;
    }
}
