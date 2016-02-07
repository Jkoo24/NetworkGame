using UnityEngine;
using System.Collections;

namespace network
{
    [RequireComponent(typeof(UnityEngine.Networking.NetworkIdentity))]
    public class NetworkObject : MonoBehaviour
    {
        protected bool owner = false;
        protected NetworkViewID objId;

        public void spawn(Vector3 pos, Quaternion rot)
        {
            Debug.Log("NetworkObject.spawn");

            GameObject obj = (GameObject)Instantiate(gameObject, pos, rot);
            UnityEngine.Networking.NetworkServer.Spawn(obj);
        }
    }
}
