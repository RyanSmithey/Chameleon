using System.Collections;
using System.Collections.Generic;
using Managing;
using PlayerStates;
using UnityEngine;
using TMPro;

namespace GameBoards
{
    public class WordSelection : MonoBehaviour
    {
        [SerializeField] private TMP_InputField TextInput;

        private void Start()
        {
            GameState.OnGameStageChange += GameState_OnGameStageChange;
        }

        private void GameState_OnGameStageChange(GameStage prev, GameStage next)
        {
            if (next != GameStage.WordSelection)
            {
                TextInput.DeactivateInputField();
            }
            else
            {
                TextInput.DeactivateInputField();
            }
        }

        public void SubmitText()
        {
            PlayerAtTable.LocalPlayer.SetWord(TextInput.text);
        }

        private void OnDestroy()
        {
            GameState.OnGameStageChange -= GameState_OnGameStageChange;
        }
    }
}