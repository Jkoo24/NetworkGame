using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace network
{
    public class NetworkManager : UnityEngine.Networking.NetworkManager
    {

        public string getOnlineSceneName()
        {
            return onlineScene;
        }

        public string getOfflineSceneName()
        {
            return offlineScene;
        }

        public override void OnStartServer()
        {
            Debug.Log("NetworkManager.OnStartServer()");
        }

        public override void OnServerConnect(NetworkConnection conn)
        {
            Debug.Log("NetworkManager.OnServerConnect()");

        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            Debug.Log("NetworkManager.OnServerDisconnect()");
        }


    }
}
