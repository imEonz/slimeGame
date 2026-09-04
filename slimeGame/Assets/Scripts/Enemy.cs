using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxHP;
    public float curHP;
    public Transform player;
    public Rigidbody2D rb;
    private float speed = 5f;
    private float xRange = 25f;
    private float yRange = 12.5f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        transform.position = new Vector2(Random.Range(-xRange,xRange), Random.Range(-yRange,yRange));
    }

    public void MyFixedUpdate()
    {
        Vector2 dir = new Vector2(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y);
        dir = dir.normalized;
        Vector2 nextPos = (Vector2)transform.position + dir * speed * Time.deltaTime;
        rb.MovePosition(nextPos);
    }
}
