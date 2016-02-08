using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

[NetworkSettings(channel = 0, sendInterval = 0.1f)]
public class PlayerSyncInput : NetworkBehaviour
{   
    private Vector2 lastInputSent = Vector2.zero;
    private Vector2 lastInputRecieved = Vector2.zero;
    private Vector2 interploatedInput = Vector2.zero;

    public Vector2 getMovementInput()
    {
        return interploatedInput;
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
        }
    }

    private void lerpInput()
    {
        interploatedInput = lastInputRecieved;
    }

    [Command]
    void CmdProvideInputToServer(Vector2 latestInput)
    {
        lastInputRecieved = latestInput;
        Debug.Log("CmdProvideInputToServer got new input = " + latestInput);
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
