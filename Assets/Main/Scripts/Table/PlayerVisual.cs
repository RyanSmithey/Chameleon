using System.Collections;
using System.Collections.Generic;
using Managing;
using PlayerStates;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Table
{
    public class PlayerVisual : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private TextMeshProUGUI Name;
        [SerializeField] private TextMeshProUGUI Word;
        [SerializeField] private TextMeshProUGUI Score;
        [SerializeField] private TextMeshProUGUI Votes;
        [SerializeField] private Image  Background;

        [SerializeField] private Color ChameleonColor;
        [SerializeField] private Color DefaultColor;

        [SerializeField] private Color HoveredColor;
        [SerializeField] private Color SelectedColor;

        private PlayerAtTable RelevantPlayer;

        protected static PlayerVisual SelectedVisual;

        public void Initialize(PlayerAtTable PAT)
        {
            this.RelevantPlayer = PAT;

            PAT.OnScoreChanged += PAT_OnScoreChanged;
            this.Score.text = PAT.Score.ToString();
            
            this.Word.text = "";
            PAT.OnWordSelected += PAT_OnWordSelected;


            this.Name.text = PlayerState.ConnectionToPlayerState[PAT.Owner].Name;
            PlayerState.ConnectionToPlayerState[PAT.Owner].OnNameChanged += OnNameChanged; ;

            this.Votes.text = "0";
            PlayerAtTable.OnAnyVoteChanged += PAT_OnAnyVoteChanged;
            
            this.Background.color = PAT.IsChameleon ? ChameleonColor : DefaultColor;
            PAT.OnIsChameleonChange += PAT_OnIsChameleonChange;
            
            PAT.OnInstanceDestroyed += PAT_OnInstanceDestroyed;
        }

        private void PAT_OnAnyVoteChanged(PlayerAtTable obj)
        {
            ReTallyVotes();
        }

        private void ReTallyVotes()
        {
            Dictionary<PlayerAtTable, int> Votes = new();

            foreach (PlayerAtTable PAT in PlayerAtTable.AllPlayers)
            {
                if (PAT.CurrentVote == null) { continue; }
                if (!Votes.ContainsKey(PAT.CurrentVote)) { Votes.Add(PAT.CurrentVote, 0); }

                Votes[PAT.CurrentVote] = Votes[PAT.CurrentVote] + 1;
            }

            this.Votes.text = Votes.GetValueOrDefault(RelevantPlayer).ToString();
        }

        private void OnNameChanged(string obj)
        {
            this.Name.text = obj;
        }

        private void PAT_OnInstanceDestroyed()
        {
            Destroy(gameObject);
        }

        private void PAT_OnWordSelected()
        {
            Word.text = RelevantPlayer.SelectedWord;
        }

        private void PAT_OnIsChameleonChange()
        {
            Background.color = RelevantPlayer.IsChameleon ? ChameleonColor : DefaultColor;
        }

        private void PAT_OnScoreChanged()
        {
            Score.text = RelevantPlayer.Score.ToString();
        }

        private void OnDestroy()
        {
            if (RelevantPlayer == null) { return; }

            RelevantPlayer.OnScoreChanged -= PAT_OnScoreChanged;
            RelevantPlayer.OnIsChameleonChange -= PAT_OnIsChameleonChange;
            RelevantPlayer.OnWordSelected -= PAT_OnWordSelected;

            RelevantPlayer.OnInstanceDestroyed -= PAT_OnInstanceDestroyed;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!IsClickable()) { return; }

            SelectedVisual?.Deselect();
            this.Select();
            PlayerAtTable.LocalPlayer.Vote(RelevantPlayer);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!IsClickable()) { return; }

            Background.color = HoveredColor;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!IsClickable()) { return; }

            Background.color = RelevantPlayer.IsChameleon ? ChameleonColor : DefaultColor;
        }

        private bool IsClickable()
        {
            if (RelevantPlayer == null) { return false; }
            if (RelevantPlayer.IsOwner) { return false; }
            if (PlayerAtTable.LocalPlayer.CurrentVote == this.RelevantPlayer) { return false; }
            if (PlayerAtTable.LocalPlayer == null) { return false; }
            if (GameState.GS.CurrentGameStage != GameStage.Voting) { return false; }

            return true;
        }

        protected void Deselect()
        {
            SelectedVisual = null;
            Background.color = DefaultColor;
        }
        protected void Select()
        {
            SelectedVisual = this;
            Background.color = SelectedColor;
        }
    }
}
