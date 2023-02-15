using System.Collections;
using System.Collections.Generic;
using Managing;
using UnityEngine;

namespace LocalTransitions
{

    public class GameStageConditional : MonoBehaviour
    {
        [SerializeField] private GameObject Object;
    
        [SerializeField] private GameStage From;
        [SerializeField] private Condition FromCondition = Condition.Ignore;

        [SerializeField] private GameStage To;
        [SerializeField] private Condition ToCondition = Condition.Include;

        [SerializeField] private GameStage Initial;
        [SerializeField] private Condition InitialCondition = Condition.Include;

        private enum Condition
        {
            Include,
            Exclude,
            Ignore
        }

        void Awake()
        {
            GameState.OnGameStageChange += GameState_OnGameStageChange;
            if (GameState.GS == null)
            {
                GameState.OnGameStageInitialized += EvaluateInitialState;
                return;
            }
            EvaluateInitialState(GameState.GS.CurrentGameStage);
        }

        private void EvaluateInitialState(GameStage obj)
        {
            bool ObjectEnabled = true;
            if (InitialCondition == Condition.Include && obj != Initial) { ObjectEnabled = false; }
            if (InitialCondition == Condition.Exclude && obj == Initial) { ObjectEnabled = false; }

            Object.SetActive(ObjectEnabled);
        }

        private void GameState_OnGameStageChange(GameStage arg1, GameStage arg2)
        {
            bool ObjectEnabled = true;
            if (FromCondition == Condition.Include && arg1 != From) { ObjectEnabled = false; }
            if (FromCondition == Condition.Exclude && arg1 == From) { ObjectEnabled = false; }
            if (ToCondition == Condition.Include && arg2 != To) { ObjectEnabled = false; }
            if (ToCondition == Condition.Exclude && arg2 == To) { ObjectEnabled = false; }

            Object.SetActive(ObjectEnabled);
        }
    }
}