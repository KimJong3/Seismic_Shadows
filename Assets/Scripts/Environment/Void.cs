﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Void : MonoBehaviour
{
    TPlayerManager tpPlayer;
    private void Awake()
    {
        tpPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<TPlayerManager>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //VARIABLES
        //tpPlayer: instancia del script 'TPlayerManager'.

        //EXPLICACION
        //Cuando el player colisiona con el objeto invisible 'Void',
        //se llama a la función para que se teletransporte al checkpoint y 
        //reciba daño.

        //PARAMETROS
        //Se indica los puntos de vida que inflinge ese void

        if(collision.collider.tag == "Player")
        {
            tpPlayer.TeleportToTPVoid(1);
        }
        if (collision.collider.tag == "Enemy")
        {
            if(collision.gameObject.TryGetComponent(out HealthEnemies h))
            {
                h.Damage(3);
            }
        }
    }
}
