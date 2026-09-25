using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    [SerializeField]
    private float maxHP;
    [SerializeField]
    private float curHP;

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
    public enum State { Still, Moving, Dashing, Consuming, Dead}
    [SerializeField]
    private State playerState;

    [SerializeField]
    PlayerHealthBar playerHealthBar;
    [SerializeField]
    private float consumeIframes = 0.5f;
    private float consumeTimer;
    private Enemy attachedEnemy;
    [SerializeField] 
    private ConsumeMinigame consumeMinigame;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    [SerializeField]
    private SkillManager skillManager;
    [SerializeField]
    private WeaponManager weaponManager;

    private UIManager uIManager;

    CameraFollow camera;
    [SerializeField]
    private Transform gameOverPanel;

    public bool AutoAim = true;

    [SerializeField]
    private float hitFlashDuration = 0.15f;
    private Coroutine hitFlashCoroutine;
    private bool isFlashing;

    void Awake()
    {
        curHP = maxHP;
    }
    void Start()
    {
        originalColor = spriteRenderer.color;
        camera = Camera.main.GetComponent<CameraFollow>();

        uIManager = GameObject.FindAnyObjectByType<UIManager>();
    }
    public void OnMove(InputValue playerInput)
    {
        moveInput = playerInput.Get<Vector2>();
    }

    private void OnDash(InputValue playerInput)
    {
        dashInput = playerInput.Get<float>();

        if (playerState == State.Consuming && dashInput > 0f)
        {
            consumeMinigame.RegisterDashPress();
        }
    }
    private void OnMode(InputValue playerInput)
    {
        if(AutoAim) AutoAim = false;
        else AutoAim = true;
    }

    private void OnTab(InputValue playerInput)
    {
        uIManager.RegisterTabPress();
    }
    private void OnPause(InputValue playerInput)
    {
        uIManager.RegisterEscPress();
    }

    private void FixedUpdate()
    {
        if (playerState == State.Consuming)
        {
            HandleConsuming();
            return;
        }
        consumeTimer += Time.deltaTime;
        HandleMovement();
        HandleAnimation();
    }


    private void HandleMovement()
    {
        if(playerState == State.Dead) return;

        if(dashInput > 0 && !isCooldown && dashTimer < dashTime)
        {
            if (!isDashing)
            {
                spriteRenderer.sortingOrder = 5;

                playerState = State.Dashing;
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


            if(!isFlashing)
            {
                Color c = spriteRenderer.color;
                c.r = Mathf.Lerp(c.r, 0f, 7f * Time.deltaTime);
                spriteRenderer.color = c;
            }


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
            playerState = State.Moving;
            curSpeed = math.lerp(curSpeed, moveSpeed, 7f * Time.deltaTime);

            if(!isFlashing)
            {
                Color c = spriteRenderer.color;
                c.r = Mathf.Lerp(c.r, originalColor.r, 7f * Time.deltaTime);
                spriteRenderer.color = c;
            }

            spriteRenderer.sortingOrder = 3;
        }


        if(moveInput == Vector2.zero) playerState = State.Still;
        rb.MovePosition(rb.position + moveInput * curSpeed * Time.deltaTime);
    }

    private void HandleAnimation()
    {
        if(playerState == State.Dead) return;

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

    public void TakeDamage(float damage)
    {
        curHP -= damage;

        SoundManager.Instance.PlaySoundFXClipRandomPitch(SoundManager.Instance.GetAudioClip("Hit"),this.transform,1f);

        if (hitFlashCoroutine != null) StopCoroutine(hitFlashCoroutine);
        hitFlashCoroutine = StartCoroutine(HitFlash());

        camera.Shake();
        if(curHP <= 0)
        {
            curHP = 0;
            PlayerDied();
        }
        playerHealthBar.UpdatePlayerHPBar();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag != "Enemy") return;
        Enemy enemy = collision.GetComponent<Enemy>();

        if(enemy.enemyState == Enemy.State.Alive && playerState == State.Moving || playerState == State.Still)
        {
            if(consumeTimer < consumeIframes) return;
            TakeDamage(enemy.GetDamage());
        }
        else if (enemy.enemyState == Enemy.State.Stagger && playerState == State.Dashing)
        {
            playerState = State.Consuming;
            enemy.enemyState = Enemy.State.Consumed;
            attachedEnemy = enemy;

            transform.position = enemy.transform.position;
            consumeMinigame.StartConsuming(attachedEnemy, OnConsumeComplete);
        }

    }

    private void HandleConsuming()
    {
        if(attachedEnemy == null)
        {
            playerState = State.Still;
            return;
        }

        spriteRenderer.sortingOrder = 5;
        rb.MovePosition(attachedEnemy.transform.position);
    }

    private void OnConsumeComplete(bool won)
    {
        if (won)
        {
            ConsumeReward(attachedEnemy);
        }
        attachedEnemy.enemyState = Enemy.State.Dead;
        ScoreManager.Instance.AddScore("Consume",300);
        
        attachedEnemy = null;
        isDashing = false;
        dashTimer = 0f;
        playerState = State.Still;
        consumeTimer = 0;
    }

    private void ConsumeReward(Enemy consumedEnemy)
    {

        foreach (var item in consumedEnemy.GetEnemyLoot())
        {
            if(UnityEngine.Random.Range(0f,1f) < item.dropRate)
            {
                if(item.reward is HealReward healReward)
                {
                    healReward.Apply(this, healReward.curLevel);
                }
                else if(item.reward is StatBuffReward statBuffReward)
                {
                    skillManager.InstantiateStatBuff(this, statBuffReward);
                }
                else if(item.reward is WeaponReward weaponReward)
                {
                    weaponManager.PickupWeapon(weaponReward);
                }
            }
        }
    }

    public float GetPlayerHP()
    {
        return curHP;
    }
    public float GetPlayerMaxHP()
    {
        return maxHP;
    }
    public void HealDamage(float percentage)
    {
        float healAmount = maxHP * percentage;

        curHP += healAmount;
        if (curHP > maxHP) curHP = maxHP;
        playerHealthBar.UpdatePlayerHPBar();
    }

    public void ApplyStatBuff(StatType statType, int level)
    {
        switch (statType)
        {
            case StatType.MaxHP:
                maxHP = maxHP * ((level * 0.1f) +1);
                curHP = curHP * ((level * 0.1f) +1);
                playerHealthBar.UpdatePlayerHPBar();
                break;
            case StatType.Speed:
                moveSpeed = moveSpeed + (level * 1.2f);
                break;
            case StatType.Dash:
                dashSpeed =  dashSpeed + (level * 3.5f);
                break;
            case StatType.DashCooldown:
            dashCooldown = dashCooldown/(2 + (5 - level));
                break;
        }
    }

    public State GetPlayerState()
    {
        return playerState;
    }

    private void PlayerDied()
    {
        playerState = State.Dead;
        Time.timeScale = 0f;
        gameOverPanel.gameObject.SetActive(true);
    }

    private IEnumerator HitFlash()
    {
        isFlashing = true;
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(hitFlashDuration);
        spriteRenderer.color = originalColor;
        isFlashing = false;
        hitFlashCoroutine = null;
    }
}
