using UnityEngine;
using System.Collections;

public class IntroManager : MonoBehaviour {

    public network.NetworkManager netManager;

	// Use this for initialization
	void Start ()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(netManager.offlineScene);    
	}

}
