using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managing;
using PlayerStates;
using FishNet;
using UnityEngine.UI;

public class ReadyButton : MonoBehaviour
{
    [SerializeField] private GameObject ReadyButtonObject;
    [SerializeField] private Image ReadyBackground;
    [SerializeField] private Color ReadyColor;
    [SerializeField] private Color UnReadyColor;

    private bool ReadyStatus;

    private void Start()
    {
        GameState.OnGameStageChange += GameState_OnGameStageChange; ;
    }

    private void GameState_OnGameStageChange(GameStage prev, GameStage next)
    {
        if (next == GameStage.NA) { GameState_OnGameEnd(); }
        else { GameState_OnGameStart(); }
    }

    private void OnDestroy()
    {
        GameState.OnGameStageChange -= GameState_OnGameStageChange; ;
    }

    private void GameState_OnGameEnd()
    {
        ReadyButtonObject.SetActive(true);
        ReadyStatus = false;
        ReadyBackground.color = ReadyStatus ? ReadyColor : UnReadyColor;
    }

    private void GameState_OnGameStart()
    {
        ReadyButtonObject.SetActive(false);
    }

    public void OnReadyPressed()
    {
        if (InstanceFinder.IsServerOnly) { return; }

        ReadyStatus = !ReadyStatus;
        ReadyBackground.color = ReadyStatus ? ReadyColor : UnReadyColor;

        ((PS_Game)PS_Game.ConnectionToPlayerState[InstanceFinder.ClientManager.Connection]).ChangeReadyStatus(ReadyStatus);
    }
}
