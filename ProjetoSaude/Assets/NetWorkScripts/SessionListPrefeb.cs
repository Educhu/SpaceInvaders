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
        FusionManager.runnerInstance.StartGame(new StartGameArgs() 
        {
            SessionName = roomName.text 
        }); 

    }

}
