using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;
using TMPro;
using Managing;
using FishNet;

[RequireComponent(typeof(TextMeshProUGUI))]
public class Timer : NetworkBehaviour
{
    [SyncObject]
    public readonly SyncTimer NetworkTimer = new();

    [SerializeField] private TextMeshProUGUI Text;
    [SerializeField] private GameMode GM;

    private void Start()
    {
        Text = GetComponent<TextMeshProUGUI>();

        if (InstanceFinder.IsServer)
        {
            GameState.OnGameStageChange += GameState_OnGameStageChange;
        }
    }

    private void GameState_OnGameStageChange(GameStage prev, GameStage next)
    {
        if (next == GameStage.Voting)
        {
            NetworkTimer.StartTimer(30.0f);
            NetworkTimer.OnChange += NetworkTimer_OnChange;
        }
    }

    private void NetworkTimer_OnChange(SyncTimerOperation op, float prev, float next, bool asServer)
    {
        if (op == SyncTimerOperation.Finished && base.IsServer)
        {
            GM.EndDiscussion();
        }
    }

    private void Update()
    {
        NetworkTimer.Update(Time.deltaTime);
        Text.text = Mathf.CeilToInt(NetworkTimer.Remaining).ToString();
        if (Text.text == "0")
        {
            Text.text = "";
        }
    }
}
