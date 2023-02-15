using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using PlayerStates;
using Managing;

namespace GameBoards
{
    public class BoardEntry : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Color BaseColor;
        [SerializeField] private Color HighlightedColor;
        [SerializeField] private Color SelectedColor;
        [SerializeField] private Color CorrectWordColor;
        
        [SerializeField] private Image WordBackground;
        [SerializeField] private TextMeshProUGUI Word;


        public static BoardEntry Selected = null;
        
        private bool Interactable
        {
            get
            {
                return PlayerAtTable.LocalPlayer != null &&
                       PlayerAtTable.LocalPlayer.IsChameleon &&
                       GameState.GS != null &&
                       GameState.GS.CurrentGameStage == GameStage.Voting &&
                       Selected != this;
            }
        }

        public void SetKeyword()
        {
            WordBackground.color = CorrectWordColor;
        }

        public void Initialize(string Text)
        {
            Word.text = Text;
            BoardEntry.Selected = null;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (Selected || !Interactable) { return; }

            WordBackground.color = BaseColor;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (Selected || !Interactable) { return; }

            WordBackground.color = HighlightedColor;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!Interactable) { return; }

            Selected?.Deselect();
            Selected = this;
            WordBackground.color = SelectedColor;

            PlayerAtTable.LocalPlayer?.SetChameleonGuess(transform.GetSiblingIndex());
        }

        public void Deselect()
        {
            this.WordBackground.color = BaseColor;
            Selected = null;
        }
    }
}
