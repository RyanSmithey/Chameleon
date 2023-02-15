using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameBoards
{
    [CreateAssetMenu(fileName = "NewSet", menuName = "Boards/New Board Set", order = 1)]
    public class BoardSet : ScriptableObject
    {
        public string NameOfSet;

        public List<Board> AllBoards;
    }
}