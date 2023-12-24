using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vegeta : CharacterController
{
    public GameObject vegetaSuperSaiyanPrefab;
    public bool isSuperSaiyan = false;
    public void Start()
    {
        mainCamera = Camera.main.transform;
        //initialCameraPosition = cameraSkill.transform.localPosition;
        InitializeHealth(health);
        uiManager = GameObject.Find("UICode").GetComponent<UIManager>();
        isLayer = LayerMask.LayerToName(gameObject.layer);
        initialTranform = transform.position;
        if (isLayer == isPlayer)
        {
            audioPlayer = GameObject.Find("AudioPlayer").GetComponent<AudioManager>();
            collisionLayer = LayerMask.GetMask(isEnemy);
            uiManager.DisplayHealth(health, true);
            uiManager.DisplayEnergy(energy, true);
            uiManager.DisplayRage(rage, true);
            targetEnemy = GameObject.Find(isEnemy).transform;
        }
        else if (isLayer == isEnemy)
        {
            audioEnemy = GameObject.Find("AudioEnemy").GetComponent<AudioManager>();
            collisionLayer = LayerMask.GetMask(isPlayer);
            uiManager.DisplayHealth(health, false);
            uiManager.DisplayEnergy(energy, false);
            uiManager.DisplayRage(rage, false);
            targetEnemy = GameObject.Find(isPlayer).transform;
        }
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
        Rotation();
        BO3();
        ResetComboState();
        AttackPoint();
        MoveFireball();
        Ki();
        IncreaseRage();
        if (Input.GetKeyDown(KeyCode.Alpha4) && isLayer == isPlayer && uiManager.isCountdownFinished && !isSuperSaiyan)
        {
            if (!isSuperSaiyan)
            {
                TransformToSuperSaiyan();
            }
        }
        else if (Input.GetKeyDown(KeyCode.H) && isLayer == isEnemy && uiManager.isCountdownFinished && !isSuperSaiyan)
        {
            if (!isSuperSaiyan)
            {
                TransformToSuperSaiyan();
            }
        }
       
    }
    private void TransformToSuperSaiyan()
    {
        if (energy < 100) return;
        isSuperSaiyan = true;
        damage *= 1.5f;
        GameObject vegetaSuperSaiyan = Instantiate(vegetaSuperSaiyanPrefab, transform.position, Quaternion.identity);
        Vegeta vegetaSuperSaiyanScript = vegetaSuperSaiyan.GetComponent<Vegeta>();
        vegetaSuperSaiyanScript.health = health;
        vegetaSuperSaiyanScript.damage = damage;
        vegetaSuperSaiyanScript.rage = rage;

        vegetaSuperSaiyan.layer = gameObject.layer;
        if (isLayer == isEnemy)
        {
            vegetaSuperSaiyan.name = "Enemy";
        }
        else
        {
            vegetaSuperSaiyan.name = "Player";
        }

        Destroy(gameObject);
    }

}
