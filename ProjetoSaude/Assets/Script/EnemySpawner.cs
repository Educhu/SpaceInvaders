using Fusion;
using static Unity.Collections.Unicode;
using UnityEngine;

using System.Collections;


public class EnemySpawner : NetworkBehaviour
{
    public NetworkObject enemyPrefab;
    public NetworkObjectPoolDefault objectPool; // Refer�ncia � pool de objetos
    public float spawnInterval; // Intervalo de tempo entre cada spawn
    public float minX = -12f; // Limite m�nimo no eixo X para a posi��o do spawn
    public float maxX = 12f; // Limite m�ximo no eixo X para a posi��o do spawn
    public float minY = 20f; // Limite m�nimo no eixo Y para a posi��o do spawn
    public float maxY = 25f; // Limite m�ximo no eixo Y para a posi��o do spawn

    private float timer; // Contador de tempo para controlar os spawns

    private void Start()
    {
        StartCoroutine(StartSpawningWithDelay(5f)); // Inicia a rotina com um atraso de 5 segundos
    }

    private IEnumerator StartSpawningWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Aguarda o tempo especificado
        spawnInterval = UnityEngine.Random.Range(1f, 5f); // Define o intervalo de spawn ap�s o atraso
    }

    private void Update()
    {
        if (Runner.IsServer)
        {
            timer += Time.deltaTime;

            if (timer >= spawnInterval)
            {
                SpawnEnemy();
                timer = 0f; // Reseta o contador de tempo
            }
        }
    }

    private void SpawnEnemy()
    {
        Vector3 spawnPosition = new Vector3(UnityEngine.Random.Range(minX, maxX), UnityEngine.Random.Range(minY, maxY), 0);
        spawnPosition += transform.position;

        // Obtenha o inimigo da pool
        NetworkObject enemyObject = objectPool.GetObjectFromPool(enemyPrefab);

        // Defina a posi��o e rota��o do inimigo antes de spawn�-lo na rede
        enemyObject.transform.position = spawnPosition;
        enemyObject.transform.rotation = Quaternion.identity;

        // Fa�a o spawn do inimigo na rede
        Runner.Spawn(enemyObject, spawnPosition, Quaternion.identity, Object.InputAuthority);

        // Atualize o intervalo de spawn
        spawnInterval = UnityEngine.Random.Range(0.1f, 1f);
    }
}