using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerNetworkSetup : NetworkBehaviour
{

	void Start ()
    {
	    if(isLocalPlayer)
        {
            GetComponent<Game.Characters.FirstPersonController>().enabled = true;
            GetComponentInChildren<Camera>().enabled = true;
            GetComponentInChildren<AudioListener>().enabled = true;
        }
	}

}
