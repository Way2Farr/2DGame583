using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    enum PlayerState {Idle, Running, Airborne}

    PlayerState state;

    public bool stateComplete;

    public Animator animator;

    public Rigidbody2D body;
    public BoxCollider2D groundCheck;
    public LayerMask groundMask;

    public float acceleration;
    
    [Range(0f, 1f)]
    public float groundDecay;
    public float maxXSpeed;

    public float jumpSpeed;

    public bool grounded;

    float xInput;
    float yInput;

    public SoundEffectPlayer soundEffect;

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        HandleJump();

        if (stateComplete) {
            SelectState();
        }

        UpdateState();
    }

    void FixedUpdate() {
        CheckGround();
        HandleXInput();
        ApplyFriction();
    }


// Play animation states
void SelectState() {
    stateComplete = false;

    if (grounded) { 
        if (xInput == 0) {
            state = PlayerState.Idle;
            StartIdle();
        }
        else {
            state = PlayerState.Running;
            StartRunning();
        }
    }
        else {
        state = PlayerState.Airborne;
        StartAirborne();
    }
}

// Update Animation
    void UpdateState() {
        switch(state) {
            case PlayerState.Idle:
                UpdateIdle();

                break;
            case PlayerState.Running:
                UpdateRunning();

                break;
            case PlayerState.Airborne:
                UpdateAirborne();

                break;

        }
    }

    void StartIdle() {
        animator.Play("Breathe Idle");
    }


    void StartRunning() {
        animator.Play("Run");
    }

    void StartAirborne() {
        animator.Play("Jump");
        
    }

    void UpdateIdle() {
        if (!grounded || xInput != 0) {
            stateComplete = true;
        }
    }

    void UpdateRunning(){
        
        float velX = body.velocity.x;

        animator.speed = Mathf.Abs(velX) / maxXSpeed;

        if (Mathf.Abs(velX) < 0.1f || !grounded) {
            stateComplete = true;

        }
  
    }

    void UpdateAirborne() {

        float time = Map(body.velocity.y, jumpSpeed, -jumpSpeed, 0 , 1, true);
        animator.Play("Jump", 0, time);
        animator.speed = 0;

        if (grounded) {
            stateComplete = true;

        }
    }

    void CheckInput() {
        xInput = Input.GetAxis("Horizontal");
        yInput = Input.GetAxis("Vertical");
    }

// Horizontal Movement
    void HandleXInput() {
        // Left or Right Keys
        if(Mathf.Abs(xInput) > 0) {
            // Acceleration value, Update speed and y velocity)
            float increment = xInput * acceleration;
            float newSpeed = Mathf.Clamp(body.velocity.x + increment, -maxXSpeed, maxXSpeed);
            body.velocity = new Vector2(newSpeed, body.velocity.y);
        // Direction of Sprite
        FaceInput();
        }
        }
    
    // Swap sprite based on direction
    void FaceInput(){
            float direction = Mathf.Sign(xInput);
            transform.localScale = new Vector3(direction, 1, 1);

    }
    // Jump
    void HandleJump(){
        // If player is grounded and pressed space, use velocity speed upwards
        if(Input.GetButtonDown("Jump") && grounded) { 
            body.velocity = new Vector2(body.velocity.x, jumpSpeed);  

                    if (soundEffect != null) {
            soundEffect.jumpSound();
        }
        }
        
    
    }

    // Check collison on the ground
    void CheckGround() {
        grounded = Physics2D.OverlapAreaAll(groundCheck.bounds.min, groundCheck.bounds.max, groundMask).Length > 0;
    }

    // Friction, adds movement decay when touching certain surfaces
    void ApplyFriction() {
        if(grounded && xInput == 0 && body.velocity.y <= 0) {
        body.velocity *= groundDecay;
        }
    }

    public static float Map(float value, float min1, float max1, float min2, float max2, bool clamp = false) {
        float val = min2 + (max2 - min2) * ((value - min1) / (max1 - min1));

        return clamp ? Mathf.Clamp(val, Mathf.Min(min2, max2), Mathf.Max(min2, max2)) : val;

    }
}
