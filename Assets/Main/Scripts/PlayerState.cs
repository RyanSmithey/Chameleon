using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

namespace PlayerStates
{
    public class PlayerState : NetworkBehaviour
    {
        public static string QueuedName;
        public static Dictionary<PlayerState, NetworkConnection> PlayerStateToConnection = new();
        public static Dictionary<NetworkConnection, PlayerState> ConnectionToPlayerState = new();
        public static event Action<PlayerState> PlayerStateCreated;
        public static event Action<NetworkConnection> PlayerStateDestroyed;

        public string Name = "";
        public event Action<string> OnNameChanged;
        
        protected virtual void Start()
        {
            PlayerStateToConnection.Add(this, this.Owner);
            ConnectionToPlayerState.Add(this.Owner, this);

            PlayerStateCreated?.Invoke(this);
        }

        protected virtual void OnDestroy()
        {
            PlayerStateToConnection.Remove(this);
            ConnectionToPlayerState.Remove(this.Owner);
        }

        public override void OnOwnershipClient(NetworkConnection prevOwner)
        {
            base.OnOwnershipClient(prevOwner);

            if (base.IsOwner)
            {
                SetName(QueuedName);
            }
        }

        public override void OnStopNetwork()
        {
            base.OnStopNetwork();

            PlayerStateDestroyed?.Invoke(this.Owner);
        }

        [ServerRpc]
        public void SetName(string NewName)
        {
            Name = NewName;
            OnNameChanged?.Invoke(Name);
            ClientSetName(NewName);
        }

        [ObserversRpc(BufferLast =true)]
        public void ClientSetName(string NewName)
        {
            Name = NewName;
            OnNameChanged?.Invoke(Name);
        }
    }
}
