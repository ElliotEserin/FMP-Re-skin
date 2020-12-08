using UnityEngine;

public class PlayerMovement : DynamicMovement
{
    public float walkSpeed, sprintSpeed;
    public Rigidbody2D rb;
    public Animator animator;
    public PlayerManager pm;
    public AudioSource source;
    public AudioClip runSound;

    public Vector2 movement;
    bool isSprinting = false;
    float moveSpeed;

    bool isRunning;

    // Update is called once per frame
    void Update()
    {
        moveSpeed = 0;
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        isSprinting = Input.GetKey(KeyCode.LeftShift);

        if (isSprinting && pm.food > 0) //increasing speed with sprint
        {
            moveSpeed += sprintSpeed;
        }

        if (movement.sqrMagnitude > 0.01) 
        {
            isRunning = true;
            base.updateSortOrder(); 
            if(!source.isPlaying && Time.deltaTime != 0)
            {
                source.PlayOneShot(runSound);
            }
        } //changing sort order
        else
        {
            if(source.isPlaying && isRunning)
            {
                source.Stop();
            }
            isRunning = false;
        }

        if (movement.x != 0 && movement.y != 0) { moveSpeed += walkSpeed / 1.75f; } //moving diagonally
        else { moveSpeed += walkSpeed; } //normal walk speed

        //animation
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
        animator.SetFloat("Speed", movement.sqrMagnitude);

        //changing food
        pm.food -= movement.sqrMagnitude * pm.rateOfFoodDecrease * Time.deltaTime;
        if(pm.currentPlayerWeight > 100)
        {
            moveSpeed /= 1 + (pm.currentPlayerWeight - 100)/4;
        }
    }

    private void FixedUpdate()
    {
        //update position
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}
