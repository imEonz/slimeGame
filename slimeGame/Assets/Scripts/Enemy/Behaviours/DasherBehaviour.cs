
using Unity.VisualScripting;
using UnityEngine;

public class DasherBehaviour : EnemyBehaviour
{
    [SerializeField] 
    private float speed;
    [SerializeField]
    private float range;
    [SerializeField]
    private float dashTime;
    [SerializeField]
    private float dashCooldown;
    [SerializeField]
    private float dashSpeed;
    [SerializeField]
    private float dashWindup;
    [SerializeField]
    private int dashAmount = 1;
    private int timesDashed;
    private enum ChargeState { Approaching, WindUp, Dashing }
    private ChargeState state = ChargeState.Approaching;
    private float stateTimer;
    Vector2 dashDirection;
    public override void Tick(float deltaTime)
    {
        float distance = Vector2.Distance(enemy.transform.position, player.position);
        Vector2 dir = ((Vector2)player.position - (Vector2)enemy.transform.position).normalized;

        stateTimer += deltaTime;

        switch (state)
        {
            case ChargeState.Approaching:
                Vector2 nextPos = (Vector2)enemy.transform.position + dir * speed * deltaTime;
                rb.MovePosition(nextPos);

                if(distance <= range && stateTimer > dashCooldown)
                {
                    state = ChargeState.WindUp;
                    stateTimer = 0f;
                }
            break;
            case ChargeState.WindUp:
                Vector2 toPlayer = (Vector2)player.position - (Vector2)enemy.transform.position;
                dashDirection = toPlayer.normalized;
                if(stateTimer >= dashWindup)
                {
                    state = ChargeState.Dashing;
                    stateTimer = 0f;
                }

            break;
            case ChargeState.Dashing:
                rb.MovePosition((Vector2)enemy.transform.position + dashDirection * dashSpeed * deltaTime);
                if (stateTimer >= dashTime) 
                { 
                    stateTimer = 0f;
                    timesDashed++;
                    if(dashAmount >= timesDashed) { state = ChargeState.Approaching; timesDashed = 0;}
                    else
                    {
                        Vector2 recaluclateDir = (Vector2)player.position - (Vector2)enemy.transform.position;
                        dashDirection = recaluclateDir.normalized;
                    }
                }

            break;
        }
    }
}