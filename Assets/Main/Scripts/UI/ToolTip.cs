using System.Collections;
using System.Collections.Generic;
using Managing;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ToolTip : MonoBehaviour
{
    [SerializeField] private string Pregame;
    [SerializeField] private string WordSelectionStandard;
    [SerializeField] private string WordSelectionChameleon;
    [SerializeField] private string VotingStandard;
    [SerializeField] private string VotingChameleon;

    private TextMeshProUGUI ToolTipTextBox;

    void Awake()
    {
        GameState.OnGameStageChange += GameState_OnGameStageChange;
        if (GameState.GS != null)
        {
            GameState_OnGameStageChange(GameStage.NA, GameState.GS.CurrentGameStage);
        }
        else
        {
            GameState.OnGameStageInitialized += GameState_OnGameStageInitialized;
        }

        ToolTipTextBox = this.GetComponent<TextMeshProUGUI>();
    }

    private void GameState_OnGameStageInitialized(GameStage obj)
    {
        GameState_OnGameStageChange(GameStage.NA, obj);
        GameState.OnGameStageInitialized -= GameState_OnGameStageInitialized;
    }

    private void GameState_OnGameStageChange(GameStage From, GameStage To)
    {
        if (To == GameStage.NA) { ToolTipTextBox.text = Pregame; return; }
        if (PlayerStates.PlayerAtTable.LocalPlayer == null) { ToolTipTextBox.text = "You will be able to join once the current game has finished"; return; }
        
        if (To == GameStage.WordSelection && !PlayerStates.PlayerAtTable.LocalPlayer.IsChameleon) { ToolTipTextBox.text = WordSelectionStandard; return; }
        if (To == GameStage.WordSelection && PlayerStates.PlayerAtTable.LocalPlayer.IsChameleon) { ToolTipTextBox.text = WordSelectionChameleon; return; }
        
        if (To == GameStage.Voting && !PlayerStates.PlayerAtTable.LocalPlayer.IsChameleon) { ToolTipTextBox.text = VotingStandard; return; }
        if (To == GameStage.Voting && PlayerStates.PlayerAtTable.LocalPlayer.IsChameleon) { ToolTipTextBox.text = VotingChameleon; return; }
    }
}
