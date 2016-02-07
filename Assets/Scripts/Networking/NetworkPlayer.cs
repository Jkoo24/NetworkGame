using UnityEngine;
using System.Collections;

namespace network
{
    public class NetworkPlayer
    {
        private int playerId = -1;

        public NetworkPlayer(int playerId)
        {
            this.playerId = playerId;
        }

        public int getPlayerId()
        {
            return playerId;
        }
    }
}