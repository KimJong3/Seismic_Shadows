﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlindurGuardian : Enemy
{
    bool followPlayer;
    bool flipOnce = true;
    [Header("Force Push")]
    [SerializeField] float forcePush;
    protected override void StatesEnemy()
    {
        //Comenzar a perseguir al player
        if (targetInRaycast || followPlayer)
        {
            countStartFollow += Time.fixedDeltaTime;
            if(countStartFollow >= timeToStartFollow)
            {
                dir = target.position - transform.position;
                followPlayer = true;
            }
            
        }
        if(targetInStopDistance && targetInRaycast)
        {
            Attack();
            dir = Vector2.zero;
        }

        //Cuando el enemigo deja de perseguir al player
        if (!targetInRaycast && followPlayer)
        {
            followPlayer = false;
            countStartFollow = 0;
            
                 
        }

        //Comprobar si ha llegado a su posicion inicial para parar el movimiento
        if (!followPath && !followPlayer)
        {
            if (Vector2.Distance(transform.position, initPos) > 10)
            {
                dir = initPos - transform.position;
                flipOnce = false;
            }
            else
            {
                dir = Vector2.zero;
                if (spawnFlipped && !flipOnce)
                {
                    Flip();
                    flipOnce = true;
                }
            }
        }
        FlipManager(rb2d.velocity.normalized.x);
    }
    protected override void Path()
    {
        if (followPath && !followPlayer && !targetInStopDistance)
        {
            if (Vector2.Distance(transform.position, wayPoints[nextPoint].position) < 10)
            {
                countWaypoint += Time.fixedDeltaTime;
                dir = Vector2.zero;
                if (countWaypoint >= timeInWayPoint)
                {
                    NextWayPoint();
                    countWaypoint = 0;
                }
            }
            dir = wayPoints[nextPoint].position - transform.position; 
        }
    }

    protected override void Attack()
    {
        Collider2D col2D = Physics2D.OverlapBox(hitAttackPos.position, sizeHitBoxAttack, 0, layerMaskEnvironent);
        if(col2D != null)
        {
            if (col2D.CompareTag("Player"))
            {
                countAttack += Time.deltaTime;
                if(countAttack >= timeToAttack)
                {
                    anim.SetTrigger("Attack");
                    col2D.GetComponent<HealthPlayer>().Damage(damage);
                    countAttack = 0;
                }
            }
        }
    }
  
    void ActiveMovement()
    {
        canMove = true;
    }
    protected override void OnTrigEnter(Collider2D col)
    {
        if (col.CompareTag("PushWave"))
        {
            Vector2 dirForce = transform.position - col.transform.position;

            canMove = false;
            rb2d.velocity = Vector2.zero;
            rb2d.AddForceAtPosition(dirForce.normalized * forcePush, transform.position,ForceMode2D.Impulse);
            Invoke("ActiveMovement", 1);
        }
    }
    protected override void OnCollEnter(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            col.gameObject.GetComponent<HealthPlayer>().Damage(1);
        }
    }
}
