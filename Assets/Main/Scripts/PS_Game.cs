using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Managing;
using UnityEngine;

namespace PlayerStates
{
    public class PS_Game : PlayerState
    {
        [SyncVar(OnChange = nameof(OnReadyChangeFunction))]
        public bool IsReady = false;
        public static event Action<PlayerState> OnReadyChanged;

        protected override void Start()
        {
            base.Start();
            GameState.OnGameStageChange += GameState_OnGameStart;
        }

        private void GameState_OnGameStart(GameStage arg1, GameStage arg2)
        {
            if (arg2 != GameStage.NA)
            {
                IsReady = false;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            GameState.OnGameStageChange -= GameState_OnGameStart;
        }

        private void OnReadyChangeFunction(bool prev, bool next, bool asServer)
        {
            OnReadyChanged?.Invoke(this);
        }

        [ServerRpc]
        public void ChangeReadyStatus(bool NewReadyStatus)
        {
            IsReady = NewReadyStatus;
            OnReadyChanged?.Invoke(this);
        }
    }
}
