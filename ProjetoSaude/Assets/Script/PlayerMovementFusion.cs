using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerMovementFusion : NetworkBehaviour
{
    private Rigidbody2D _rb; // Refer�ncia ao Rigidbody2D para movimento 2D

    public float moveSpeed = 15f; // Velocidade de movimento

    [SerializeField] private GameObject projectilePrefab;  // Prefab do proj�til
    [SerializeField] private Transform firePoint;          // Ponto de disparo do proj�til
    [SerializeField] private float projectileSpeed = 20f;  // Velocidade do proj�til
    [Networked] public int score { get; set; }            // Pontua��o do jogador

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Disparar o proj�til quando a tecla espa�o for pressionada
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FireProjectile();
        }
    }

    private void FireProjectile()
    {
        // Instancia o proj�til na posi��o e rota��o do firePoint
        NetworkObject projectile = Runner.Spawn(projectilePrefab, firePoint.position, firePoint.rotation);

        // Aplica a velocidade ao proj�til
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if(rb != null)
        {
            rb.velocity = firePoint.up * projectileSpeed; // Move o proj�til para frente
        }

        //Define o jogador como o dono do proj�til para rastrear a pontua��o
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.SetOwner(this);
        }
    }

    // M�todo chamado quando o proj�til acerta um inimigo
    public void OnEnemyHit(int points)
    {
        score += points;  // Adiciona pontos � pontua��o do jogador
        Debug.Log($"Pontua��o: {score}");
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            // Normaliza a dire��o para evitar acelera��o excessiva
            data.direction.Normalize();

            // Move o jogador aplicando for�a ao Rigidbody2D
            _rb.velocity = data.direction * moveSpeed;
        }
    }
}