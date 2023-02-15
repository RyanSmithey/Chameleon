using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PlayerStates;

public class SetName : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField InputName;

    public void SetNameFunc()
    {
        PlayerState.QueuedName = InputName.text;
    }
}
