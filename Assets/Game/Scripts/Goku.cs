using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Goku : CharacterController
{
    public bool isSuperSaiyan = false;
    public GameObject gokuSuperSaiyanPrefab;

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
            SetLayerRecursively(skillThree, skillThree.layer);
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
            SetLayerRecursively(skillThree, skillThree.layer);
        }
        RotationEnemy();

        // Instantiate Skill Three
        GameObject spiritboom = Instantiate(skillThree);
        spiritboom.SetActive(false);
        spiritboom.name = "SpiritBoomOfGoku";
        skillThree = spiritboom;
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
        if (skillThree.activeInHierarchy)
        {
            ParticleLifetimesAndMoveSpiritBoom(skillThree.transform);
            CameraSmooth(cameraSkill, Vector3.up);
            cameraSkill.enabled = true;
            CheckCameraCoreGoku(skillThree.transform);
        }
        else if (!skillThree.activeInHierarchy)
        {
            isSpiritRuning = false;
            if (Input.GetKeyDown(KeyCode.Alpha3) && isLayer == isPlayer && uiManager.isCountdownFinished)
            {
                if (80f > energy) return;
                ApplyReduceEnergy(80f);
                skillThree.SetActive(true);
                playerAnim.SetTrigger("spiritboomfirst");
                playerAnim.SetTrigger("spiritboommiddle");
                audioPlayer.PlaySkillSFX("Spirit");
                audioPlayer.PlayVoiceSFX("VoiceSpirit");
            }
            else if (Input.GetKeyDown(KeyCode.L) && isLayer == isEnemy && uiManager.isCountdownFinished)
            {
                if (80f > energy) return;
                ApplyReduceEnergy(80f);
                skillThree.SetActive(true);
                playerAnim.SetTrigger("spiritboomfirst");
                playerAnim.SetTrigger("spiritboommiddle");
                audioEnemy.PlaySkillSFX("Spirit");
                audioEnemy.PlayVoiceSFX("VoiceSpirit");
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
        damage *= 1.5f;
        GameObject gokuSuperSaiyan = Instantiate(gokuSuperSaiyanPrefab, transform.position, Quaternion.identity);
        Goku gokuSuperSaiyanScript = gokuSuperSaiyan.GetComponent<Goku>();
        gokuSuperSaiyanScript.health = health;
        gokuSuperSaiyanScript.damage = damage;
        gokuSuperSaiyanScript.rage = rage;
        gokuSuperSaiyan.layer = gameObject.layer;
        if (isLayer == isEnemy)
        {
            gokuSuperSaiyan.name = "Enemy";
            Skin(gokuSuperSaiyan.transform);
        }
        else
        {
            gokuSuperSaiyan.name = "Player";
            Skin(gokuSuperSaiyan.transform);
        }
        Destroy(gameObject);
    }

    private void Skin(Transform parent)
    {
        SkinnedMeshRenderer body = parent.transform.Find("Body").GetComponent<SkinnedMeshRenderer>();
        Material[] materials = body.materials;
        Texture2D texture = new Texture2D(2, 2);
        byte[] fileData = File.ReadAllBytes(PlayerPrefs.GetString($"Skin{parent.name}"));
        texture.LoadImage(fileData);
        foreach (Material material in materials)
        {
            if (material.name == "tex3.png (Instance)")
            {
                Debug.Log("?");
                material.SetTexture("_BaseMap", texture);
                break;
            }
        }
    }
}
