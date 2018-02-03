﻿using UnityEngine;
using UnityEngine.Networking;

namespace ggj2018.Core.Network
{
    [RequireComponent(typeof(NetworkManagerHUD))]
    public sealed class NetworkManager : UnityEngine.Networking.NetworkManager
    {
        public void Initialize(bool enableNetwork)
        {
            if(!enableNetwork) {
                StartLANHost();
            }

            GetComponent<NetworkManagerHUD>().enabled = enableNetwork;
        }

        private void StartLANHost()
        {
            Debug.Log($"Starting LAN host on {networkAddress}:{networkPort}...");

            StartServer();
            StartClient();
        }
    }
}
