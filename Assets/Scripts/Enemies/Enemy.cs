﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("General Settings")]
    [SerializeField] protected float radius;
    [SerializeField] protected float speed;
    protected float currentSpeed;
    protected bool targetInRange;
    protected bool isInitPos;
    protected Transform target;
    protected Vector2 initPosition;
    protected Vector2 dirEnemy;
    bool canMove = true;

    [Header("Attack Settings")]
    [SerializeField] protected float timeToAttack;
    [SerializeField] protected int damage;
    [SerializeField] protected float stopDistance;
    [SerializeField] protected LayerMask raycastLayerMask;
    protected bool targetInStopDistance;
    float countAttack = float.MaxValue;
    protected bool playerInRaycast;
    [Header("Hit box Attack")]
    [SerializeField] protected Vector2 sizeHitBoxAttack;
    [SerializeField] protected Transform hitAttackPos;

    [Header("WayPoint Settings")]
    [SerializeField] protected bool followPath;
    [SerializeField] protected float timeBetweenWaypoints;
    [SerializeField] protected Transform[] wayPoints;
    protected int sizeWayPoints;
    protected int nextPoint = 0;
    protected float countWaypoints = 0;
    protected float distanceToTarget;

    //Componentes
    protected Rigidbody2D rb2d;
    protected HealthPlayer healthPlayer;
    protected Animator anim;
    [SerializeField] protected FOV fov;
    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
    protected virtual void Start()
    {
        sizeWayPoints = wayPoints.Length;
        currentSpeed = speed;
        initPosition = transform.position;
        target = GameObject.FindGameObjectWithTag("Player").transform;
        healthPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<HealthPlayer>();
    }

    void Update()
    {
        distanceToTarget = Vector2.Distance(transform.position, target.position);
        
        //Si el player esta en el rango, en el Raycast y en el FOV
        targetInRange = radius >= distanceToTarget && PlayerInRaycast();

        //Si el player esta en la stopDistance
        targetInStopDistance = stopDistance >= distanceToTarget;

        //Si el Enemigo esta en su posicion inicial
        isInitPos = Vector2.Distance(transform.position, initPosition) <= 0;

        anim.SetFloat("SpeedX", Mathf.Abs(rb2d.velocity.x));
    }
    void FixedUpdate()
    {
        //Comportamiento del enemigo
        StatesEnemy();

        //Seguir el Path si no esta el player en el rango
        dirEnemy = Path(dirEnemy);

        Flip();
        if(canMove)
        rb2d.velocity = new Vector2(dirEnemy.normalized.x * currentSpeed, rb2d.velocity.y);
    }

    public virtual Vector2 Path(Vector2 dirEnemy)
    {
        
        if (followPath && !targetInRange && !fov.IsInFov())
        {
            Transform currentWaypoint = wayPoints[nextPoint];

            float distanteToNextWaypoint = Vector2.Distance(transform.position, currentWaypoint.position);

            dirEnemy = currentWaypoint.position - transform.position;

            if (distanteToNextWaypoint <= 40)
            {
                //Pasar al siguiente Waypoint
                countWaypoints += Time.deltaTime;
                canMove = false;
                if (countWaypoints >= timeBetweenWaypoints)
                {
                    NextWaypoint();
                    countWaypoints = 0;
                    canMove = true;
                }
            }
        }

        return dirEnemy;
    }

    public virtual void StatesEnemy()
    {
       
    }
    public virtual void OnCollEnter(Collision2D col) {;}
    public virtual void OnTrigEnter(Collider2D col) {;}
    public virtual void OnTrigStay(Collider2D col) {;}
    public virtual void Attack() {;}
    public int Damage()
    {
        return damage;
    }
    protected void NextWaypoint()
    {
        currentSpeed = 0;
        nextPoint++;
        if (nextPoint >= sizeWayPoints)
        {
            nextPoint = 0;
        }
        currentSpeed = speed;
    }
    protected void Flip()
    {
        if (rb2d.velocity.normalized.x < 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }     
        else if(rb2d.velocity.normalized.x > 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }
   
    public bool IsMoving()
    {
        return rb2d.velocity.x != 0;
    }

    protected bool PlayerInRaycast()
    {        
        RaycastHit2D hit = Physics2D.Raycast(transform.position, target.position - transform.position,radius, raycastLayerMask);
        
        Debug.DrawRay(transform.position, target.position - transform.position);

        if(hit.collider != null)
        {
            if (hit.collider.CompareTag("Player"))
            {
                playerInRaycast = true;
                return playerInRaycast;
            }
        }
        playerInRaycast =  false;
        return playerInRaycast;

    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            countAttack += Time.fixedDeltaTime;
            if (countAttack >= timeToAttack)
            {
                Attack();
                countAttack = 0;
            }
        }
        OnTrigStay(collision); 
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnTrigEnter(collision);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnCollEnter(collision);
    }
    private void OnDrawGizmos()
    {
        if (targetInRange)
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }
        Gizmos.DrawWireSphere(transform.position, radius);
        Gizmos.DrawWireSphere(transform.position, stopDistance);

        Gizmos.color = Color.blue;
        if(hitAttackPos!= null)
        Gizmos.DrawWireCube(hitAttackPos.position, sizeHitBoxAttack);

    }
}
