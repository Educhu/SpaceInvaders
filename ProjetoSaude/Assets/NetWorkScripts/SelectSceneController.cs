using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectSceneController : MonoBehaviour
{
    [SerializeField] private Button loadGameSceneButton; // Referência ao botão
    [SerializeField] private string gameSceneName = "GameScene"; // Nome da cena do jogo

    private void Start()
    {
        if (loadGameSceneButton != null)
        {
            loadGameSceneButton.onClick.AddListener(OnLoadGameSceneButtonClicked);
        }
        else
        {
            Debug.LogError("Botão 'loadGameSceneButton' não está atribuído.");
        }
    }

    private void OnLoadGameSceneButtonClicked()
    {
        // Carregar a cena GameScene
        SceneManager.LoadScene(gameSceneName);
    }
}
