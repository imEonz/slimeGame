using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{


    [SerializeField]
    private Rigidbody2D rb;
    [SerializeField]
    private float moveSpeed = 5f;
    [SerializeField]
    private float dashSpeed = 15f;
    private Vector2 dashDirection;
    [SerializeField]
    private float curSpeed;

    [SerializeField]
    Animator playerAnimator;

    private Vector2 moveInput;
    private float dashInput;
    [SerializeField]
    private float dashTime;
    private float dashTimer;
    private bool isDashing;
    [SerializeField]
    private float dashCooldown;
    private float cooldownTimer;
    private bool isCooldown;

    private enum Direction { None, Up, Down, Left, Right }


    public void OnMove(InputValue playerInput)
    {
        moveInput = playerInput.Get<Vector2>();
    }

    private void OnDash(InputValue playerInput)
    {
        dashInput = playerInput.Get<float>();
    }

    private void FixedUpdate()
    {
        

        HandleMovement();
        HandleAnimation();
    }


    private void HandleMovement()
    {
        if(dashInput > 0 && !isCooldown && dashTimer < dashTime)
        {
            if (!isDashing)
            {
                dashDirection = moveInput.normalized;
                Direction dashDir = GetDirection(moveInput);
                string dashClip = null;
                if (dashDir == Direction.Left) dashClip = "dashLeft";
                else if (dashDir == Direction.Right) dashClip = "dashRight";

                if (dashClip != null)
                {
                    playerAnimator.Play(dashClip);
                    float clipLength = playerAnimator.GetCurrentAnimatorStateInfo(0).length;
                    playerAnimator.speed = clipLength / dashTime;
                }
            }

            isDashing = true;
            dashTimer += Time.deltaTime;
            curSpeed = math.lerp(curSpeed, dashSpeed, 7f * Time.deltaTime);

            rb.MovePosition(rb.position + dashDirection * curSpeed * Time.deltaTime);
            return;
        }
        else
        {
            if(isDashing || isCooldown)
            {
                if(playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("dashLeft") || playerAnimator.GetCurrentAnimatorStateInfo(0).IsName("dashRight")) playerAnimator.Play("idle");
                isDashing = false;
                dashInput = 0f;
                dashTimer = 0f;
                isCooldown = true;
                cooldownTimer += Time.deltaTime;
                if(cooldownTimer > dashCooldown) 
                {
                    cooldownTimer = 0f;
                    isCooldown = false;
                    
                }
            }
            curSpeed = math.lerp(curSpeed, moveSpeed, 7f * Time.deltaTime);
        }
        rb.MovePosition(rb.position + moveInput * curSpeed * Time.deltaTime);
    }

    private void HandleAnimation()
    {
        AnimatorStateInfo currentState = playerAnimator.GetCurrentAnimatorStateInfo(0);

        if (currentState.IsName("dashLeft") || currentState.IsName("dashRight"))
        return;

        
        switch (GetDirection(moveInput))
        {

            case Direction.Left:
                playerAnimator.Play("moveLeft");
                break;
            case Direction.Right:
                playerAnimator.Play("moveRight");
                break;            
        case Direction.Up:
                playerAnimator.Play("moveUp");
                break;
            case Direction.Down:
                playerAnimator.Play("moveDown");
                break;
            case Direction.None:
                playerAnimator.Play("idle");
                break;
        }
        playerAnimator.speed = 1f;
    }

    private Direction GetDirection(Vector2 input)
    {
        if (input == Vector2.zero) return Direction.None;

        if (Mathf.Abs(input.x) >= Mathf.Abs(input.y))
            return input.x > 0 ? Direction.Right : Direction.Left;
        else
            return input.y > 0 ? Direction.Up : Direction.Down;
    }
}
