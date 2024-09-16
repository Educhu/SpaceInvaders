using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using static Unity.Collections.Unicode;


public class Enemy : NetworkBehaviour
{
    public enum MovementType
    {
<<<<<<< Updated upstream
        base.Spawned();
        isInitialized = true;

        // Inicie o timer de despawn
        despawnTimer = TickTimer.CreateFromSeconds(Runner, despawnTime);
=======
        Falling,
        Pursue,
        StopAtDistance
>>>>>>> Stashed changes
    }

    public MovementType movementType;
    public float chaseRadius = 5f;
    public float stopDistance = 1f;
    public float speed = 3f;

    private Transform targetPlayer;
    public CircleCollider2D visionCollider; // O trigger de visão
   

    private void Start()
    {
       
    }

    private void Update()
    {
        switch (movementType)
        {
            case MovementType.Falling:
                HandleFallingMovement();
                break;
            case MovementType.Pursue:
                HandlePursueMovement();
                break;
            case MovementType.StopAtDistance:
                HandleStopAtDistanceMovement();
                break;
        }

        //if (collision.CompareTag("Bullet"))
        //{
        //    DespawnEnemy(); // Chama a função para despawnar o objeto
        //}
    }

<<<<<<< Updated upstream
    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    Debug.Log("ENCOSTOU!");

    //    if (!isInitialized || !Runner.IsRunning) return;

    //    // Verifica se a colisão é com uma parede
    //    if (collision.CompareTag("Wall"))
    //    {
    //        IsCollidingWithWall = true;
    //    }

    //    //if (collision.CompareTag("Bullet"))
    //    //{
    //    //    DespawnEnemy(); // Chama a função para despawnar o objeto
    //    //}
    //}

    public override void FixedUpdateNetwork()
=======
    private void HandleFallingMovement()
>>>>>>> Stashed changes
    {
        if (transform != null)
        {
            transform.Translate(Vector3.down * speed * Time.deltaTime);
        }
    }

    private void HandlePursueMovement()
    {
<<<<<<< Updated upstream
        Debug.Log("Despawn");
        if (!isInitialized || !Object.HasStateAuthority) return;

        Destroy(gameObject);//Destroi o objeto
        Runner.Despawn(Object);//Destroi o objeto na rede

        // Notifique todos os clientes para despawn o inimigo
        RPC_DespawnEnemy();
=======
        if (targetPlayer != null && transform != null)
        {
            Vector3 direction = (targetPlayer.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
        }
>>>>>>> Stashed changes
    }

    private void HandleStopAtDistanceMovement()
    {
        if (targetPlayer != null && transform != null)
        {
            float distance = Vector3.Distance(transform.position, targetPlayer.position);

            if (distance > stopDistance)
            {
                Vector3 direction = (targetPlayer.position - transform.position).normalized;
                transform.position += direction * speed * Time.deltaTime;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se o objeto que entrou no trigger de visão é um jogador
        PlayerDataNetworked playerData = other.GetComponent<PlayerDataNetworked>();
        if (playerData != null)
        {
            targetPlayer = playerData.transform;
        }
    }

}