using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    Animator animator;
    CharacterController playerController;

    int isRunningHash;
    int strafeLeftHash;
    int strafeRightHash;
    int isRunningBackwardsHash;
    int cancelAnimationHash;
    int isCrouchingHash;
    int isJumpingHash;

    public bool jumpTrigger; //detect if player is jumping from other scripts
    bool crouchTrigger = false; //detect if player is crouching

    public float jumpHeight = 2;
    public float gravity = -12;
    public float upVelocity;
    float currentSpeed;

    public bool isCrouching, playerCrouch;
    public float characterHeight, charNewHeight, lastPosition, newPosition, h, nch, returnUpTime, crouchKeyTimer;

    private void Awake()
    {
        playerController = GetComponent<CharacterController>(); 
        characterHeight = playerController.height;
    }
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        isRunningHash = Animator.StringToHash("isRunning");
        isRunningBackwardsHash = Animator.StringToHash("isRunningBackwards");
        strafeLeftHash = Animator.StringToHash("strafeLeft");
        strafeRightHash = Animator.StringToHash("strafeRight");
        cancelAnimationHash = Animator.StringToHash("cancelAnimation");
        isCrouchingHash = Animator.StringToHash("isCrouching");
        isJumpingHash = Animator.StringToHash("isJumping");
    }

    // Update is called once per frame
    void Update()
    {
        //add gravity vvv
        upVelocity += Time.deltaTime * gravity;
        //allows chracter to interact with collisions?
        Vector3 velocity = transform.forward * currentSpeed + Vector3.up * upVelocity;
        playerController.Move(velocity * Time.deltaTime);
        //reset upVelocity when character is on the ground
        if (playerController.isGrounded)
        {
            upVelocity = 0;
        }
        //A bit scuffed, but should hopefully animate the character when performing specific movements.
        runAnimation();
        strafeAnimation();
        runBackwardsAnimation();
        if (playerController.isGrounded)
        {
            crouch();
        }
        jump();

    }
    void runAnimation()
    {
        bool wPressed = Input.GetKey("w");
        bool forwardPressed = Input.GetKey(KeyCode.UpArrow);

        bool isRunning = animator.GetBool("isRunning");

        //press W or up arrow
        if (!isRunning && (wPressed || forwardPressed))
        {
            animator.SetBool(isRunningHash, true);
            animator.SetBool(cancelAnimationHash, true);
        }
        if (isRunning && !wPressed && !forwardPressed)
        {
            animator.SetBool(isRunningHash, false);
            animator.SetBool(cancelAnimationHash, false);
        }
    }
    void strafeAnimation()
    {
        bool aPressed = Input.GetKey("a");
        bool leftPressed = Input.GetKey(KeyCode.LeftArrow);
        bool dPressed = Input.GetKey("d");
        bool rightPressed = Input.GetKey(KeyCode.RightArrow);

        bool strafeLeft = animator.GetBool("strafeLeft");
        bool strafeRight = animator.GetBool("strafeRight");

        //press A or left arrow
        if (!strafeLeft && (aPressed || leftPressed))
        {
            animator.SetBool(strafeLeftHash, true);
            animator.SetBool(cancelAnimationHash, true);
        }
        if (strafeLeft && (!aPressed && !leftPressed))
        {
            animator.SetBool(strafeLeftHash, false);
            animator.SetBool(cancelAnimationHash, false);
        }
        //press D or right arrow
        if (!strafeRight && (dPressed || rightPressed))
        {
            animator.SetBool(strafeRightHash, true);
            animator.SetBool(cancelAnimationHash, true);
        }
        if (strafeRight && !dPressed && !rightPressed)
        {
            animator.SetBool(strafeRightHash, false);
            animator.SetBool(cancelAnimationHash, false);
        }
    }
    void runBackwardsAnimation()
    {
        bool sPressed = Input.GetKey("s");
        bool downPressed = Input.GetKey(KeyCode.DownArrow);

        bool isRunningBackwards = animator.GetBool("isRunningBackwards");

        //press S or down arrow
        if (!isRunningBackwards && (sPressed || downPressed))
        {
            animator.SetBool(isRunningBackwardsHash, true);
            animator.SetBool(cancelAnimationHash, true);
        }
        if (isRunningBackwards && !sPressed && !downPressed)
        {
            animator.SetBool(isRunningBackwardsHash, false);
            animator.SetBool(cancelAnimationHash, false);
        }
    }
    void crouch()
    {
        h = characterHeight;
        nch = -0.25f;
        charNewHeight = 1.5f;

        if (Input.GetKeyDown("c") && crouchTrigger == false)
        {
            crouchTrigger = true;
            playerController.height = charNewHeight; 
            isCrouching = true;
            playerCrouch = true;
            playerController.center = new Vector3(playerController.center.x, nch, playerController.center.z);
            animator.SetBool(isCrouchingHash, true);
            animator.SetBool(cancelAnimationHash, true);
        }
        else if (Input.GetKeyDown("c") && crouchTrigger == true)
        {
            crouchTrigger = false;

            nch = -0.1f;
            charNewHeight = 1.85f;

            playerController.height = charNewHeight;
            isCrouching = false;
            playerCrouch = false;
            playerController.center = new Vector3(playerController.center.x, nch, playerController.center.z);
            animator.SetBool(isCrouchingHash, false);
            animator.SetBool(cancelAnimationHash, false);
        }
    }
    void jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (playerController.isGrounded)
            {
                float jumpVelocity = Mathf.Sqrt(-2 * gravity * jumpHeight);
                upVelocity = jumpVelocity;
                animator.SetBool(isJumpingHash, true);
                animator.SetBool(cancelAnimationHash, true);
            }
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            animator.SetBool(cancelAnimationHash, false);
            animator.SetBool(isJumpingHash, false);
        }
    }
}
