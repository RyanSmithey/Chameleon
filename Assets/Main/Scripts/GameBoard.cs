using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Object;
using GameBoards;
using FishNet.Connection;
using PlayerStates;
using Sirenix.OdinInspector;
using TMPro;

namespace GameBoards
{
    public class GameBoard : NetworkBehaviour
    {
        public BoardSet BoardSet;
        
        private int SelectedBoard;
        public int SelectedWordIndex;

        private List<GameObject> AllBoardComponents = new();

        [SerializeField] private BoardEntry BoardEntry;
        [SerializeField] private TextMeshProUGUI BoardNameField;
        
        public void SelectNewBoard()
        {
            SelectedBoard = Random.Range(0, BoardSet.AllBoards.Count);
            SelectedWordIndex = Random.Range(0, BoardSet.AllBoards[SelectedBoard].Words.Length);
            
            DisplayBoard(SelectedBoard);

            foreach (PlayerAtTable PAT in PlayerAtTable.AllPlayers)
            {
                RevealWordIndex(PAT.Owner, PAT.IsChameleon ? -1 : SelectedWordIndex);
            }
        }

        [TargetRpc]
        private void RevealWordIndex(NetworkConnection conn, int Word)
        {
            SelectedWordIndex = Word;

            if (Word == -1) { return; }
            AllBoardComponents[Word].GetComponent<BoardEntry>()?.SetKeyword();
        }

        [ObserversRpc]
        public void DisplayBoard(int NewSelectedBoard)
        {
            SelectedBoard = NewSelectedBoard;
            UpdateBoard();
        }

        [TargetRpc]
        public void DisplayKeyWord(NetworkConnection conn, int KeyWord)
        {
            AllBoardComponents[KeyWord].GetComponent<BoardEntry>()?.SetKeyword();
        }

        private void UpdateBoard()
        {
            //Destroy Appropriate Children
            while (transform.childCount != 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }

            AllBoardComponents.Clear();
            //Instantate new ones
            foreach (string s in BoardSet.AllBoards[SelectedBoard].Words)
            {
                GameObject g = Instantiate(BoardEntry.gameObject, this.transform);

                g.GetComponent<BoardEntry>().Initialize(s);
                AllBoardComponents.Add(g);
            }

            BoardNameField.text = BoardSet.AllBoards[SelectedBoard].NameOfBoard;
        }
    }
}