using System.Collections;
using System.Collections.Generic;
using FishNet;
using FishNet.Managing.Scened;
using FishNet.Object;
using UnityEngine;

#if UNITY_EDITOR
using ParrelSync;
#endif

public class ServerLoader : MonoBehaviour
{
    public string SceneName = "";
    public bool Active = true;


    private void Start()
    {
        if (!Active) { return; }

#if UNITY_EDITOR
        if (!ClonesManager.IsClone())
        {
            InstanceFinder.ServerManager.OnServerConnectionState += ServerManager_OnServerConnectionState;

            InstanceFinder.ServerManager.StartConnection();
            return;
        }

        PlayerStates.PlayerState.QueuedName = ClonesManager.GetArgument();
        InstanceFinder.ClientManager.StartConnection();
        
        return;
#else
        if (SystemInfo.graphicsDeviceID == 0)
        {
            InstanceFinder.ServerManager.OnServerConnectionState += ServerManager_OnServerConnectionState;

            InstanceFinder.ServerManager.StartConnection();
            return;
        }
#endif
    }

    private void ServerManager_OnServerConnectionState(FishNet.Transporting.ServerConnectionStateArgs obj)
    {
        if (obj.ConnectionState != FishNet.Transporting.LocalConnectionState.Started) { return; }

        SceneLoadData sld = new SceneLoadData(SceneName);
        sld.ReplaceScenes = ReplaceOption.All;
        sld.MovedNetworkObjects = new NetworkObject[] { };
        
        InstanceFinder.SceneManager.LoadGlobalScenes(sld);
    }

    private void OnDestroy()
    {
#if UNITY_EDITOR
        if (!ClonesManager.IsClone())
        {
            InstanceFinder.ServerManager.OnServerConnectionState -= ServerManager_OnServerConnectionState;
        }
#endif
    }
}
