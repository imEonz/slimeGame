using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxHP;
    public float curHP;
    public float damage;
    public Transform player;
    public Rigidbody2D rb;
    private float speed = 5f;
    private float xRange = 25f;
    private float yRange = 12.5f;
    public EnemyManager em;

    [SerializeField, Range(0f,1f)]
    private float staggerChance;
    [SerializeField]
    private float staggerTime;
    private float staggerTimer;
    [SerializeField]
    private EnemyHealthBar enemyHealthBar;

    public enum State {Alive, Dead, Stagger, Consumed}
    public State enemyState;

    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private float flashSpeed = 10f;
    private Color originalColor;

    void Start()
    {
        enemyState = State.Alive;
        curHP = maxHP;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        transform.position = new Vector2(Random.Range(-xRange,xRange), Random.Range(-yRange,yRange));

        originalColor = spriteRenderer.color;
    }

    public void MyFixedUpdate()
    {
        if(enemyState == State.Alive)
        {
            Vector2 dir = new Vector2(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y);
            dir = dir.normalized;
            Vector2 nextPos = (Vector2)transform.position + dir * speed * Time.deltaTime;
            rb.MovePosition(nextPos);
        }
        else if (enemyState == State.Stagger)
        {
            Stagger();
        }
        else if (enemyState == State.Dead)
        {
            em.enemies.Remove(this);
            Destroy(gameObject);
        }

    }

    public void TakeDamage(float damage)
    {
        curHP -= damage;
        if(curHP <= 0)
        {
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
            if(staggerChance > Random.Range(0f,1f)) enemyState = State.Stagger;
            else enemyState = State.Dead;
        }
        enemyHealthBar.UpdateSlider();
    }

    private void Stagger()
    {
        staggerTimer += Time.deltaTime;

        float t = Mathf.PingPong(staggerTimer * flashSpeed, 1f);
        spriteRenderer.color = Color.Lerp(originalColor, Color.white, t);

        if(staggerTimer > staggerTime) enemyState = State.Dead;
    }

    public void SetOpacity(float opacity)
    {
        Color c = spriteRenderer.color;
        c.a = opacity;
        spriteRenderer.color = c;
    }
}
