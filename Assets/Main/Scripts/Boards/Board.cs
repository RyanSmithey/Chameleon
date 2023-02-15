using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBoards
{

    [CreateAssetMenu(fileName = "NewBoard", menuName = "Boards/New Standard Board", order = 1)]
    public class Board : ScriptableObject
    {
        public string NameOfBoard;

        public string[] Words = new string[25];
    }
}
