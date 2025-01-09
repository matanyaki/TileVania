using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] float runSpeed = 7f;
    [SerializeField] float jumpSpeed = 20f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] Vector2 deathKick = new Vector2(20f, 10f);
    [SerializeField] GameObject bullet;
    [SerializeField] Transform gun;
    float gravityScaleAtStart;
    bool isAlive = true;

    Animator animator;
    Vector2 moveInput;
    Rigidbody2D rigidbody2D;
    CapsuleCollider2D bodyCollider;
    CircleCollider2D feetCollider;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        bodyCollider = GetComponent<CapsuleCollider2D>();
        feetCollider = GetComponent<CircleCollider2D>();
        gravityScaleAtStart = rigidbody2D.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAlive) { return; }
        Run();
        FlipSprite();
        ClimbLadder();
        Die();
    }
    void OnMove(InputValue value)
    {
        if (!isAlive) { return; }
        moveInput = value.Get<Vector2>();
    }
    void OnJump(InputValue value)
    {
        if (!isAlive) { return; }
        if (value.isPressed && (feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))) //|| collider.IsTouchingLayers(LayerMask.GetMask("Climbing"))))
        {
            Debug.Log("Jump");
            rigidbody2D.velocity += new Vector2(0f, jumpSpeed);
        }
    }
    void OnFire(InputValue value)
    {
        if (!isAlive) { return; }
        Instantiate(bullet, gun.position, transform.rotation);
    }
    void Run()
    {
        Vector2 PlayerVelocity = new Vector2(moveInput.x * runSpeed, rigidbody2D.velocity.y);
        rigidbody2D.velocity = PlayerVelocity;

        bool isPlayerNotStanding = Mathf.Abs(rigidbody2D.velocity.x) > Mathf.Epsilon;
        animator.SetBool("isRunning", isPlayerNotStanding);
    }
    void FlipSprite()
    {
        bool isPlayerNotStanding = Mathf.Abs(rigidbody2D.velocity.x) > Mathf.Epsilon;

        if (isPlayerNotStanding)
        {
            transform.localScale = new Vector2(Mathf.Sign(rigidbody2D.velocity.x), 1f);
        }
    }
    void ClimbLadder()
    {
        if (feetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            bool isPlayerNotStandingOnLadder = Mathf.Abs(rigidbody2D.velocity.y) > Mathf.Epsilon;
            animator.SetBool("isClimbing", isPlayerNotStandingOnLadder);
            rigidbody2D.gravityScale = 0f;
            Vector2 climbVelocity = new Vector2(rigidbody2D.velocity.x, moveInput.y * climbSpeed);
            rigidbody2D.velocity = climbVelocity;
        }
        else
        {
            animator.SetBool("isClimbing", false);
            rigidbody2D.gravityScale = gravityScaleAtStart;
        }
    }
    void Die()
    {
        if (bodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemies", "Hazards")))
        {
            isAlive = false;
            animator.SetTrigger("Dying");
            rigidbody2D.velocity = deathKick;
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }
    }
}
