using System;
using System.Collections;
using System.Collections.Generic;
using FishNet;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Managing;
using UnityEngine;

namespace PlayerStates
{
    public class PlayerAtTable : NetworkBehaviour
    {
        public static readonly HashSet<PlayerAtTable> AllPlayers = new();
        public static event Action<PlayerAtTable> OnNewPlayerAtTable;
        public event Action OnInstanceDestroyed;
        public static event Action OnAnyInstanceDestroyed;

        public static PlayerAtTable LocalPlayer;

        public bool IsChameleon = false;
        public event Action OnIsChameleonChange;
        public static event Action<PlayerAtTable> OnAnyIsChameleonChange;

        public int IndexOfChameleonSelectedWord = -1;
        public event Action OnChameleonSelectedWordChange;

        [SyncVar(OnChange = nameof(OnScoreChangeFunction))]
        public int Score = 0;
        public event Action OnScoreChanged;
        public static event Action<PlayerAtTable> OnAnyScoreChanged;

        public string SelectedWord = null;
        public event Action OnWordSelected;
        public static event Action<PlayerAtTable> OnAnySelectedWord;

        public PlayerAtTable CurrentVote;
        public event Action OnVoteChanged;
        public static event Action<PlayerAtTable> OnAnyVoteChanged;

        [ServerRpc]
        public void SetChameleonGuess(int IndexOfWord)
        {
            IndexOfChameleonSelectedWord = IndexOfWord;
            OnChameleonSelectedWordChange?.Invoke();
        }

        [ServerRpc(RunLocally = true)]
        public void SetWord(string NewWord)
        {
            if (GameState.GS.CurrentGameStage != GameStage.WordSelection) { return; }

            SelectedWord = NewWord;
            OnWordSelected?.Invoke();
            OnAnySelectedWord?.Invoke(this);
        }
        
        [ServerRpc]
        public void Vote(PlayerAtTable Nob)
        {
            ShareVote(Nob);
        }
        [ObserversRpc(RunLocally = true)]
        public void ShareVote(PlayerAtTable Nob)
        {
            CurrentVote = Nob;
            OnVoteChanged?.Invoke();
            OnAnyVoteChanged?.Invoke(this);
        }

        public void RevealWord()
        {
            Observer_RevealWord(this.SelectedWord);
        }

        [ObserversRpc]
        private void Observer_RevealWord(string NewWord)
        {
            SelectedWord = NewWord;
            OnWordSelected?.Invoke();
            OnAnySelectedWord?.Invoke(this);
        }

        private void OnScoreChangeFunction(int prev, int next, bool asServer)
        {
            OnScoreChanged?.Invoke();
            OnAnyScoreChanged?.Invoke(this);
        }

        internal void SetChameleon()
        {
            IsChameleon = true;

            RevealChameleon(Owner, true);
        }

        [ObserversRpc, TargetRpc]
        public void RevealChameleon(NetworkConnection conn, bool IsChameleon)
        {
            if (IsChameleon == this.IsChameleon) { return; }

            this.IsChameleon = IsChameleon;
            OnIsChameleonChange?.Invoke();
            OnAnyIsChameleonChange?.Invoke(this);
        }

        [ObserversRpc(RunLocally =true)]
        public void RoundReset()
        {
            this.IsChameleon = false;
            this.OnIsChameleonChange?.Invoke();
            OnAnyIsChameleonChange?.Invoke(this);

            this.CurrentVote = null;
            this.OnVoteChanged?.Invoke();
            OnAnyVoteChanged?.Invoke(this);

            this.IndexOfChameleonSelectedWord = -1;
            this.OnChameleonSelectedWordChange?.Invoke();

            this.SelectedWord = null;
            this.OnWordSelected?.Invoke();
            OnAnySelectedWord?.Invoke(this);
        }

        private void Awake()
        {
            AllPlayers.Add(this);
        }

        public void Start()
        {
            OnNewPlayerAtTable?.Invoke(this);
            if (base.Owner.IsLocalClient)
            {
                LocalPlayer = this;
            }
        }

        public void OnDestroy()
        {
            AllPlayers.Remove(this);
            OnInstanceDestroyed?.Invoke();
            OnAnyInstanceDestroyed?.Invoke();
        }
    }
}