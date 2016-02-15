using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

[NetworkSettings(channel = 0, sendInterval = 0.1f)]
public class PlayerSyncState : NetworkBehaviour
{

    private class PlayerState
    {
        public double timeStamp;
        public Vector3 pos;
        public Vector2 moveInput;

        public override string ToString()
        {
            return "timestamp = " + timeStamp + ", input = " + moveInput + ", pos = " + pos;
        }
    }

    private List<PlayerState> playbackStates = new List<PlayerState>();
    private PlayerState receivedClientState = null; //last sent state from client
    private float speed = 10f;
    private float posSendThreshold = 0.1f;

    public Transform playerTransform;

    void FixedUpdate()
    {
        Vector2 input = Vector2.zero;

        //if server, grab input the client last sent
        if (isServer && receivedClientState != null)
            input = receivedClientState.moveInput;

        //if client, grab last input sent(which should be the current input)
        if (isClient && getLastStateSent() != null)
            input = getLastStateSent().moveInput;

        if (isServer || isLocalPlayer)
        {
            sendState();
            movePlayer(input, Time.deltaTime);
        }
    }

    void Update()
    {
        if(isServer && (Vector3.Distance(playerTransform.position, receivedClientState.pos) > posSendThreshold))
        {
            receivedClientState.pos = playerTransform.position;            

            RpcstateValueSync(receivedClientState);
        }
    }

    private void movePlayer(Vector2 movement, float deltaTime)
    {
        playerTransform.position = new Vector3(deltaTime * speed * movement.x + playerTransform.position.x, 0, deltaTime * speed * movement.y + playerTransform.position.z);
    }

    private void recordSentState(PlayerState newState)
    {
        playbackStates.Add(newState);
    }

    private PlayerState getLastStateSent()
    {
        if (playbackStates.Count <= 0)
            return null;

        return playbackStates[playbackStates.Count - 1];
    }

    private void reconcileState(PlayerState serverState)
    {
        for(int i=0; i < playbackStates.Count; i++)
        {
            if(playbackStates[i].timeStamp == serverState.timeStamp)
            {
                Debug.Log("found match at index = " + i);
                playbackStates.RemoveRange(0, i);
            }
        }

        //if (serverState.timeStamp != getLastStateSent().timeStamp)
        {
            //Debug.Log(serverState.ToString() + "       |       " + getLastStateSent().ToString());

            playerTransform.position = serverState.pos;
        }
    }

    //client recieves new state from game server
    [ClientRpc]
    void RpcstateValueSync(PlayerState serverState)
    {
        if (!isServer && !isLocalPlayer)
        {
            playerTransform.position = serverState.pos;
        }
        else if(!isServer && isLocalPlayer)
        {
            reconcileState(serverState);
        } 
    }

    //server recieves new state from client that owns the object
    [Command]
    void CmdOnStateChanged(PlayerState lateststate)
    {
        receivedClientState = lateststate;
    }

    //sent from client that owns object to inform server of a new state
    [ClientCallback]
    void sendState()
    {
        Vector2 inputVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        PlayerState lastSentState = getLastStateSent();

        if (isLocalPlayer && ((lastSentState == null) || (lastSentState.moveInput != inputVector)))
        {
            PlayerState newState = new PlayerState();
            newState.timeStamp = Network.time;
            newState.pos = playerTransform.position;
            newState.moveInput = inputVector;

            //add to playback
            recordSentState(newState);

            //tell server
            CmdOnStateChanged(newState);
        }
    }
}
