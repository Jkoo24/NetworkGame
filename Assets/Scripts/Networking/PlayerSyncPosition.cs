using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

[NetworkSettings(channel = 0, sendInterval = 0.1f)]
public class PlayerSyncPosition : NetworkBehaviour
{
    [SyncVar(hook = "syncPositionValues")]
    private Vector3 syncPos;

    //[SyncVar(hook = "syncRotationalValues")]
    //private Quaternion rotation;

    private float lerpRate;
   

    private Vector2 lastInputSent = Vector2.zero;
    private Vector2 lastInputRecieved = Vector2.zero;
    private Vector2 interploatedInput = Vector2.zero;

    private Transform playerTransform;


    public Vector2 getMovementInput()
    {
        return interploatedInput;
    }

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
        if (isServer)
        {
            lerpInput();            
        }
        
        if(isLocalPlayer)
        {
            transmitInput();
            lerpTransform();
        }
    }

    private void lerpInput()
    {
        interploatedInput = lastInputRecieved;// Vector2.Lerp(interploatedInput, lastInputRecieved, Time.deltaTime * lerpRate);
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


   // [Command]
    //void CmdProvidePositionToServer(Vector3 pos)
    //{
    //    syncPos = pos;
    //}

    //[ClientCallback]
    //void trasmitPosition()
    //{
    //    if (isLocalPlayer && Vector3.Distance(myTransform.position, lastPos) > threshold)
    //    {
    //        CmdProvidePositionToServer(myTransform.position);
    //        lastPos = myTransform.position;
    //    }
    //}

    [Command]
    void CmdProvideInputToServer(Vector2 latestInput)
    {
        lastInputRecieved = latestInput;
        Debug.Log("CmdProvideInputToServer got new input = " + latestInput);
    }

    [Client]
    private void syncPositionValues(Vector3 latestInput)
    {
        syncPos = latestInput;
    }

    [ClientCallback]
    void transmitInput()
    {
        Vector2 inputVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (isLocalPlayer && !inputVector.Equals(lastInputSent))
        {        
            CmdProvideInputToServer(inputVector);
            lastInputSent = inputVector;
        }
    }
}
