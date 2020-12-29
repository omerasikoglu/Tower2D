using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, ICollider
{
    private enum AnimationType
    {
        IDLE,
        WALK,
        WALLGRAB,
        JUMP,
        DASH,
    }
    private Rigidbody2D rigidbody2d;
    private AnimationType activeAnimationType;

    //[SerializeField] private SpriteAnimator spriteAnimator;

    [SerializeField] private Sprite[] idleAnimationFrameArray;
    [SerializeField] private Sprite[] walkingAnimationFrameArray;
    [SerializeField] private Sprite[] jumpingAnimationFrameArray;
    [SerializeField] private Sprite[] dashAnimationFrameArray;
    [SerializeField] private Sprite[] wallGrabbingAnimationFrameArray;

    #region Constants
    private const float movementSpeed = 10f;
    private const float jumpForce = 6f;
    private const float fallMultiplier = 5f;
    private const float lowJumpMultiplier = 6f;
    private const float wallJumpMultiplier = 6f;
    #endregion

    #region Vars
    private float x;   //girilen inputun yönünü belirlemek için
    private bool isFacingRight;

    private bool wallGrab;
    private bool wallSlide;
    private bool isMoving;

    //Interface objects
    private bool isGrounded;
    private bool onRightWall;
    private bool onLeftWall;

    //sonradan eklemeler
    private bool canJumpFromRight, canJumpFromLeft;
    private bool canMove = true;
    private bool wallJumped = false;
    private bool isDashing = false;



    private Vector2 velocity;
    #endregion

    #region Interface Implements
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
        //spriteAnimator.PlayAnimation(idleAnimationFrameArray, .2f);
    }
    private void Update()
    {
        isMoving = false;
        wallGrab = false;
        wallSlide = false;

        x = Input.GetAxisRaw("Horizontal");    //yürüme

        if (rigidbody2d.velocity.x != 0) isMoving = true;    //yürüyor

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
                if (Input.GetKey(KeyCode.X)) wallGrab = true;

                if (x == 1 && rigidbody2d.velocity.y < 0) //duvarda sağa tıklıyorsa
                {
                    wallSlide = true;
                }

                if (canJumpFromRight)
                {
                    if (Input.GetKeyDown(KeyCode.C)) //zıpladıysa
                    {
                        rigidbody2d.velocity = new Vector2(rigidbody2d.velocity.x, wallJumpMultiplier);
                        //canJumpFromRight = false;
                        //canJumpFromLeft = true;
                    }
                }
            }
            else if (onLeftWall)
            {
                if (Input.GetKey(KeyCode.X)) wallGrab = true;

                if (x == -1 && rigidbody2d.velocity.y < 0) //duvarda sola tıklıyorsa
                {
                    wallSlide = true;
                }

                if (canJumpFromLeft)
                {
                    if (Input.GetKeyDown(KeyCode.C)) //zıpladıysa
                    {
                        rigidbody2d.velocity = new Vector2(rigidbody2d.velocity.x, wallJumpMultiplier);
                        //canJumpFromRight = true;
                        //canJumpFromLeft = false;
                    }
                }
            }
            //else if(!onLeftWall && !onRightWall)
            //{
            //    canJumpFromRight = false;
            //    canJumpFromLeft = false;
            //}

        }

        if (wallGrab)
        {
            rigidbody2d.gravityScale = 0;
            rigidbody2d.velocity = new Vector2(rigidbody2d.velocity.x, 0);
        }
        else
        {
            rigidbody2d.gravityScale = 1;
        }

        if (rigidbody2d.velocity.y < 0)      //düşüyorsa
        {
            rigidbody2d.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            if (wallSlide)
            {
                rigidbody2d.velocity = new Vector2(rigidbody2d.velocity.x, -2f);
            }
        }
        else if (rigidbody2d.velocity.y > 0 && !Input.GetKey(KeyCode.C))      //zıpladıysa
        {
            rigidbody2d.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        //if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)|| Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) //yürüyo mu
        //{
        //    isMoving = true;

        //    
        //}
        //else if ((!isMoving)) //yürümüyosa
        //{
        //    if (isGrounded) PlayAnimation(AnimationType.IDLE);
        //}

        #region movement
        #endregion

        #region wallGrab



        //if (Input.GetKey(KeyCode.X) && (onLeftWall || onRightWall))
        //{
        //    wallGrab = true;
        //}
        //if (!Input.GetKey(KeyCode.X) && (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow)) && (onLeftWall || onRightWall))
        //{
        //    wallSlide = true;
        //}
        //if (Input.GetKeyDown(KeyCode.C))    //zıplama
        //{

        //    if (isGrounded)
        //    {
        //        rb.velocity = new Vector2(rb.velocity.x, 0);
        //        rb.velocity += jumpForce * (Vector2.up);
        //        canJumpFromRight = true;
        //        canJumpFromLeft = true;
        //    }
        //    if (!isGrounded)
        //    {
        //        if (onRightWall && canJumpFromRight)
        //        {
        //            rb.velocity = new Vector2(rb.velocity.x, wallSlideMultiplier);
        //            // rb.velocity += jumpForce * (Vector2.up / 1.4f);
        //            canJumpFromRight = false;
        //            canJumpFromLeft = true;
        //        }
        //        if (onLeftWall && canJumpFromLeft)
        //        {
        //            rb.velocity = new Vector2(rb.velocity.x, wallSlideMultiplier);
        //            // rb.velocity += jumpForce * (Vector2.up / 1.4f);
        //            canJumpFromRight = true;
        //            canJumpFromLeft = false;
        //        }
        //    }
        //}

        //if (wallGrab)
        //{
        //    rb.gravityScale = 0;
        //    rb.velocity = new Vector2(rb.velocity.x, 0);
        //}
        //else
        //{
        //    rb.gravityScale = 1;
        //}

        //basılı tutmaya duyarlı zıplama
        //if (rb.velocity.y < 0)      //düşüyorsa
        //{
        //    rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        //    if (wallSlide)
        //    {
        //        rb.velocity = new Vector2(rb.velocity.x, -.6f);
        //    }
        //}
        //else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.C))      //zıpladıysa
        //{
        //    rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        //}
        #endregion

        //MovementInput();
        //JumpAndGrabInput();
        //CheckMovementDirection();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
    }
    private void MovementInput()
    {

    }
    private void JumpAndGrabInput()
    {


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
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180f, 0f);
    }
    private void ApplyMovement()
    {
        rigidbody2d.velocity = new Vector2(movementSpeed * x, rigidbody2d.velocity.y);
    }

    private void PlayAnimation(AnimationType animationType)
    {
        if (animationType != activeAnimationType)
        {
            activeAnimationType = animationType;

            switch (animationType)
            {
                case AnimationType.IDLE:
                    // spriteAnimator.PlayAnimation(idleAnimationFrameArray, .2f);//float arttıkça animasyon yavaşlar
                    break;
                case AnimationType.WALK:
                    // spriteAnimator.PlayAnimation(walkingAnimationFrameArray, .1f);
                    break;
                case AnimationType.DASH:
                    // spriteAnimator.PlayAnimation(dashAnimationFrameArray, .1f);
                    break;
                case AnimationType.JUMP:
                    // spriteAnimator.PlayAnimation(jumpingAnimationFrameArray, .1f);
                    break;
                case AnimationType.WALLGRAB:
                    // spriteAnimator.PlayAnimation(wallGrabbingAnimationFrameArray, .1f);
                    break;

            }
        }
    }

}
