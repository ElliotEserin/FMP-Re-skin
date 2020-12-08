using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitObjects : MonoBehaviour
{
    public Item item;

    public bool isRanged = false;
    public float bulletSpeed = 5f;
    public float bulletSpread = 1f;

    public float delay = 0f;
    public float Knockback = 300f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy"))
        {
            EnemyMovement enemy = collision.GetComponent<EnemyMovement>();
            enemy.health -= item.damage;

            Vector2 moveDirection = GameObject.FindGameObjectWithTag("Player").transform.position - transform.position;
            collision.GetComponent<Rigidbody2D>().AddForce(moveDirection.normalized * -300f);
        }
        if (!collision.CompareTag("Player") && isRanged)
        {
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        if(isRanged)
        {
            transform.position = transform.position + (transform.right * bulletSpeed * Time.deltaTime);
        }
    }

    private void Start()
    {
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            Destroy(gameObject, GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length + delay);
        }
        else
        {
            Destroy(gameObject, delay);
        }
        if(isRanged)
        {
            Vector3 mouseScreenPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 lookAt = mouseScreenPosition;

            float angleRad = Mathf.Atan2(lookAt.y - transform.position.y, lookAt.x - transform.position.x);
            float angleDeg = (180 / Mathf.PI) * angleRad;

            angleDeg = Random.Range(angleDeg-bulletSpread, angleDeg+bulletSpread);

            transform.rotation = Quaternion.Euler(0, 0, angleDeg);
        }
    }
}
