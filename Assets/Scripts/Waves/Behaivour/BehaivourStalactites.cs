﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaivourStalactites : BehaivourWave
{
    protected HealthPlayer playerHealth;
    bool activated;
    [SerializeField] int gravityScale;
    private void Start()
    {
        activated = false;
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<HealthPlayer>();
    }
    protected override void ActionOnWave(Collider2D col)
    {
        if (col.gameObject.CompareTag("InteractiveWave"))
        {
            //Esto se ejecuta cuando una onda interactiva choca con este objeto
            rb2d.gravityScale = gravityScale;
            activated = true;
            GetComponent<SpriteRenderer>().sortingLayerName = "AlwaysVisible";
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerHealth.Damage(1);
            this.gameObject.SetActive(false);
        }
        if (collision.gameObject.CompareTag("Ground"))
        {
            this.gameObject.SetActive(false);
        }
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if(collision.gameObject.TryGetComponent(out HealthEnemies h))
            {
                h.Damage(3);
            }
            this.gameObject.SetActive(false);
        }
    }
}
