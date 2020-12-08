using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : DynamicMovement
{
    public Transform player;
    public PlayerManager playerManager;
    public float health = 10f;
    public float detectionRadius = 10f;
    public float attackRadius = 1f;
    public float moveSpeed = 4;
    public float offset = 10f;
    public float timeBetweenDirectionChange = 1f;
    public float damage = 2f;

    bool isVisible = true;
    float timer = 0f;

    Vector2 move;

    States state = States.Idle;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    private void Start()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        var playerGO = GameObject.Find("Player");
        if(playerGO)
        {
            player = playerGO.transform;
        }
    }

    private void OnBecameInvisible()
    {
        isVisible = false;
    }
    private void OnBecameVisible()
    {
        isVisible = true;
    }

    private void Update()
    {
        if (isVisible)
        {
            switch (state)
            {
                case States.Idle:
                    Idle();
                    break;
                case States.Detected:
                    Detected();
                    break;
                case States.Attack:
                    Attack();
                    break;
            }

            if (health <= 0)
            {
                Destroy(gameObject);
            }

            updateSortOrder();
        }
    }

    void Idle()
    {
        timer += 1 * Time.deltaTime;
        if (timer >= timeBetweenDirectionChange)
        {
            move = new Vector2(Random.Range(transform.position.x - offset, transform.position.x + offset), 
                               Random.Range(transform.position.y - offset, transform.position.y + offset));
            timer = 0;
        }
        transform.position = Vector2.MoveTowards(transform.position, move, moveSpeed * Time.deltaTime);

        if(player != null)
            if(Vector2.Distance(transform.position, player.position) <= detectionRadius)
            {
                state = States.Detected;
            }
    }
    void Detected()
    {
        Debug.Log("Detected!");
        transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, player.position) > detectionRadius)
        {
            state = States.Idle;
        }
        else if (Vector2.Distance(transform.position, player.position) <= attackRadius)
        {
            state = States.Attack;
        }
    }
    void Attack()
    {
        Debug.Log("Attacking!");
        playerManager.currentPlayerHealth -= damage * Time.deltaTime;

        if (Vector2.Distance(transform.position, player.position) > attackRadius)
        {
            state = States.Detected;
        }
    }

    enum States
    {
        Idle,
        Detected,
        Attack
    }
}
