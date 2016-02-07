using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

[NetworkSettings(channel = 0, sendInterval = 0.1f)]
public class PlayerSyncPosition : NetworkBehaviour
{
    [SyncVar(hook = "syncPositionValues")]
    private Vector3 syncPos;

    [SerializeField]
    Transform myTransform;

        
    private float lerpRate;
    private float normalLerpRate = 15;
    private float FasterLerpRate = 20;

    private Vector3 lastPos = Vector3.zero;
    private float threshold = 0.5f;

    private List<Vector3> syncPosList = new List<Vector3>();

    private float closeEnough = 0.1f;

    public void Start()
    {
        lerpRate = normalLerpRate;
    }

    void FixedUpdate()
    {
        trasmitPosition();
    }

    void Update()
    {
        LerpPosition();
    }

    void LerpPosition()
    {
        if(!isLocalPlayer)
        {
                historicalLerping();          
        }
    }

    [Client]
    private void syncPositionValues(Vector3 latestestPos)
    {
        syncPos = latestestPos;
        syncPosList.Add(latestestPos);
    }

    private void ordinaryLerping()
    {
        myTransform.position = Vector3.Lerp(myTransform.position, syncPos, Time.deltaTime * lerpRate);
    }

    private void historicalLerping()
    {
        if (syncPosList.Count == 0)
            return;

        myTransform.position = Vector3.Lerp(myTransform.position, syncPosList[0], Time.deltaTime * lerpRate);

        if (Vector3.Distance(myTransform.position, syncPosList[0]) < closeEnough)
        {
            syncPosList.RemoveAt(0);
        }

        if(syncPosList.Count > 10)
        {
            lerpRate = FasterLerpRate;
        }
        else
        {
            lerpRate = normalLerpRate;
        }

        Debug.Log("pos list count = " + syncPosList.Count);
    }


    [Command]
    void CmdProvidePositionToServer(Vector3 pos)
    {
        syncPos = pos;
    }

    [ClientCallback]
    void trasmitPosition()
    {
        if (isLocalPlayer && Vector3.Distance(myTransform.position, lastPos) > threshold)
        {
            CmdProvidePositionToServer(myTransform.position);
            lastPos = myTransform.position;
        }
    }
}
