﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalBoss : Enemy
{
    [Header("Boss Settings")]
    [SerializeField]Phases currentPhase;
    [SerializeField] enum Phases { PHASE_1,PHASE_2,PHASE_3};
    [SerializeField]bool followPlayer;
    [SerializeField] HealthBoss healthBoss;
    [SerializeField] Transform tpPosBoss;
    [SerializeField] float timebtwTeleport;
    [SerializeField] float offsetX_TP = 70;

    [Header("Zombie Settings")]
    [SerializeField] GameObject zombiePrefab;
    [SerializeField] Transform spawnPoint;
    [SerializeField] float timeBtwSpawn;

    float counter = 0;
    const int maxZombies = 2;
    int currentZombies = 0;
    int zombiesAlive = maxZombies;
    bool hasSpawnedAll;
    bool teleport;
    bool canTeleport = true;
    
    protected override void StatesEnemy()
    {
        switch (currentPhase)
        {
            case Phases.PHASE_1:
                Phase1();
                break;

            case Phases.PHASE_2:
                Phase2();
                break;

            case Phases.PHASE_3:
                Phase3();
                break;
        }

        FlipManager(dir.normalized.x);
    }
    private void Phase3()
    {
        if (targetInStopDistance && canTeleport)
        {
            canTeleport = false;
            dir = Vector2.zero;
            StartCoroutine(TeleportAttack());
        }
        if (followPlayer)
        {
           dir = (target.position - transform.position).normalized;

        }
        
    }
    private void Phase2()
    {
        //Spawn de zombies en la Fase 2
        SpawnZombie();
        if (hasSpawnedAll && !healthBoss.GetShield())
        {
            if (targetInStopDistance)
            {
                countAttack += Time.deltaTime;
                if (countAttack >= timeToAttack)
                {
                    anim.SetTrigger("Attack");
                    countAttack = 0;
                }
                dir = Vector2.zero;
            }
            else
            {
                dir = (target.position - transform.position).normalized;
            }
        }
    }
    private void Phase1()
    {
        if (targetInStopDistance)
        {
            countAttack += Time.deltaTime;
            if (countAttack >= timeToAttack)
            {
                anim.SetTrigger("Attack");
                countAttack = 0;
            }
            dir = Vector2.zero;
        }
        else
        {
            dir = (target.position - transform.position).normalized;
        }
    }
    private void SpawnZombie()
    {
        if (!hasSpawnedAll)
        {

            dir = Vector2.zero;
            counter += Time.deltaTime;
            if (counter >= timeBtwSpawn && currentZombies < maxZombies)
            {
                anim.SetTrigger("Invoke");
                GetComponent<HealthBoss>().SetShield(true);
                counter = 0;
            }
            hasSpawnedAll = currentZombies >= maxZombies;
        }
    }

    protected override void Path()
    {
        //if (followPath && followPlayer && !targetInStopDistance)
        //{
        //    if (Vector2.Distance(transform.position, wayPoints[nextPoint].position) < 10)
        //    {
        //        countWaypoint += Time.fixedDeltaTime;
        //        dir = Vector2.zero;
        //        if (countWaypoint >= timeInWayPoint)
        //        {
        //            NextWayPoint();
        //            countWaypoint = 0;
        //        }
        //    }
        //    dir = wayPoints[nextPoint].position - transform.position;
        //}
    }

    //Se ejecuta en un evento de la animacion
    protected override void Attack()
    {
        Collider2D col2D = Physics2D.OverlapBox(hitAttackPos.position, sizeHitBoxAttack, 0);
        if (col2D != null)
        {
            if (col2D.CompareTag("Player"))
            {
                col2D.GetComponent<HealthPlayer>().Damage(damage);
            }
        }
    }

    //Se ejecuta en un evento de la animacion del "Invoke"
    void InstantiateZombie()
    {
        if(currentZombies < maxZombies)
        {
            Instantiate(zombiePrefab, spawnPoint.position, Quaternion.identity, null);
            currentZombies++;
        }
    }

    IEnumerator TeleportAttack()
    {
        Vector2 dir = target.position - transform.position;
        Vector2 pos;
        //Derecha
        if(dir.x > 0)
        {
            pos = new Vector2(target.position.x + offsetX_TP, transform.position.y);
        }
        //Izquierda
        else
        {
            pos = new Vector2(target.position.x - offsetX_TP, transform.position.y);
        }

        followPlayer = false;
        anim.SetTrigger("Attack");

        yield return new WaitForSeconds(timebtwTeleport);

        rb2d.position = pos;
        Flip();
        anim.SetTrigger("Attack");

        yield return new WaitForSeconds(1);

        followPlayer = true;
        canTeleport = true;
        
    }
    public void UpdatePhaseManager(float _life,float _maxLife)
    {
        //Fase 2
        if(_life == _maxLife - 1)
        {
            currentPhase = Phases.PHASE_2;
        }
        //Fase 3
        if(_life == _maxLife - 2)
        {
            hasSpawnedAll = false;
            currentZombies = 0;
            currentPhase = Phases.PHASE_3;
        }
    }
    public void DeathAZombie()
    {
        zombiesAlive--;
        if(zombiesAlive <= 0)
        {
            GetComponent<HealthBoss>().SetShield(false);
            zombiesAlive = 0;
        }
    }
    public int GetCurrentZombies() {
        return zombiesAlive;
    }

}
