using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class Gohan : CharacterController
{
    private Vector3 betweenHandsKame;
    public bool isSuperSaiyan = false;
    public GameObject gohanSuperSaiyanPrefab;

    public void Start()
    {
        mainCamera = Camera.main.transform;
        initialCameraPosition = cameraSkill.transform.localPosition;
        uiManager = GameObject.Find("UICode").GetComponent<UIManager>();
        InitializeHealth(health);
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
            skillTwo.layer = gameObject.layer;
            skillThree.layer = gameObject.layer;
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
            skillThree.layer = gameObject.layer;
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
        MoveFireball();
        AttackPoint();
        Ki();
        IncreaseRage();
        if (skillThree.activeInHierarchy)
        {
            skillThree.transform.rotation = Quaternion.LookRotation(transform.forward) * Quaternion.Euler(0f, -90f, 0f);
            ColliderKame(skillThree.transform);
            betweenHandsKame = (RightArmAttackPoint.transform.position + LeftArmAttackPoint.transform.position) / 2;
            CheckChildParticleLifetimesKame(skillThree.transform);
            skillThree.transform.position = betweenHandsKame;
            if (isLayer == isPlayer)
                CameraSmooth(cameraSkill, Vector3.forward);
            else CameraSmooth(cameraSkill, Vector3.back);
            cameraSkill.enabled = true;
            CheckCameraCoreGohan(skillThree.transform);
        }
        if (!skillThree.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.Alpha3) && isLayer == isPlayer && uiManager.isCountdownFinished)
            {
                if (80f > energy) return;
                ApplyReduceEnergy(80f);
                skillThree.SetActive(true);
                audioPlayer.PlayVoiceSFX("SuperKamehame");
                audioPlayer.PlaySkillSFX("SuperCore");
                playerAnim.SetTrigger("superkame");
            }
            else if (Input.GetKeyDown(KeyCode.L) && isLayer == isEnemy && uiManager.isCountdownFinished)
            {
                if (80f > energy) return;
                ApplyReduceEnergy(80f);
                skillThree.SetActive(true);
                audioEnemy.PlayVoiceSFX("SuperKamehame");
                audioEnemy.PlaySkillSFX("SuperCore");
                playerAnim.SetTrigger("superkame");
            }
        }
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
        damage *= 0.5f;
        GameObject gohanSuperSaiyan = Instantiate(gohanSuperSaiyanPrefab, transform.position, Quaternion.identity);
        Gohan gohanSuperSaiyanScript = gohanSuperSaiyan.GetComponent<Gohan>();
        gohanSuperSaiyanScript.health = health;
        gohanSuperSaiyanScript.damage = damage;
        gohanSuperSaiyanScript.rage = rage;
        gohanSuperSaiyan.layer = gameObject.layer;
        if (isLayer == isEnemy)
        {
            gohanSuperSaiyan.name = "Enemy";
            Skin(gohanSuperSaiyan.transform);
        }
        else
        {
            gohanSuperSaiyan.name = "Player";
            Skin(gohanSuperSaiyan.transform);
        }
        
        Destroy(gameObject);
    }
    private void Skin(Transform parent)
    {
        SkinnedMeshRenderer body = parent.transform.Find("Gohan").GetComponent<SkinnedMeshRenderer>();
        Material[] materials = body.materials;
        Texture2D texture = new Texture2D(2, 2);
        byte[] fileData = File.ReadAllBytes(PlayerPrefs.GetString($"Skin{parent.name}"));
        texture.LoadImage(fileData);
        foreach (Material material in materials)
        {
            if (material.name == "Gohan - Body (Instance)")
            {
                Debug.Log("?");
                material.SetTexture("_BaseMap", texture);
                break;
            }
        }
    }
}
