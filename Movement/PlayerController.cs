using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, ICollider
{
    private Rigidbody2D rigidbody2d;

    //[SerializeField] private SpriteAnimator spriteAnimator;

    #region Animations
    private enum AnimationType
    {
        IDLE,
        WALK,
        WALLGRAB,
        JUMP,
        DASH,
    }

    private SpriteAnimator spriteAnimator;
    private AnimationType activeAnimationType;

    [SerializeField] private Sprite[] idleAnimationFrameArray;
    [SerializeField] private Sprite[] walkingAnimationFrameArray;
    [SerializeField] private Sprite[] jumpingAnimationFrameArray;
    [SerializeField] private Sprite[] dashAnimationFrameArray;
    [SerializeField] private Sprite[] wallGrabbingAnimationFrameArray;

    #endregion

    #region Dash

    //variables
    private float dashTimeLeft;
    private float lastImageXpos;
    private float lastDash = -1f; //son dash attıgımızdan geçen süre

    //consts
    private float dashTime = .2f;
    private float dashSpeed = 30f;
    private float distanceBetweenImages = .1f;
    private float dashCooldown = 2.5f;

    #endregion

    #region Movement

    //vars
    private float x;   //girilen inputun yönünü belirlemek için
    private bool isMoving;
    private bool isFacingRight;
    private bool canFlip;

    //consts
    private const float movementSpeed = 10f;

    #endregion

    #region Jump

    //consts
    private const float jumpForce = 6f;
    private const float fallMultiplier = 5f;
    private const float lowJumpMultiplier = 6f;
    private const float wallJumpMultiplier = 6f;

    #endregion

    #region Wall-e

    //vars
    private bool wallGrab;
    private bool wallSliding;
    private bool canJumpFromRight;
    private bool canJumpFromLeft;

    //consts
    private float wallSlideSpeed = 2f;
    private float movementForceInAir = 50f;
    private float airDragMultiplier = .95f; //hava direnci
    private float variableJumpHeightMultiplier = .5f;

    #endregion

    #region Incoming 

    //eklenecekler
    private bool canMove = true;
    private bool wallJumped = false;
    private bool isDashing = false;
    private Vector2 velocity;

    #endregion

    #region Interface

    //Interface objects
    private bool isGrounded;
    private bool onRightWall;
    private bool onLeftWall;

    //Interface Implements
    public void SetBoolIsGrounded(bool isGrounded)
    {
        this.isGrounded = isGrounded;
    }

    public void SetBoolOnRightWall(bool onRightWall)
    {
        this.onRightWall = onRightWall;
    }

    public void SetBoolOnLeftWall(bool onLeftWall)
    {
        this.onLeftWall = onLeftWall;
    }
    #endregion

    private void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        isFacingRight = true;
        //spriteAnimator.PlayAnimation(idleAnimationFrameArray, .2f,false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (Time.time >= (lastDash + dashCooldown)) AttemptToDash();
        }
        CheckInput();
        //CheckOnGrounded();
        CheckWallGrab();

        CheckMovementDirection();
        CheckDash();
        // CheckIfWallSliding();
    }
    private void FixedUpdate()
    {
        ApplyMovement();
        CheckJumping();
    }

    private void CheckIfWallSliding()
    {
        if (IsTouchingWall() && !isGrounded && rigidbody2d.velocity.y < 0)
        {
            wallSliding = true;
        }
        else
        {
            wallSliding = false;
        }
    }

    private void CheckInput()
    {
        wallSliding = false;
        x = Input.GetAxisRaw("Horizontal");    //yürüme

        if (x != 0) isMoving = true;    //yürüyor
        else isMoving = false;
        if (isGrounded)
        {
            canJumpFromRight = true;
            canJumpFromLeft = true;

            if (Input.GetKeyDown(KeyCode.C))
            {
                //rigidbody2d.velocity = new Vector2(rigidbody2d.velocity.x, 0);
                rigidbody2d.velocity += jumpForce * (Vector2.up);
            }

            if (isMoving)
            {
                PlayAnimation(AnimationType.WALK);
            }
            else
            {
                PlayAnimation(AnimationType.IDLE);
            }
        }
        else if (!isGrounded)   //yere değmiyorsa
        {
            if (onRightWall)
            {
                if (Input.GetKey(KeyCode.Z)) wallGrab = true;
                else
                {
                    wallGrab = false;
                }

                if (x == 1 && rigidbody2d.velocity.y < 0) //duvarda sağa tıklıyorsa
                {
                    wallSliding = true;
                }

                if (canJumpFromRight)
                {
                    if (Input.GetKeyDown(KeyCode.C)) //zıpladıysa
                    {
                        rigidbody2d.velocity = new Vector2(rigidbody2d.velocity.x, wallJumpMultiplier);

                        canJumpFromRight = false;
                        canJumpFromLeft = true;
                    }
                }
            }
            else if (onLeftWall)
            {
                if (Input.GetKey(KeyCode.Z)) wallGrab = true;
                else
                {
                    wallGrab = false;
                }

                if (x == -1 && rigidbody2d.velocity.y < 0) //duvarda sola tıklıyorsa
                {
                    wallSliding = true;
                }


                if (canJumpFromLeft)
                {
                    if (Input.GetKeyDown(KeyCode.C)) //zıpladıysa
                    {
                        rigidbody2d.velocity = new Vector2(rigidbody2d.velocity.x, wallJumpMultiplier);
                        canJumpFromRight = true;
                        canJumpFromLeft = false;
                    }
                }
            }
            else if (!onLeftWall && !onRightWall)
            {
                canJumpFromRight = true;
                canJumpFromLeft = true;
            }
        }
    }

    private void AttemptToDash()
    {
        isDashing = true;
        dashTimeLeft = dashTime;
        lastDash = Time.time;

        PlayerAfterImagePool.Instance.GetFromPool();
        lastImageXpos = transform.position.x;
    }
    private void CheckDash()
    {
        if (isDashing)
        {
            if (dashTimeLeft > 0)
            {
                canMove = false;
                canFlip = false;

                rigidbody2d.velocity = new Vector2(dashSpeed * x, rigidbody2d.velocity.y); //add:facingDirection <== x yerine
                dashTimeLeft -= Time.deltaTime;

                if (Mathf.Abs(transform.position.x - lastImageXpos) > distanceBetweenImages)
                {
                    PlayerAfterImagePool.Instance.GetFromPool();
                    lastImageXpos = transform.position.x;
                }
            }
            if (dashTimeLeft <= 0 || onRightWall || onLeftWall)
            {
                isDashing = false;
                canMove = true;
                canFlip = true;
            }

        }
    }
    private void CheckJumping()
    {
        if (rigidbody2d.velocity.y < 0)      //düşüyorsa
        {
            rigidbody2d.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            if (wallSliding)
            {
                rigidbody2d.velocity = new Vector2(rigidbody2d.velocity.x, -2f);
            }
        }
        else if (rigidbody2d.velocity.y > 0 && !Input.GetKey(KeyCode.C))      //zıpladıysa
        {
            rigidbody2d.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }
    private void CheckWallGrab()
    {
        if (wallGrab)
        {
            rigidbody2d.gravityScale = 0;
            rigidbody2d.velocity = new Vector2(rigidbody2d.velocity.x, 0);
        }
        else
        {
            rigidbody2d.gravityScale = 1;
        }
    }
    private void CheckMovementDirection()
    {
        if (isFacingRight && x < 0)
        {
            Flip();
        }
        else if (!isFacingRight && x > 0)
        {
            Flip();
        }
    }
    public void Flip()
    {
        if (!wallSliding)
        {
            isFacingRight = !isFacingRight;
            transform.Rotate(0f, 180f, 0f);
        }

    }
    private void ApplyMovement()
    {
        if (canMove)
        {
            if (isGrounded)
            {
                rigidbody2d.velocity = new Vector2(movementSpeed * x, rigidbody2d.velocity.y);
            }
            else if (!isGrounded && !wallSliding && x != 0)
            {
                Vector2 forceToAdd = new Vector2(movementForceInAir * x, 0f);
                rigidbody2d.AddForce(forceToAdd);

                if (Mathf.Abs(rigidbody2d.velocity.x) > movementSpeed)
                {
                    rigidbody2d.velocity = new Vector2(movementSpeed * x, rigidbody2d.velocity.y);
                }
            }
            else if (!isGrounded && !wallSliding && x == 0) //havada durdugunda x'teki hızını azaltma
            {
                rigidbody2d.velocity = new Vector2(rigidbody2d.velocity.x * airDragMultiplier, rigidbody2d.velocity.y);
            }
        }

        if (wallSliding)
        {
            if (rigidbody2d.velocity.y < -wallSlideSpeed)
            {
                rigidbody2d.velocity = new Vector2(rigidbody2d.velocity.x, -wallSlideSpeed);
            }
        }
    }

    private void PlayAnimation(AnimationType animationType)
    {
        if (animationType != activeAnimationType)
        {
            activeAnimationType = animationType;

            switch (animationType)
            {
                case AnimationType.IDLE:
                    //spriteAnimator.PlayAnimation(idleAnimationFrameArray, .2f,true);//float arttıkça animasyon yavaşlar
                    break;
                case AnimationType.WALK:
                    //spriteAnimator.PlayAnimation(walkingAnimationFrameArray, .1f, true);
                    break;
                case AnimationType.DASH:
                    //spriteAnimator.PlayAnimation(dashAnimationFrameArray, .1f, true);
                    break;
                case AnimationType.JUMP:
                    //spriteAnimator.PlayAnimation(jumpingAnimationFrameArray, .1f, true);
                    break;
                case AnimationType.WALLGRAB:
                    //spriteAnimator.PlayAnimation(wallGrabbingAnimationFrameArray, .1f, true);
                    break;

            }
        }
    }

    private bool IsTouchingWall()
    {
        if (onRightWall || onLeftWall)
        {
            return true;
        }
        else return false;
    }
}
