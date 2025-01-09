using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float bulletSpeed = 20f;
    PlayerMovement player;
    Rigidbody2D rigidbody2D;
    float xSpeed;

    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<PlayerMovement>();
        xSpeed = player.transform.localScale.x * bulletSpeed;
    }

    void Update()
    {
        rigidbody2D.velocity = new Vector2(xSpeed, 0f);
        transform.localScale = new Vector2(Mathf.Sign(rigidbody2D.velocity.x), 1f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            Destroy(other.gameObject);
        }
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        Destroy(gameObject);
    }
}
