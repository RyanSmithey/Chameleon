using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FishNet;
using UnityEngine;
using PlayerStates;
using FishNet.Object;
using GameBoards;
using System;
using Managing;
using FishNet.Transporting;
using FishNet.Connection;

public class GameMode : MonoBehaviour
{
    [SerializeField] private NetworkObject PlayerTableObject;

    [SerializeField] private GameBoard Board;

    [SerializeField] private float ThinkingTime;

    [SerializeField] private int WinningScore;

    [SerializeField] private float DiscussionTime;
    
    [SerializeField] private GameState GS;
    
    private void Start()
    {
        if (!InstanceFinder.IsServer) { DestroyImmediate(gameObject); return; }

        PS_Game.OnReadyChanged += OnPlayerReady;
        PlayerAtTable.OnAnySelectedWord += OnAnySelectedWord;
        PlayerAtTable.OnAnyInstanceDestroyed += OnPlayerRemoved;
    }

    private void OnPlayerRemoved()
    {
        if (PlayerAtTable.AllPlayers.Count <= 1)
        {
            GS.CurrentGameStage = GameStage.NA;
        }
    }

    private void OnAnySelectedWord(PlayerAtTable obj)
    {
        if (GameState.GS.CurrentGameStage != GameStage.WordSelection) { return; }

        foreach (PlayerAtTable PAT in PlayerAtTable.AllPlayers)
        {
            if (PAT.SelectedWord == null || PAT.SelectedWord.Length == 0) { return; }
        }

        EndWordSelection();
    }

    public void OnPlayerReady(PlayerState Players)
    {
        if (GS.CurrentGameStage != GameStage.NA) { return; }
        if (PlayerState.ConnectionToPlayerState.Count <= 1) { return; }

        foreach (PS_Game PS in PlayerState.PlayerStateToConnection.Keys.Cast<PS_Game>())
        {
            if (!PS.IsReady) { return; }
        }

        StartGame();
    }

    private void SelectNewChameleon()
    {
        var SelectedPlayer = PlayerAtTable.AllPlayers.ElementAt(UnityEngine.Random.Range(0, PlayerAtTable.AllPlayers.Count));
        SelectedPlayer.SetChameleon();
    }

    private void AdjustScores()
    {
        Dictionary<PlayerAtTable, int> Votes = new();

        bool ChameleonGuessCorrect = false;
        foreach (PlayerAtTable PAT in PlayerAtTable.AllPlayers)
        {
            if (PAT.IsChameleon)
            { ChameleonGuessCorrect = PAT.IndexOfChameleonSelectedWord == Board.SelectedWordIndex; }

            if (PAT.CurrentVote == null) { continue; }
            if (!Votes.ContainsKey(PAT.CurrentVote)) { Votes.Add(PAT.CurrentVote, 0); }

            Votes[PAT.CurrentVote] = Votes[PAT.CurrentVote] + 1;
        }

        PlayerAtTable MaxVotes = null;
        int MaxValue = 0;
        foreach (KeyValuePair<PlayerAtTable, int> kvp in Votes)
        {
            if (MaxValue == kvp.Value)
            {
                MaxVotes = null;
            }
            if (MaxValue < kvp.Value)
            {
                MaxValue = kvp.Value;
                MaxVotes = kvp.Key;
            }
        }

        bool ChameleonVotedFor = MaxVotes != null && MaxVotes.IsChameleon;
        bool ChameleonWon = !ChameleonVotedFor || ChameleonGuessCorrect;
        int ScoreChange = !ChameleonWon ? 2 : ChameleonVotedFor ? 1 : 2;

        foreach (PlayerAtTable PAT in PlayerAtTable.AllPlayers)
        {
            if (ChameleonWon == PAT.IsChameleon)
            {
                PAT.Score += ScoreChange;
            }
        }
    }

    public void StartGame()
    {
        foreach (PlayerAtTable PAT in PlayerAtTable.AllPlayers)
        {
            InstanceFinder.ServerManager.Despawn(PAT.gameObject, DespawnType.Destroy);
        }

        foreach (PS_Game PS in PlayerState.PlayerStateToConnection.Keys.Cast<PS_Game>())
        {
            InstanceFinder.ServerManager.Spawn(Instantiate(PlayerTableObject), PS.Owner);
        }

        StartRound();
    }

    public void StartRound()
    {
        SelectNewChameleon();
        Board.SelectNewBoard();

        GS.CurrentGameStage = GameStage.WordSelection;
    }

    public void EndWordSelection()
    {
        foreach (PlayerAtTable PAT in PlayerAtTable.AllPlayers)
        {
            PAT.RevealWord();
        }

        GS.CurrentGameStage = GameStage.Voting;
    }
    public void EndDiscussion()
    {
        AdjustScores();

        GS.CurrentGameStage = GameStage.Endgame;

        EndRound();
    }

    public void EndRound()
    {
        if (PlayerAtTable.AllPlayers.Count <= 1) { EndGame(); return; }

        foreach (PlayerAtTable PT in PlayerAtTable.AllPlayers)
        {
            if (PT.Score >= WinningScore)
            {
                EndGame(); 
                return;
            }
        }

        foreach (PlayerAtTable PT in PlayerAtTable.AllPlayers)
        {
            PT.RoundReset();
        }

        StartRound();
    }

    public void EndGame()
    {
        GS.CurrentGameStage = GameStage.NA;
        foreach (PlayerAtTable PAT in PlayerAtTable.AllPlayers)
        {
            InstanceFinder.ServerManager.Despawn(PAT.gameObject, DespawnType.Destroy);
        }
    }

    public void OnDestroy()
    {
        if (!InstanceFinder.IsServer) { return; }

        PS_Game.OnReadyChanged -= OnPlayerReady;
        PlayerAtTable.OnAnySelectedWord -= OnAnySelectedWord;
    }
}