using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Fusion;

public class SessionListPrefeb : MonoBehaviour
{

    public TextMeshProUGUI roomName, playerCount;
    public Button joinButto;

    public void JoinButton()
    {
        string sessionName = roomName.text;
        Debug.Log($"Tentando entrar na sessão: {sessionName}");

        if (FusionManager.runnerInstance != null)
        {
            FusionManager.runnerInstance.GetComponent<FusionManager>().JoinSession(sessionName);
        }
        else
        {
            Debug.LogError("runnerInstance não está inicializado.");
        }
    }
}
