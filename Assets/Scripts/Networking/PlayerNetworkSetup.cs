using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerNetworkSetup : NetworkBehaviour
{

	void Start ()
    {
	    if(isLocalPlayer)
        {
            GetComponentInChildren<Camera>().enabled = true;
            GetComponentInChildren<AudioListener>().enabled = true;
        }

        if(isServer)
        {
            //GetComponent<Game.Characters.FirstPersonController>().enabled = true;
        }
	}

}
