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
    [SerializeField]
    private float curSpeed;

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
    }


    private void HandleMovement()
    {
        if(dashInput > 0 && !isCooldown && dashTimer < dashTime)
        {
            isDashing = true;
            dashTimer += Time.deltaTime;
            curSpeed = math.lerp(curSpeed, dashSpeed, 7f * Time.deltaTime);
        }
        else
        {
            if(isDashing || isCooldown)
            {
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
}
