using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buu : CharacterController
{
    public void Start()
    {
        mainCamera = Camera.main.transform;
        //initialCameraPosition = cameraSkill.transform.localPosition;
        InitializeHealth(health);
        isLayer = LayerMask.LayerToName(gameObject.layer);
        uiManager = GameObject.Find("UICode").GetComponent<UIManager>();
        initialTranform = transform.position;
        if (isLayer == isPlayer)
        {
            audioPlayer = GameObject.Find("AudioPlayer").GetComponent<AudioManager>();
            collisionLayer = LayerMask.GetMask(isEnemy);
            uiManager.DisplayHealth(health, true);
            uiManager.DisplayEnergy(energy, true);
            uiManager.DisplayRage(rage, true);
            targetEnemy = GameObject.Find(isEnemy).transform;
            skillTwo.layer = gameObject.layer;
        }
        else if (isLayer == isEnemy)
        {
            audioEnemy = GameObject.Find("AudioEnemy").GetComponent<AudioManager>();
            collisionLayer = LayerMask.GetMask(isPlayer);
            uiManager.DisplayHealth(health, false);
            uiManager.DisplayEnergy(energy, false);
            uiManager.DisplayRage(rage, false);
            targetEnemy = GameObject.Find(isPlayer).transform;
            skillTwo.layer = gameObject.layer;
        }
        RotationEnemy();

    }

    // Update is called once per frame
    void Update()
    {
        if (targetEnemy == null)
        {
            if (isLayer == isEnemy)
            {
                targetEnemy = GameObject.Find(isPlayer).transform;
            }
            else
            {
                targetEnemy = GameObject.Find(isEnemy).transform;
            }
        }
        stateInfo = playerAnim.GetCurrentAnimatorStateInfo(0);
        BO3();
        ResetComboState();
        AttackPoint();
        MoveFireball();
        Ki();
        IncreaseRage();
    }

}
