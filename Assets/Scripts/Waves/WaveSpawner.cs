﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [Header("Enabled Waves")]
    [SerializeField] bool spawnInteracticeWave;
    [SerializeField] bool spawnPushWave;
    [SerializeField] bool spawnFlashWave;

    [Header("Flash Wave")]
    [SerializeField] Animator animFlashWave;
    
    [Header("Interactive Wave")]
    [SerializeField] Animator animInteractiveWave;
    [SerializeField] float coolDown_InteractiveWave = 0.5f;
    bool doingInteractiveWave;

    [Header("Push Wave")]
    [SerializeField] Animator animPushWave;
    [SerializeField] float coolDown_pushWave = 0.5f;
    bool doingPushWave;

    [Header("Step Wave")]
    [SerializeField] Animator animStepWave;

    [Header("Ground Wave")]
    [SerializeField] GameObject groundWavePrefab;

    PlayerMovement player;
    Animator animPlayer;

    //Counters
    float countInteractive = float.MaxValue;
    float countPushWave = float.MaxValue;

    private void Awake()
    {
        player = GetComponent<PlayerMovement>();
        animPlayer = GetComponent<Animator>();
    }
    private void Start()
    {
        //Poner en gris los iconos si no se pueden usar las ondas
        if (!spawnPushWave)
        {
            HUDManager.singletone.SetPushWaveIcon(0, 1);
        }
        if (!spawnInteracticeWave)
        {
            HUDManager.singletone.SetInteractiveWaveIcon(0, 1);
        }
    }
    private void Update()
    {
        AnimControllers();
        Cooldowns();
    }

    private void Cooldowns()
    {
        //Push Wave
        if (doingPushWave)
        {
            countPushWave += Time.deltaTime;
            doingPushWave = true;
            if (countPushWave >= coolDown_pushWave)
            {
                doingPushWave = false;             
            }
        }
        //Interactive Wave
        if (doingInteractiveWave)
        {
            countInteractive += Time.deltaTime;
            doingInteractiveWave = true;
            if (countInteractive >= coolDown_InteractiveWave)
            {
                doingInteractiveWave = false;
            }
        }

        //Actualizando el valor de los Iconos de las ondas
        if (doingPushWave)
            HUDManager.singletone.SetPushWaveIcon(countPushWave, coolDown_pushWave);

        if (doingInteractiveWave)
            HUDManager.singletone.SetInteractiveWaveIcon(countInteractive, coolDown_InteractiveWave);
    }

    private void AnimControllers()
    {
        animStepWave.SetBool("IsMoving", player.IsMoving() && !player.IsWallSliding() && !player.IsStealth());
        animStepWave.SetFloat("Speed", player.CurrentVelocityX());
        animFlashWave.SetBool("Spending", GameManager.singletone.IsSpeendingEnergy());
    }

    #region DoPlayerWaves
    //Se ejecutan en el InputManager
    public void DoPlayerFlashWave()
    {
        if (!spawnFlashWave) return;
        if (GameManager.singletone.GetEnergy() > 0)
        {
            animPlayer.SetTrigger("FlashWave");
            
        }
    }
    public void DoPlayerInteractiveWave()
    {
        if (!spawnInteracticeWave) return;
        
        if (!doingInteractiveWave)
        {
            doingInteractiveWave = true;
            countInteractive = 0;
            animPlayer.SetTrigger("InteractionWave");
        }
    }
    public void DoPlayerPushWave()
    {
        if (!spawnPushWave) return;

        if (!doingPushWave)
        {
            doingPushWave = true;
            countPushWave = 0;
            animPlayer.SetTrigger("PushWave");
        }
    }
    #endregion

    #region DoWaves

    //Se ejecuta en la animacion del player
    void DoPushWave()
    {
        animPushWave.SetTrigger("DoWave");
        SimpleCameraShakeInCinemachine.singletone.DoCameraShake();
    }
    //Se ejecuta en la animacion del player
    void DoInteractiveWave()
    {
        animInteractiveWave.SetTrigger("DoWave");
        SimpleCameraShakeInCinemachine.singletone.DoCameraShake();
    }
    //Se ejecuta en la animacion del player
    void DoFlashWave()
    {
        GameManager.singletone.SetSpending();
        SimpleCameraShakeInCinemachine.singletone.DoCameraShake();
    }

    public void DoGroundWave()
    {
        GameObject go = Instantiate(groundWavePrefab, player.transform.position, Quaternion.identity, null);
        Destroy(go, 1);
    }

    #endregion
}
