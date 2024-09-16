using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectSceneController : MonoBehaviour
{
    [SerializeField] private Button loadGameSceneButton; // Refer�ncia ao bot�o
    [SerializeField] private string gameSceneName = "GameScene"; // Nome da cena do jogo

    private void Start()
    {
        if (loadGameSceneButton != null)
        {
            loadGameSceneButton.onClick.AddListener(OnLoadGameSceneButtonClicked);
        }
        else
        {
            Debug.LogError("Bot�o 'loadGameSceneButton' n�o est� atribu�do.");
        }
    }

    private void OnLoadGameSceneButtonClicked()
    {
        // Carregar a cena GameScene
        SceneManager.LoadScene(gameSceneName);
    }
}
