using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

namespace Managing
{
    public enum GameStage
        {
            NA,
            Pregame,
            WordSelection,
            Voting,
            Endgame
        }
    public class GameState : NetworkBehaviour
    {
        public static GameState GS;
        private void Start()
        {
            GS = this;
            OnGameStageInitialized?.Invoke(CurrentGameStage);
        }

        public static event Action<GameStage> OnGameStageInitialized;
        public static event Action<GameStage, GameStage> OnGameStageChange;
        [SyncVar(OnChange = nameof(GameStageChangeFunction))] public GameStage CurrentGameStage;
        
        protected void GameStageChangeFunction(GameStage prev, GameStage next, bool asServer)
        {
            OnGameStageChange?.Invoke(prev, next);
        }
    }
}
