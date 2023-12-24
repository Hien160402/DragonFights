
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    // Move
    public GameObject ki;
    public GameObject kiFull;
    public List<ParticleSystem> kiGround;
    public Animator playerAnim;
    public Rigidbody playerRb;
    public Transform targetEnemy;
    private bool activateTimerToReset;
    private float default_combo_timer = 2f;
    private float current_combo_timer;
    private ComboState current_combo_state;
    public AnimatorStateInfo stateInfo;
    protected Vector3 initialTranform;

    public float w_speed, wb_speed, rn_speed;
    public bool isWalking;
    public bool isWalkingBack = false;
    private bool isRunning = false;

    public float jumpForce = 5f;
    public bool isOnGround = true;
    private bool isJump = false;
    public bool isFlipped = false;
    public bool isBlock = false;

    // Collider
    public LayerMask collisionLayer;
    public float radius = 1f;
    public float damage = 10f;
    public GameObject hit_FX;

    // Layer
    protected string isLayer;
    protected const string isPlayer = "Player";
    protected const string isEnemy = "Enemy";

    // Attack Point
    public GameObject RightArmAttackPoint;
    public GameObject LeftArmAttackPoint;
    public GameObject RightLegAttackPoint;
    public GameObject LeftLegAttackPoint;
    public GameObject RightForeArmAttackPoint;

    // Apply Dame
    public float health = 100f;
    private float initialHealth;
    public float energy = 15f;
    public bool characterDied;
    private bool healing = false;
    public UIManager uiManager;


    //Skill
    public GameObject skillOne;
    public GameObject skillTwo;
    public GameObject skillThree;
    public GameObject shield;
    private bool isPooling = true;
    private bool isKame = false;
    public bool isSpiritRuning = false;
    public List<GameObject> listSkillOne = new List<GameObject>();
    public List<Quaternion> listSkillOneRotation = new List<Quaternion>();
    public int speedFireball = 20;
    private Vector3 betweenHands;

    // Camera 
    public Camera cameraSkill;
    public Transform mainCamera;
    protected Vector3 initialCameraPosition;
    private Vector3 characterScreenPosition;
    private Vector3 otherCharacterScreenPosition;

    //Rage
    private float rageIncreaseSpeed = 1f;
    public float rage = 0f;
    private float targetRage = 100f;
    private float timeElapsed = 0f;
    private bool isUsingRage = false;
    private float initialDamage;

    // Audio
    protected AudioManager audioPlayer;
    protected AudioManager audioEnemy;

    public void InitializeHealth(float initialHealth)
    {
        this.initialHealth = initialHealth;
    }
    protected void Rotation()
    {
        transform.LookAt(targetEnemy);
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
    }
    protected void RotationEnemy()
    {
        transform.rotation = Quaternion.LookRotation(targetEnemy.position - transform.position);
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
    }
    protected bool RestrictAction()
    {
        if (!ki.activeInHierarchy && !skillTwo.activeInHierarchy && !stateInfo.IsName("superkame") && !stateInfo.IsName("SpiritBoomMiddle") && !stateInfo.IsName("SpiritBoomFirst") && !stateInfo.IsName("knockup") && !stateInfo.IsName("knockdown") && !stateInfo.IsName("knockend") && !stateInfo.IsName("standup"))
        {
            return false;
        }
        return true;
    }

    void Movement()
    {
        characterScreenPosition = Camera.main.WorldToScreenPoint(transform.position);
        otherCharacterScreenPosition = Camera.main.WorldToScreenPoint(targetEnemy.transform.position);
        if (characterDied)
            return;
        if (isLayer == isPlayer)
        {
            if (Input.GetKey(KeyCode.C) && isOnGround && uiManager.isCountdownFinished)
            {
                if (!isBlock)
                {
                    isBlock = true;
                    playerAnim.SetTrigger("block");
                    playerAnim.ResetTrigger("idle");
                }
            }
            if (Input.GetKey(KeyCode.D) && uiManager.isCountdownFinished && !isBlock)
            {
                if (Input.GetKeyDown(KeyCode.D))
                {
                    audioPlayer.PlayMoveLoopSFX("Walk");
                }
                if (Input.GetKeyDown(KeyCode.LeftShift))
                    audioPlayer.PlayMoveLoopSFX("Run");
                if (characterScreenPosition.x < otherCharacterScreenPosition.x)
                {
                    transform.Translate(Vector3.forward * w_speed * Time.deltaTime);
                    playerAnim.SetTrigger("walk");
                }
                else
                {
                    transform.Translate(Vector3.back * w_speed * Time.deltaTime);
                    playerAnim.SetTrigger("walkback");
                }
                RotationEnemy();
                playerAnim.ResetTrigger("idle");
            }
            else if (Input.GetKey(KeyCode.A) && uiManager.isCountdownFinished && !isBlock)
            {
                if (Input.GetKeyDown(KeyCode.A))
                {
                    audioPlayer.PlayMoveLoopSFX("Walk");
                }
                if (characterScreenPosition.x < otherCharacterScreenPosition.x)
                {
                    transform.Translate(Vector3.back * w_speed * Time.deltaTime);
                    playerAnim.SetTrigger("walkback");
                }
                else
                {
                    transform.Translate(Vector3.forward * w_speed * Time.deltaTime);
                    playerAnim.SetTrigger("walk");
                }
                RotationEnemy();
                playerAnim.ResetTrigger("idle");
            }
            else if (Input.GetKey(KeyCode.W) && !isJump && uiManager.isCountdownFinished && !isBlock)
            {
                if (Input.GetKeyDown(KeyCode.W))
                {
                    audioPlayer.PlayMoveLoopSFX("Walk");
                }
                RotationEnemy();
                transform.Translate(mainCamera.forward * 3f * Time.deltaTime, Space.World);
                playerAnim.SetTrigger("walkback");
                playerAnim.ResetTrigger("idle");
            }
            else if (Input.GetKey(KeyCode.S) && !isJump && uiManager.isCountdownFinished && !isBlock)
            {
                if (Input.GetKeyDown(KeyCode.S))
                {
                    audioPlayer.PlayMoveLoopSFX("Walk");
                }
                RotationEnemy();
                transform.Translate(-mainCamera.forward * 3f * Time.deltaTime, Space.World);
                playerAnim.SetTrigger("walkback");
                playerAnim.ResetTrigger("idle");
            }
            else if (!Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.C) && uiManager.isCountdownFinished)
            {
                if (isFlipped)
                {
                    playerAnim.ResetTrigger("walkback");
                    isWalking = true;
                }
                else
                {
                    playerAnim.ResetTrigger("walk");
                    isWalking = false;
                }
                playerAnim.ResetTrigger("block");
                isBlock = false;
                playerAnim.SetTrigger("idle");
            }
            if (Input.GetKeyUp(KeyCode.D))
            {
                audioPlayer.StopMoveLoopSFX("Walk");
            }
            if (Input.GetKeyUp(KeyCode.A))
            {
                audioPlayer.StopMoveLoopSFX("Walk");
            }
            if (Input.GetKeyUp(KeyCode.W))
            {
                audioPlayer.StopMoveLoopSFX("Walk");
            }
            if (Input.GetKeyUp(KeyCode.S))
            {
                audioPlayer.StopMoveLoopSFX("Walk");
            }
            if (Input.GetKey(KeyCode.LeftShift) && uiManager.isCountdownFinished)
            {
                if (Input.GetKey(KeyCode.D) && characterScreenPosition.x < otherCharacterScreenPosition.x || Input.GetKey(KeyCode.A) && characterScreenPosition.x > otherCharacterScreenPosition.x)//!isJump
                {
                    isRunning = true;
                    w_speed = 10f;
                    playerAnim.SetTrigger("run");
                    playerAnim.ResetTrigger("walk");
                }
                else
                {
                    isRunning = false;
                }
            }
            else
            {
                isRunning = false;
            }

            if (!isRunning)
            {
                w_speed = 3f;
            }

            if (isJump)
            {
                isWalking = false;
            }

            if (Input.GetKeyDown(KeyCode.X) && isOnGround && uiManager.isCountdownFinished)
            {
                audioPlayer.PlayVoiceSFX(gameObject.tag + "Jump");
                playerAnim.SetTrigger("jumping");
                playerAnim.SetTrigger("falling");
                playerAnim.ResetTrigger("idle");
                isOnGround = false;
                isJump = true;
                //playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); 
                playerRb.AddForce(transform.up * jumpForce);
            }
            if (Input.GetKeyDown(KeyCode.Z) && isOnGround && uiManager.isCountdownFinished)
            {
                if (20f > energy) return;
                audioPlayer.PlaySkillSFX("Teleport");
                ApplyReduceEnergy(20f);
                PerformInstantMoveSkill();
                RotationEnemy();
            }
            if (isLayer == isPlayer && uiManager.playerWins == 2)
            {
                playerAnim.SetTrigger("victory");
            }
        }
        if (isLayer == isEnemy)
        {
            if (Input.GetKey(KeyCode.B) && isOnGround && uiManager.isCountdownFinished)
            {
                if (!isBlock)
                {
                    isBlock = true;
                    playerAnim.SetTrigger("block");
                    playerAnim.ResetTrigger("idle");
                }
            }
            if (Input.GetKey(KeyCode.RightArrow) && uiManager.isCountdownFinished && !isBlock)
            {
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    audioEnemy.PlayMoveLoopSFX("Walk");
                }
                if (Input.GetKeyDown(KeyCode.RightShift))
                    audioEnemy.PlayMoveLoopSFX("Run");
                if (characterScreenPosition.x < otherCharacterScreenPosition.x)
                {
                    transform.Translate(Vector3.forward * w_speed * Time.deltaTime);
                    playerAnim.SetTrigger("walk");
                }
                else
                {
                    transform.Translate(Vector3.back * w_speed * Time.deltaTime);
                    playerAnim.SetTrigger("walkback");
                }
                RotationEnemy();
                playerAnim.ResetTrigger("idle");
            }
            else if (Input.GetKey(KeyCode.LeftArrow) && uiManager.isCountdownFinished && !isBlock)
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    audioEnemy.PlayMoveLoopSFX("Walk");
                }
                if (characterScreenPosition.x < otherCharacterScreenPosition.x)
                {
                    transform.Translate(Vector3.back * w_speed * Time.deltaTime);
                    playerAnim.SetTrigger("walkback");
                }
                else
                {
                    transform.Translate(Vector3.forward * w_speed * Time.deltaTime);
                    playerAnim.SetTrigger("walk");
                }
                RotationEnemy();
                playerAnim.ResetTrigger("idle");
            }
            else if (Input.GetKey(KeyCode.UpArrow) && !isJump && uiManager.isCountdownFinished && !isBlock)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    audioEnemy.PlayMoveLoopSFX("Walk");
                }
                RotationEnemy();
                transform.Translate(mainCamera.forward * 3f * Time.deltaTime, Space.World);
                playerAnim.SetTrigger("walkback");
                playerAnim.ResetTrigger("idle");
            }
            else if (Input.GetKey(KeyCode.DownArrow) && !isJump && uiManager.isCountdownFinished && !isBlock)
            {
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    audioEnemy.PlayMoveLoopSFX("Walk");
                }
                RotationEnemy();
                transform.Translate(-mainCamera.forward * 3f * Time.deltaTime, Space.World);
                playerAnim.SetTrigger("walkback");
                playerAnim.ResetTrigger("idle");
            }
            else if (!Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.B) && uiManager.isCountdownFinished)
            {
                if (isFlipped)
                {
                    playerAnim.ResetTrigger("walkback");
                    isWalking = true;
                }
                else
                {
                    playerAnim.ResetTrigger("walk");
                    isWalking = false;
                }
                playerAnim.ResetTrigger("block");
                isBlock = false;
                playerAnim.SetTrigger("idle");
            }
            if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                audioEnemy.StopMoveLoopSFX("Walk");
            }
            if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                audioEnemy.StopMoveLoopSFX("Walk");
            }
            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                audioEnemy.StopMoveLoopSFX("Walk");
            }
            if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                audioEnemy.StopMoveLoopSFX("Walk");
            }
            if (Input.GetKey(KeyCode.RightShift) && uiManager.isCountdownFinished)
            {
                if (Input.GetKey(KeyCode.RightArrow) && characterScreenPosition.x < otherCharacterScreenPosition.x || Input.GetKey(KeyCode.LeftArrow) && characterScreenPosition.x > otherCharacterScreenPosition.x)//!isJump
                {
                    isRunning = true;
                    w_speed = 10f;
                    playerAnim.SetTrigger("run");
                    playerAnim.ResetTrigger("walk");
                }
                else
                {
                    isRunning = false;
                }
            }
            else
            {
                isRunning = false;
            }

            if (!isRunning)
            {
                w_speed = 3f;
            }

            if (isJump)
            {
                isWalking = false;
            }

            if (Input.GetKeyDown(KeyCode.N) && isOnGround && uiManager.isCountdownFinished)
            {
                audioEnemy.PlayVoiceSFX(gameObject.tag + "Jump");
                playerAnim.SetTrigger("jumping");
                playerAnim.SetTrigger("falling");
                playerAnim.ResetTrigger("idle");
                isOnGround = false;
                isJump = true;
                //playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); 
                playerRb.AddForce(transform.up * jumpForce);
            }
            if (Input.GetKeyDown(KeyCode.M) && isOnGround && uiManager.isCountdownFinished)
            {
                if (20f > energy) return;
                audioEnemy.PlaySkillSFX("Teleport");
                ApplyReduceEnergy(20f);
                PerformInstantMoveSkill();
                RotationEnemy();
            }
            if (isLayer == isEnemy && uiManager.enemyWins == 2)
            {
                playerAnim.SetTrigger("victory");
            }
        }
    }

    protected void Ki()
    {
        if (characterDied)
            return;
        if (isLayer == isPlayer)
        {
            if (stateInfo.IsName("chargekilast") || stateInfo.IsName("chargekimidle") || stateInfo.IsName("chargeki"))
            {
                ki.SetActive(true);
            }
            if (!stateInfo.IsName("chargekilast") && !stateInfo.IsName("chargekimidle") && !stateInfo.IsName("chargeki"))
            {
                if (!kiFull.activeInHierarchy)
                {
                    audioPlayer.StopAuraLoopSFX("Aura");
                }
                ki.SetActive(false);
                playerAnim.ResetTrigger("chargekilast");
            }
            if (!RestrictAction() && uiManager.isCountdownFinished)
            {
                if (Input.GetKeyDown(KeyCode.R))
                {
                    audioPlayer.PlayVoiceSFX(gameObject.tag + "VoiceKi");
                    audioPlayer.PlayAuraSFX("AuraBegin");
                    audioPlayer.PlayAuraLoopSFX("Aura");
                    playerAnim.SetTrigger("chargeki");
                    playerAnim.SetTrigger("chargekimidle");
                }
                Movement();
                ComboAttacks();
                Skill();
                Rage();
            }
            if (Input.GetKeyUp(KeyCode.R) && uiManager.isCountdownFinished)
            {
                playerAnim.SetTrigger("chargekilast");
                if (kiFull.activeInHierarchy)
                {
                    audioPlayer.PlayAuraLoopSFX("Aura");
                }
            }
            if (ki.activeInHierarchy)
            {
                ApplyIncreaseEnergy(10f);
            }
            if (skillTwo.activeInHierarchy)
            {
                skillTwo.transform.rotation = Quaternion.LookRotation(transform.forward) * Quaternion.Euler(0f, -90f, 0f);
                ColliderKame(skillTwo.transform);
                betweenHands = (RightArmAttackPoint.transform.position + LeftArmAttackPoint.transform.position) / 2;
                CheckChildParticleLifetimesKame(skillTwo.transform);
                skillTwo.transform.position = betweenHands;
            }

            if (kiFull.activeInHierarchy || ki.activeInHierarchy)
            {
                if (GetComponent<Rigidbody>().velocity.y > 0)
                {
                    foreach (ParticleSystem ki in kiGround)
                    {
                        ki.gameObject.SetActive(false);
                    }
                }
                else if (!isJump)
                {
                    foreach (ParticleSystem ki in kiGround)
                    {
                        ki.gameObject.SetActive(true);
                    }
                }
                // Ki opacity up and down
                ParticleSystem[] kiStartColor = kiFull.GetComponentsInChildren<ParticleSystem>();
                foreach (ParticleSystem kiColor in kiStartColor)
                {
                    Color StartColor = kiColor.startColor;
                    float h, s, v;
                    Color.RGBToHSV(StartColor, out h, out s, out v);
                    if (v > (energy / 100f))
                    {
                        v -= 0.1f * Time.deltaTime;
                        if (v < 0.4)
                        {
                            kiFull.SetActive(false);
                        }
                    }
                    else if (v < (energy / 100f) && v < 0.7f)
                    {
                        v += 0.1f * Time.deltaTime;
                    }
                    Color newColor = Color.HSVToRGB(h, s, v);
                    kiColor.startColor = newColor;
                }
            }
        }
        if (isLayer == isEnemy)
        {
            if (stateInfo.IsName("chargekilast") || stateInfo.IsName("chargekimidle") || stateInfo.IsName("chargeki"))
            {
                ki.SetActive(true);
            }
            if (!stateInfo.IsName("chargekilast") && !stateInfo.IsName("chargekimidle") && !stateInfo.IsName("chargeki"))
            {
                if (!kiFull.activeInHierarchy)
                {
                    audioEnemy.StopAuraLoopSFX("Aura");
                }
                ki.SetActive(false);
                playerAnim.ResetTrigger("chargekilast");
            }
            if (!RestrictAction() && uiManager.isCountdownFinished)
            {
                if (Input.GetKeyDown(KeyCode.P))
                {
                    audioEnemy.PlayVoiceSFX(gameObject.tag + "VoiceKi");
                    audioEnemy.PlayAuraSFX("AuraBegin");
                    audioEnemy.PlayAuraLoopSFX("Aura");
                    playerAnim.SetTrigger("chargeki");
                    playerAnim.SetTrigger("chargekimidle");
                }
                Movement();
                ComboAttacks();
                Skill();
                Rage();
            }
            if (Input.GetKeyUp(KeyCode.P) && uiManager.isCountdownFinished)
            {
                playerAnim.SetTrigger("chargekilast");
                if (kiFull.activeInHierarchy)
                {
                    audioEnemy.PlayAuraLoopSFX("Aura");
                }
            }
            if (ki.activeInHierarchy)
            {
                ApplyIncreaseEnergy(10f);
            }
            if (skillTwo.activeInHierarchy)
            {
                skillTwo.transform.rotation = Quaternion.LookRotation(transform.forward) * Quaternion.Euler(0f, -90f, 0f);
                ColliderKame(skillTwo.transform);
                betweenHands = (RightArmAttackPoint.transform.position + LeftArmAttackPoint.transform.position) / 2;
                CheckChildParticleLifetimesKame(skillTwo.transform);
                skillTwo.transform.position = betweenHands;
            }

            if (kiFull.activeInHierarchy || ki.activeInHierarchy)
            {
                if (GetComponent<Rigidbody>().velocity.y > 0)
                {
                    foreach (ParticleSystem ki in kiGround)
                    {
                        ki.gameObject.SetActive(false);
                    }
                }
                else if (!isJump)
                {
                    foreach (ParticleSystem ki in kiGround)
                    {
                        ki.gameObject.SetActive(true);
                    }
                }
                // Ki opacity up and down
                ParticleSystem[] kiStartColor = kiFull.GetComponentsInChildren<ParticleSystem>();
                foreach (ParticleSystem kiColor in kiStartColor)
                {
                    Color StartColor = kiColor.startColor;
                    float h, s, v;
                    Color.RGBToHSV(StartColor, out h, out s, out v);
                    if (v > (energy / 100f))
                    {
                        v -= 0.1f * Time.deltaTime;
                        if (v < 0.4)
                        {
                            kiFull.SetActive(false);
                        }
                    }
                    else if (v < (energy / 100f) && v < 0.7f)
                    {
                        v += 0.1f * Time.deltaTime;
                    }
                    Color newColor = Color.HSVToRGB(h, s, v);
                    kiColor.startColor = newColor;
                }
            }
        }
    }
    protected void Skill()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && isLayer == isPlayer && uiManager.isCountdownFinished)
        {
            if (isPooling)
            {
                for (int i = 0; i < 5; i++)
                {
                    GameObject fireball = Instantiate(skillOne, transform.position, Quaternion.identity);
                    fireball.layer = LayerMask.NameToLayer(isLayer);
                    SetLayerRecursively(fireball, fireball.layer);
                    fireball.SetActive(false);
                    listSkillOne.Add(fireball);
                }
                isPooling = false;
            }
            ActivateFireball(10f);
        }
        else if (Input.GetKeyDown(KeyCode.J) && isLayer == isEnemy && uiManager.isCountdownFinished)
        {
            if (isPooling)
            {
                for (int i = 0; i < 5; i++)
                {
                    GameObject fireball = Instantiate(skillOne, transform.position, Quaternion.identity);
                    fireball.layer = LayerMask.NameToLayer(isLayer);
                    SetLayerRecursively(fireball, fireball.layer);
                    fireball.SetActive(false);
                    listSkillOne.Add(fireball);
                }
                isPooling = false;
            }
            ActivateFireball(10f);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && isLayer == isPlayer && uiManager.isCountdownFinished)
        {
            if (50f > energy) return;
            ApplyReduceEnergy(50f);
            skillTwo.SetActive(true);
            playerAnim.SetTrigger("kame");
            audioPlayer.PlayVoiceSFX(gameObject.tag + "Kamehame");
            audioPlayer.PlaySkillSFX("KameCore");
        }
        else if (Input.GetKeyDown(KeyCode.K) && isLayer == isEnemy && uiManager.isCountdownFinished)
        {
            if (50f > energy) return;
            ApplyReduceEnergy(50f);
            skillTwo.SetActive(true);
            playerAnim.SetTrigger("kame");
            audioEnemy.PlayVoiceSFX(gameObject.tag + "Kamehame");
            audioEnemy.PlaySkillSFX("KameCore");
        }
    }

    public void CheckChildParticleLifetimesKame(Transform parent)
    {
        foreach (Transform child in parent)
        {
            ParticleSystem particleSystem = child.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                ParticleSystem.MainModule mainModule = particleSystem.main;
                float remainingLifetime = mainModule.duration - particleSystem.time;
                if (remainingLifetime == 0 && child.name == "KameCore")
                {
                    skillTwo.GetComponent<CapsuleCollider>().enabled = false;
                    if (skillThree.GetComponent<CapsuleCollider>() != null)
                        skillThree.GetComponent<CapsuleCollider>().enabled = false;
                    skillTwo.SetActive(false);
                    skillThree.SetActive(false);
                    break;
                }
                //Debug.Log("Remaining Lifetime of Particle System: " + remainingLifetime + " seconds");
            }

            CheckChildParticleLifetimesKame(child);
        }
    }

    public void ColliderKame(Transform parent)
    {
        foreach (Transform child in parent)
        {
            ParticleSystem particleSystem = child.GetComponent<ParticleSystem>();
            if (particleSystem != null && child.name == "Core")
            {
                ParticleSystem.MainModule mainModule = particleSystem.main;
                float remainingLifetime = mainModule.duration - particleSystem.time;
                if (remainingLifetime > 0.3f)
                {
                    RotationEnemy();
                }
                if (remainingLifetime > 0f && remainingLifetime < 0.007f && child.name == "Core")
                {
                    skillTwo.GetComponent<CapsuleCollider>().enabled = true;
                    if (skillThree.GetComponent<CapsuleCollider>() != null)
                        skillThree.GetComponent<CapsuleCollider>().enabled = true;
                    if (audioPlayer != null)
                    {
                        audioPlayer.PlayVoiceSFX(gameObject.tag + "Ha");
                        audioPlayer.PlaySkillSFX("KameHa");
                    }
                    else
                    {
                        audioEnemy.PlayVoiceSFX(gameObject.tag + "Ha");
                        audioEnemy.PlaySkillSFX("KameHa");
                    }
                    //child.gameObject.SetActive(false);
                }
            }
        }
    }
    protected void CheckCameraCoreGohan(Transform parent)
    {
        foreach (Transform child in parent)
        {
            ParticleSystem particleSystem = child.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                ParticleSystem.MainModule mainModule = particleSystem.main;
                float remainingLifetime = mainModule.duration - particleSystem.time;

                if (remainingLifetime <= 1f && child.name == "Core")
                {
                    cameraSkill.enabled = false;
                }
            }
        }
    }
    protected void CheckCameraCoreGoku(Transform parent)
    {
        foreach (Transform child in parent)
        {
            ParticleSystem particleSystem = child.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                ParticleSystem.MainModule mainModule = particleSystem.main;
                float remainingLifetime = mainModule.duration - particleSystem.time;

                if (remainingLifetime <= 1f && child.name == "SpiritBoom")
                {
                    cameraSkill.enabled = false;
                }
            }
        }
    }
    protected void CameraSmooth(Camera camera, Vector3 dir)
    {
        camera.transform.Translate(dir * Time.deltaTime);
        if (!camera.enabled)
        {
            cameraSkill.transform.localPosition = initialCameraPosition;
        }
    }

    public void ParticleLifetimesAndMoveSpiritBoom(Transform parent)
    {
        if ((!uiManager.isCountdownFinished || characterDied) && !isSpiritRuning)
        {
            skillThree.SetActive(false);
        }
        foreach (Transform child in parent)
        {
            if (child.name == "SpiritBoom")
            {

                ParticleSystem particleSystem = child.GetComponent<ParticleSystem>();
                ParticleSystem.MainModule mainModule = particleSystem.main;
                float remainingLifetime = mainModule.duration - particleSystem.time;
                if (remainingLifetime > 0.3f)
                {
                    RotationEnemy();
                }
                if (remainingLifetime == 0)
                {
                    if (!isSpiritRuning)
                    {
                        skillThree.transform.rotation = Quaternion.LookRotation(transform.forward);
                        if (audioPlayer != null)
                        {
                            audioPlayer.PlaySkillSFX("Boom");
                            audioPlayer.PlayVoiceSFX("VoiceBoom");
                        }
                        else
                        {
                            audioEnemy.PlaySkillSFX("Boom");
                            audioEnemy.PlayVoiceSFX("VoiceBoom");
                        }
                    }

                    isSpiritRuning = true;
                    skillThree.transform.rotation = Quaternion.Euler(0f, skillThree.transform.eulerAngles.y, 0f);
                    skillThree.transform.Translate(Vector3.forward * 9f * Time.deltaTime);
                    skillThree.transform.position += Vector3.down * 3f * Time.deltaTime;
                    child.gameObject.SetActive(false);
                    playerAnim.SetTrigger("spiritboomlast");
                    CheckOffscreenSpirit(skillThree);
                    if (!skillThree.activeInHierarchy)
                    {
                        child.gameObject.SetActive(true);
                        playerAnim.ResetTrigger("spiritboomlast");
                    }
                }
                else if (remainingLifetime > 0f)
                {
                    skillThree.transform.position = new Vector3(transform.position.x, transform.position.y + 10f, transform.position.z);
                }
            }
        }
    }

    private void CheckOffscreenSpirit(GameObject skill)
    {
        foreach (Transform spirit in skill.transform)
        {
            if (spirit.gameObject.name != "SpiritBoomPerfectCollider")
            {
                CheckOffscreenSpirit(spirit.gameObject);
            }
            else
            {
                Vector3 screenPos = Camera.main.WorldToScreenPoint(skill.transform.position);
                float screenWidth = Screen.width;
                float expandWidth = screenWidth * 0.4f;
                if (screenPos.x < -expandWidth || screenPos.x > screenWidth + expandWidth)
                {
                    spirit.gameObject.SetActive(true);
                    skillThree.SetActive(false);
                }
            }
        }
    }

    public void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
    private void ActivateFireball(float fireballEnergy)
    {
        if (fireballEnergy > energy) return;
        playerAnim.SetTrigger("fireball");
        ApplyReduceEnergy(fireballEnergy);
        foreach (GameObject fireball in listSkillOne)
        {
            if (!fireball.activeSelf)
            {
                listSkillOneRotation.Add(Quaternion.LookRotation(transform.forward));
                fireball.transform.position = RightArmAttackPoint.transform.position;
                fireball.SetActive(true);
                if (audioPlayer != null)
                {
                    audioPlayer.PlayVoiceSFX(gameObject.tag + "Fireball");
                    audioPlayer.PlaySkillSFX("Fireball");
                }
                else
                {
                    audioEnemy.PlayVoiceSFX(gameObject.tag + "Fireball");
                    audioEnemy.PlaySkillSFX("Fireball");
                }

                break;
            }
        }
    }

    public void MoveFireball()
    {
        if (!isPooling)
        {
            int index = 0;
            foreach (GameObject fireball in listSkillOne)
            {
                if (fireball.activeInHierarchy)
                {
                    listSkillOne[index].transform.rotation = listSkillOneRotation[index];

                    fireball.transform.Translate(Vector3.forward * speedFireball * Time.deltaTime);
                    CheckOffscreenFireball(index);
                    TurnOffFireball();
                }
                index++;
            }
        }
    }




    private void CheckOffscreenFireball(int index)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(listSkillOne[index].transform.position);
        float screenWidth = Screen.width;
        float expandWidth = screenWidth * 0.2f;
        if (screenPos.x < -expandWidth || screenPos.x > screenWidth + expandWidth)
        {
            listSkillOne[index].SetActive(false);

            // Kiểm tra số lượng fireball kích hoạt
            int activeFireballCount = 0;
            for (int i = 0; i < listSkillOne.Count; i++)
            {
                if (listSkillOne[i].activeSelf)
                {
                    activeFireballCount++;
                }
            }

            // Xóa phần tử trong listSkillOneRotation nếu không còn fireball kích hoạt
            if (activeFireballCount == 0)
            {
                for (int i = listSkillOneRotation.Count - 1; i >= 0; i--)
                {
                    listSkillOneRotation.RemoveAt(i);
                }
            }
        }
    }
    protected void TurnOffFireball()
    {
        if (!uiManager.isCountdownFinished)
        {
            foreach (GameObject fireball in listSkillOne)
            {
                fireball.SetActive(false);
            }
        }
    }
    void ComboAttacks()
    {
        if (isLayer == isPlayer)
        {
            if (Input.GetKeyDown(KeyCode.E) && uiManager.isCountdownFinished)
            {

                if (current_combo_state == ComboState.attack3 ||
                    current_combo_state == ComboState.attack4 ||
                    current_combo_state == ComboState.attack5)
                    return;
                current_combo_state++;
                activateTimerToReset = true;
                current_combo_timer = default_combo_timer;

                if (current_combo_state == ComboState.attack1)
                {
                    playerAnim.SetTrigger("attack1");
                    audioPlayer.PlayMoveSFX("Furi");
                }
                if (current_combo_state == ComboState.attack2)
                {
                    playerAnim.SetTrigger("attack2");
                    audioPlayer.PlayMoveSFX("Furi");
                }
                if (current_combo_state == ComboState.attack3)
                {
                    playerAnim.SetTrigger("attack3");
                    audioPlayer.PlayMoveSFX("Furi");
                }
            }
            if (Input.GetKeyDown(KeyCode.Q) && uiManager.isCountdownFinished)
            {
                if (current_combo_state == ComboState.attack5 ||
                    current_combo_state == ComboState.attack3)
                    return;
                if (current_combo_state == ComboState.none ||
                    current_combo_state == ComboState.attack1 ||
                    current_combo_state == ComboState.attack2)
                {
                    current_combo_state = ComboState.attack4;
                }
                else if (current_combo_state == ComboState.attack4)
                {
                    current_combo_state++;
                }
                activateTimerToReset = true;
                current_combo_timer = default_combo_timer;
                if (current_combo_state == ComboState.attack4)
                {
                    audioPlayer.PlayMoveSFX("Furi");
                    playerAnim.SetTrigger("attack4");
                }
                if (current_combo_state == ComboState.attack5)
                {
                    playerAnim.SetTrigger("attack5");
                    audioPlayer.PlayMoveSFX("Furi");
                }
            }
        }
        if (isLayer == isEnemy)
        {
            if (Input.GetKeyDown(KeyCode.O) && uiManager.isCountdownFinished)
            {

                if (current_combo_state == ComboState.attack3 ||
                    current_combo_state == ComboState.attack4 ||
                    current_combo_state == ComboState.attack5)
                    return;
                current_combo_state++;
                activateTimerToReset = true;
                current_combo_timer = default_combo_timer;

                if (current_combo_state == ComboState.attack1)
                {
                    playerAnim.SetTrigger("attack1");
                    audioEnemy.PlayMoveSFX("Furi");
                }
                if (current_combo_state == ComboState.attack2)
                {
                    playerAnim.SetTrigger("attack2");
                    audioEnemy.PlayMoveSFX("Furi");
                }
                if (current_combo_state == ComboState.attack3)
                {
                    playerAnim.SetTrigger("attack3");
                    audioEnemy.PlayMoveSFX("Furi");
                }
            }
            if (Input.GetKeyDown(KeyCode.I) && uiManager.isCountdownFinished)
            {
                if (current_combo_state == ComboState.attack5 ||
                    current_combo_state == ComboState.attack3)
                    return;
                if (current_combo_state == ComboState.none ||
                    current_combo_state == ComboState.attack1 ||
                    current_combo_state == ComboState.attack2)
                {
                    current_combo_state = ComboState.attack4;
                }
                else if (current_combo_state == ComboState.attack4)
                {
                    current_combo_state++;
                }
                activateTimerToReset = true;
                current_combo_timer = default_combo_timer;
                if (current_combo_state == ComboState.attack4)
                {
                    audioEnemy.PlayMoveSFX("Furi");
                    playerAnim.SetTrigger("attack4");
                }
                if (current_combo_state == ComboState.attack5)
                {
                    playerAnim.SetTrigger("attack5");
                    audioEnemy.PlayMoveSFX("Furi");
                }
            }
        }
    }
    protected void ResetComboState()
    {
        if (activateTimerToReset)
        {
            current_combo_timer -= Time.deltaTime;
            if (current_combo_timer <= 0f)
            {
                current_combo_state = ComboState.none;
                activateTimerToReset = false;
                current_combo_timer = default_combo_timer;
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") && isJump)
        {
            playerAnim.SetTrigger("grouding");
            playerAnim.SetTrigger("idle");
            playerAnim.ResetTrigger("jumping");
            playerAnim.ResetTrigger("falling");
            isOnGround = true;
            isJump = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Kame") && other.gameObject.layer != gameObject.layer)
        {
            ApplyDamage(40f, true);
        }
        if (other.gameObject.CompareTag("SuperKame") && other.gameObject.layer != gameObject.layer)
        {
            ApplyDamage(70f, true);
        }
    }

    private void OnParticleCollision(GameObject collision)
    {
        if (collision.gameObject.layer != gameObject.layer && collision.gameObject.CompareTag("FireBall"))
        {
            ApplyDamage(10f, false);
            DeactivateTopmostParent(collision);
            DeactivateRotation(collision);
        }
        if (collision.gameObject.layer != gameObject.layer && collision.gameObject.CompareTag("SpiritBoom"))
        {
            ApplyDamage(70f, true);
            collision.SetActive(false);
        }
    }

    void DeactivateRotation(GameObject collision)
    {
        string layerName = LayerMask.LayerToName(collision.gameObject.layer);
        GameObject scriptHolder = GameObject.Find(layerName); // Tìm GameObject bằng tên Layer

        if (scriptHolder != null)
        {
            var scripts = scriptHolder.GetComponents<MonoBehaviour>();

            foreach (var script in scripts)
            {
                var scriptType = script.GetType();
                var publicVariables = scriptType.GetFields();
                var listSkillOneRotationVariable = publicVariables.FirstOrDefault(variable => variable.Name == "listSkillOneRotation");
                var listSkillOneVariable = publicVariables.FirstOrDefault(variable => variable.Name == "listSkillOne");

                if (listSkillOneRotationVariable != null && listSkillOneVariable != null)
                {
                    var listSkillOneRotationValue = listSkillOneRotationVariable.GetValue(script) as List<Quaternion>;
                    var listSkillOneValue = listSkillOneVariable.GetValue(script) as List<GameObject>;

                    // Kiểm tra số lượng fireball kích hoạt
                    int activeFireballCount = 0;
                    for (int i = 0; i < listSkillOneValue.Count; i++)
                    {
                        if (listSkillOneValue[i].activeSelf)
                        {
                            activeFireballCount++;
                        }
                    }

                    // Xóa phần tử trong listSkillOneRotation nếu không còn fireball kích hoạt
                    if (activeFireballCount == 0)
                    {
                        for (int i = listSkillOneRotationValue.Count - 1; i >= 0; i--)
                        {
                            listSkillOneRotationValue.RemoveAt(i);
                        }
                    }

                }
            }
        }
        else
        {
            Debug.LogWarning("Script holder GameObject not found for layer: " + layerName);
        }
    }



    void DeactivateTopmostParent(GameObject childObject)
    {
        Transform parent = childObject.transform.parent;

        while (parent != null)
        {
            childObject = parent.gameObject;
            parent = childObject.transform.parent;
        }

        childObject.SetActive(false);
    }
    void DetectCollisionRightArm()
    {
        Collider[] hit = Physics.OverlapSphere(RightArmAttackPoint.transform.position, radius, collisionLayer);
        for (int i = 0; i < hit.Length; i++)
        {
            //print("Hit the " + hit[0].gameObject.name);
            //cho player
            if (isLayer == isPlayer)
            {
                Instantiate(hit_FX, RightArmAttackPoint.transform.position, Quaternion.identity);

                if (RightArmAttackPoint.gameObject.tag == "RightArm" || gameObject.tag == "RightLeg")
                {
                    audioPlayer.PlayHitSFX("Knock");
                    hit[0].GetComponent<CharacterController>().ApplyDamage(damage, true);
                }
                else
                {
                    audioPlayer.PlayHitSFX("Punch");
                    hit[0].GetComponent<CharacterController>().ApplyDamage(2f, false);
                }
            }
            //cho enemy
            else if (isLayer == isEnemy)
            {

                Instantiate(hit_FX, RightArmAttackPoint.transform.position, Quaternion.identity);
                if (RightArmAttackPoint.gameObject.tag == "RightArm" || gameObject.tag == "RightLeg")
                {
                    audioEnemy.PlayHitSFX("Knock");
                    hit[0].GetComponent<CharacterController>().ApplyDamage(damage, true);
                }
                else
                {
                    audioEnemy.PlayHitSFX("Punch");
                    hit[0].GetComponent<CharacterController>().ApplyDamage(2f, false);
                }
            }
            RightArmAttackPoint.gameObject.SetActive(false);
        }
    }

    void DetectCollisionLeftArm()
    {
        Collider[] hit = Physics.OverlapSphere(LeftArmAttackPoint.transform.position, radius, collisionLayer);
        for (int i = 0; i < hit.Length; i++)
        {
            //print("Hit the " + hit[0].gameObject.name);
            //cho player
            if (isLayer == isPlayer)
            {
                //Vector3 hitFX_Pos = hit[0].transform.position;
                //hitFX_Pos.y += 1.3f;
                //if (hit[0].transform.forward.x > 0)
                //{
                //    hitFX_Pos.x += 0.3f;
                //}
                //else if (hit[0].transform.forward.x < 0)
                //{
                //    hitFX_Pos.x -= 0.3f;
                //}
                Instantiate(hit_FX, LeftArmAttackPoint.transform.position, Quaternion.identity);
                if (gameObject.tag == "RightArm" || gameObject.tag == "RightLeg")
                {
                    audioPlayer.PlayHitSFX("Knock");
                    hit[0].GetComponent<CharacterController>().ApplyDamage(damage, true);
                }
                else
                {
                    audioPlayer.PlayHitSFX("Punch");
                    hit[0].GetComponent<CharacterController>().ApplyDamage(2f, false);
                }
            }
            //cho enemy
            else if (isLayer == isEnemy)
            {
                //Vector3 hitFX_Pos = hit[0].transform.position;
                //hitFX_Pos.y += 1.3f;
                //if (hit[0].transform.forward.x > 0)
                //{
                //    hitFX_Pos.x += 0.3f;
                //}
                //else if (hit[0].transform.forward.x < 0)
                //{
                //    hitFX_Pos.x -= 0.3f;
                //}
                Instantiate(hit_FX, LeftArmAttackPoint.transform.position, Quaternion.identity);
                if (gameObject.tag == "RightArm" || gameObject.tag == "RightLeg")
                {
                    audioEnemy.PlayHitSFX("Knock");
                    hit[0].GetComponent<CharacterController>().ApplyDamage(damage, true);
                }
                else
                {
                    audioEnemy.PlayHitSFX("Punch");
                    hit[0].GetComponent<CharacterController>().ApplyDamage(2f, false);
                }
            }
            LeftArmAttackPoint.gameObject.SetActive(false);
        }
    }

    void DetectCollisionRightLeg()
    {
        Collider[] hit = Physics.OverlapSphere(RightLegAttackPoint.transform.position, radius, collisionLayer);
        for (int i = 0; i < hit.Length; i++)
        {
            //print("Hit the " + hit[0].gameObject.name);
            //cho player
            if (isLayer == isPlayer)
            {
                //Vector3 hitFX_Pos = hit[0].transform.position;
                //hitFX_Pos.y += 1.3f;
                //if (hit[0].transform.forward.x > 0)
                //{
                //    hitFX_Pos.x += 0.3f;
                //}
                //else if (hit[0].transform.forward.x < 0)
                //{
                //    hitFX_Pos.x -= 0.3f;
                //}
                Instantiate(hit_FX, RightLegAttackPoint.transform.position, Quaternion.identity);
                if (gameObject.tag == "RightArm" || RightLegAttackPoint.gameObject.tag == "RightLeg")
                {
                    audioPlayer.PlayHitSFX("Knock");
                    hit[0].GetComponent<CharacterController>().ApplyDamage(damage, true);
                }
                else
                {
                    audioPlayer.PlayHitSFX("Punch");
                    hit[0].GetComponent<CharacterController>().ApplyDamage(3f, false);
                }
            }
            //cho enemy
            else if (isLayer == isEnemy)
            {
                //Vector3 hitFX_Pos = hit[0].transform.position;
                //hitFX_Pos.y += 1.3f;
                //if (hit[0].transform.forward.x > 0)
                //{
                //    hitFX_Pos.x += 0.3f;
                //}
                //else if (hit[0].transform.forward.x < 0)
                //{
                //    hitFX_Pos.x -= 0.3f;
                //}
                Instantiate(hit_FX, RightLegAttackPoint.transform.position, Quaternion.identity);
                if (gameObject.tag == "RightArm" || RightLegAttackPoint.gameObject.tag == "RightLeg")
                {
                    audioEnemy.PlayHitSFX("Knock");
                    hit[0].GetComponent<CharacterController>().ApplyDamage(damage, true);
                }
                else
                {
                    audioEnemy.PlayHitSFX("Punch");
                    hit[0].GetComponent<CharacterController>().ApplyDamage(3f, false);
                }
            }
            RightLegAttackPoint.gameObject.SetActive(false);
        }
    }

    void DetectCollisionLeftLeg()
    {
        Collider[] hit = Physics.OverlapSphere(LeftLegAttackPoint.transform.position, radius, collisionLayer);
        for (int i = 0; i < hit.Length; i++)
        {
            //print("Hit the " + hit[0].gameObject.name);
            //cho player
            if (isLayer == isPlayer)
            {
                //Vector3 hitFX_Pos = hit[0].transform.position;
                //hitFX_Pos.y += 1.3f;
                //if (hit[0].transform.forward.x > 0)
                //{
                //    hitFX_Pos.x += 0.3f;
                //}
                //else if (hit[0].transform.forward.x < 0)
                //{
                //    hitFX_Pos.x -= 0.3f;
                //}
                Instantiate(hit_FX, LeftLegAttackPoint.transform.position, Quaternion.identity);
                if (gameObject.tag == "RightArm" || gameObject.tag == "RightLeg" || LeftLegAttackPoint.gameObject.tag == "LeftLeg")
                {
                    audioPlayer.PlayHitSFX("Knock");
                    hit[0].GetComponent<CharacterController>().ApplyDamage(damage, true);
                }
                else
                {
                    audioPlayer.PlayHitSFX("Punch");
                    hit[0].GetComponent<CharacterController>().ApplyDamage(3f, false);
                }
            }
            //cho enemy
            else if (isLayer == isEnemy)
            {
                //Vector3 hitFX_Pos = hit[0].transform.position;
                //hitFX_Pos.y += 1.3f;
                //if (hit[0].transform.forward.x > 0)
                //{
                //    hitFX_Pos.x += 0.3f;
                //}
                //else if (hit[0].transform.forward.x < 0)
                //{
                //    hitFX_Pos.x -= 0.3f;
                //}
                Instantiate(hit_FX, LeftLegAttackPoint.transform.position, Quaternion.identity);
                if (gameObject.tag == "RightArm" || gameObject.tag == "RightLeg" || LeftLegAttackPoint.gameObject.tag == "LeftLeg")
                {
                    audioEnemy.PlayHitSFX("Knock");
                    hit[0].GetComponent<CharacterController>().ApplyDamage(damage, true);
                }
                else
                {
                    audioEnemy.PlayHitSFX("Punch");
                    hit[0].GetComponent<CharacterController>().ApplyDamage(3f, false);
                }
            }
            LeftLegAttackPoint.gameObject.SetActive(false);
        }
    }

    void DetectCollisionRightForeArm()
    {
        Collider[] hit = Physics.OverlapSphere(RightForeArmAttackPoint.transform.position, radius, collisionLayer);
        for (int i = 0; i < hit.Length; i++)
        {
            //print("Hit the " + hit[0].gameObject.name);
            //cho player
            if (isLayer == isPlayer)
            {
                //Vector3 hitFX_Pos = hit[0].transform.position;
                //hitFX_Pos.y += 1.3f;
                //if (hit[0].transform.forward.x > 0)
                //{
                //    hitFX_Pos.x += 0.3f;
                //}
                //else if (hit[0].transform.forward.x < 0)
                //{
                //    hitFX_Pos.x -= 0.3f;
                //}
                Instantiate(hit_FX, RightForeArmAttackPoint.transform.position, Quaternion.identity);
                if (gameObject.tag == "RightArm" || gameObject.tag == "RightLeg")
                {
                    audioPlayer.PlayHitSFX("Knock");
                    hit[0].GetComponent<CharacterController>().ApplyDamage(damage, true);
                }
                else
                {
                    audioPlayer.PlayHitSFX("Punch");
                    hit[0].GetComponent<CharacterController>().ApplyDamage(2.5f, false);
                }
            }
            //cho enemy
            else if (isLayer == isEnemy)
            {
                //Vector3 hitFX_Pos = hit[0].transform.position;
                //hitFX_Pos.y += 1.3f;
                //if (hit[0].transform.forward.x > 0)
                //{
                //    hitFX_Pos.x += 0.3f;
                //}
                //else if (hit[0].transform.forward.x < 0)
                //{
                //    hitFX_Pos.x -= 0.3f;
                //}
                Instantiate(hit_FX, RightForeArmAttackPoint.transform.position, Quaternion.identity);
                if (gameObject.tag == "RightArm" || gameObject.tag == "RightLeg")
                {
                    audioEnemy.PlayHitSFX("Knock");
                    hit[0].GetComponent<CharacterController>().ApplyDamage(damage, true);
                }
                else
                {
                    audioEnemy.PlayHitSFX("Punch");
                    hit[0].GetComponent<CharacterController>().ApplyDamage(2.5f, false);
                }
            }
            RightForeArmAttackPoint.gameObject.SetActive(false);
        }
    }
    void RightArmAttackOn()
    {
        RightArmAttackPoint.SetActive(true);
    }
    void RightArmAttackOff()
    {
        if (RightArmAttackPoint.activeInHierarchy)
        {
            RightArmAttackPoint.SetActive(false);
        }
    }
    void LeftTArmAttackOn()
    {
        LeftArmAttackPoint.SetActive(true);
    }
    void LeftArmAttackOff()
    {
        if (LeftArmAttackPoint.activeInHierarchy)
        {
            LeftArmAttackPoint.SetActive(false);
        }
    }
    void LeftTLegAttackOn()
    {
        LeftLegAttackPoint.SetActive(true);
    }
    void LeftLegAttackOff()
    {
        if (LeftLegAttackPoint.activeInHierarchy)
        {
            LeftLegAttackPoint.SetActive(false);
        }
    }
    void RightLegAttackOn()
    {
        RightLegAttackPoint.SetActive(true);
    }
    void RightLegAttackOff()
    {
        if (RightLegAttackPoint.activeInHierarchy)
        {
            RightLegAttackPoint.SetActive(false);
        }
    }
    void RightForeArmAttackOn()
    {
        RightForeArmAttackPoint.SetActive(true);
    }
    void RightForeArmAttackOff()
    {
        if (RightForeArmAttackPoint.activeInHierarchy)
        {
            RightForeArmAttackPoint.SetActive(false);
        }
    }
    void TagRightArm()
    {
        RightArmAttackPoint.tag = "RightArm";
    }
    void UnTagRightArm()
    {
        RightArmAttackPoint.tag = "Untagged";
    }
    void TagRightLeg()
    {
        RightLegAttackPoint.tag = "RightLeg";
    }
    void UnTagRightLeg()
    {
        RightLegAttackPoint.tag = "Untagged";
    }
    void TagLeftLeg()
    {
        LeftLegAttackPoint.tag = "LeftLeg";
    }
    void UnTagLeftLeg()
    {
        LeftLegAttackPoint.tag = "Untagged";
    }
    protected void BO3()
    {
        if (uiManager.enemyWins == 2 && uiManager.playerWins == 2)
        {
            StartCoroutine(ShowWinMessageCoroutine("DRAW!"));
        }
        else if (isLayer == isPlayer && uiManager.playerWins == 2)
        {
            StartCoroutine(ShowWinMessageCoroutine("P1 WIN!"));
        }
        else if (isLayer == isEnemy && uiManager.enemyWins == 2)
        {
            StartCoroutine(ShowWinMessageCoroutine("P2 WIN!"));
        }

        if (uiManager.enemyWins == 1 && isLayer == isPlayer)
        {
            if (health <= 0)
            {
                uiManager.RestartCountdown();
                uiManager.DisplayBO3();
            }

            if (!uiManager.isCountdownFinished && !healing)
            {
                IncreaseHealth();
            }
        }
        if (uiManager.playerWins == 1 && isLayer == isEnemy)
        {
            if (health <= 0)
            {
                uiManager.RestartCountdown();
                uiManager.DisplayBO3();
            }

            if (!uiManager.isCountdownFinished && !healing)
            {
                IncreaseHealth();
            }
        }
    }


    private IEnumerator ShowWinMessageCoroutine(string message)
    {
        yield return new WaitForSeconds(5f); // 
        uiManager.DisplayWinMessage(message);
    }
    public void ApplyDamage(float damage, bool knockDown)
    {
        if (characterDied || uiManager.playerWins == 2 || uiManager.enemyWins == 2 || uiManager.playerWins == 2 && uiManager.enemyWins == 2)
            return;
        if (!isBlock)
        {
            health -= damage;
        }

        if (isLayer == isPlayer)
        {
            if (isBlock)
            {
                audioPlayer.PlayMoveSFX("Shield");
                shield.SetActive(true);
                shield.transform.position = RightArmAttackPoint.transform.position;
                shield.transform.GetComponentInChildren<ParticleSystem>().Play();
            }
            uiManager.DisplayHealth(health, true);

            if (health <= 0f)
            {
                audioPlayer.PlayVoiceSFX(gameObject.tag + "Died");
                playerAnim.ResetTrigger("standup");
                playerAnim.SetTrigger("died");
                characterDied = true;
                ++uiManager.enemyWins;
                uiManager.DisplayBO3();
                if (uiManager.enemyWins == 1)
                {
                    playerAnim.SetTrigger("standup");
                    playerAnim.ResetTrigger("standup");
                    Debug.Log("1");
                    characterDied = false;
                    knockDown = false;
                }
            }
            if (skillTwo.activeInHierarchy || skillThree.activeInHierarchy || characterDied) return;

            if (knockDown && !characterDied && !isBlock)
            {
                if (Random.Range(0, 2) > 0)
                {
                    Vector3 knockbackDirection = (-transform.forward + transform.up).normalized;
                    float knockbackForce = 20f;

                    playerRb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
                    audioPlayer.PlayVoiceSFX(gameObject.tag + "Knock");
                    playerAnim.SetTrigger("knockdown");
                    playerAnim.SetTrigger("standup");
                }
            }
            else if (!characterDied && !isBlock)
            {

                audioPlayer.PlayVoiceSFX(gameObject.tag + "Hit");
                playerAnim.SetTrigger("hit");

            }
            if (knockDown && isKame && !characterDied)
            {
                audioPlayer.PlayVoiceSFX(gameObject.tag + "Knock");
                isKame = false;
                playerAnim.SetTrigger("knockdown");
                playerAnim.SetTrigger("standup");
                playerAnim.ResetTrigger("standup");
                Debug.Log("3");
            }
        }
        else if (isLayer == isEnemy)
        {
            if (isBlock)
            {
                audioEnemy.PlayMoveSFX("Shield");
                shield.SetActive(true);
                shield.transform.position = RightArmAttackPoint.transform.position;
                shield.transform.GetComponentInChildren<ParticleSystem>().Play();
            }
            uiManager.DisplayHealth(health, false);

            if (health <= 0f)
            {
                playerAnim.ResetTrigger("standup");
                playerAnim.SetTrigger("died");
                characterDied = true;
                ++uiManager.playerWins;
                uiManager.DisplayBO3();
                if (uiManager.playerWins == 1)
                {
                    playerAnim.SetTrigger("standup");
                    playerAnim.ResetTrigger("standup");
                    Debug.Log("4");
                    characterDied = false;
                    knockDown = false;
                }
            }
            if (skillTwo.activeInHierarchy || skillThree.activeInHierarchy || characterDied) return;

            if (knockDown && !characterDied && !isBlock)
            {
                if (Random.Range(0, 2) > 0)
                {
                    Vector3 knockbackDirection = (-transform.forward + transform.up).normalized;
                    float knockbackForce = 20f;

                    playerRb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
                    audioEnemy.PlayVoiceSFX(gameObject.tag + "Knock");
                    playerAnim.SetTrigger("knockdown");
                    playerAnim.SetTrigger("standup");
                }
            }
            else if (!characterDied && !isBlock)
            {

                audioEnemy.PlayVoiceSFX(gameObject.tag + "Hit");
                playerAnim.SetTrigger("hit");

            }
            if (knockDown && isKame && !characterDied)
            {
                audioEnemy.PlayVoiceSFX(gameObject.tag + "Knock");
                isKame = false;
                playerAnim.SetTrigger("knockdown");
                playerAnim.SetTrigger("standup");
                playerAnim.ResetTrigger("standup");
                Debug.Log("6");
            }
        }
        if (uiManager.playerWins >= 2)
        {
            Debug.Log("Player thang roiiiiiiiiiiii");
            playerAnim.SetTrigger("died");
        }
        if (uiManager.enemyWins >= 2)
        {
            Debug.Log("Enemy thang roiiiiiiiiiiii");
            playerAnim.SetTrigger("died");
        }

        if (stateInfo.IsName("died"))
        {
            skillTwo.SetActive(false);
        }
    }
    protected void IncreaseHealth()
    {
        if (health <= initialHealth)
        {
            health += (initialHealth / 2f) * Time.deltaTime;
            if (health >= initialHealth)
            {
                health = initialHealth;
                if (health == initialHealth)
                {
                    healing = true;
                }
            }
            if (isLayer == isPlayer)
            {
                uiManager.DisplayHealth(health, true);
            }
            else if (isLayer == isEnemy)
            {
                uiManager.DisplayHealth(health, false);
            }
        }

    }
    public void ApplyIncreaseEnergy(float speed)
    {
        energy += speed * Time.deltaTime;

        if (isLayer == isPlayer)
        {
            uiManager.DisplayEnergy(energy, true);
        }
        else if (isLayer == isEnemy)
        {
            uiManager.DisplayEnergy(energy, false);
        }

        if (energy > 100f)
        {
            energy = 100f;
            kiFull.SetActive(true);

        }
    }
    public void ApplyReduceEnergy(float charge)
    {
        energy -= charge;
        if (isLayer == isPlayer)
        {
            uiManager.DisplayEnergy(energy, true);
        }
        else if (isLayer == isEnemy)
        {
            uiManager.DisplayEnergy(energy, false);
        }
    }
    public void IncreaseRage()
    {
        if (rage < targetRage)
        {
            timeElapsed += Time.deltaTime;

            if (timeElapsed >= 1f / rageIncreaseSpeed)
            {
                rage += 1f;
                timeElapsed = 0f;
                if (isLayer == isPlayer)
                {
                    uiManager.DisplayRage(rage, true);
                }
                else if (isLayer == isEnemy)
                {
                    uiManager.DisplayRage(rage, false);
                }
            }
        }
    }
    protected void Rage()
    {
        if (isLayer == isPlayer)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                UseRage();
            }

            if (isUsingRage)
            {
                DecreaseRage();
            }
        }
        if (isLayer == isEnemy)
        {
            if (Input.GetKeyDown(KeyCode.U))
            {
                UseRage();
            }

            if (isUsingRage)
            {
                DecreaseRage();
            }
        }
    }
    void UseRage()
    {
        if (!isUsingRage && rage >= 10f)
        {
            isUsingRage = true;
            initialDamage = damage;
        }
    }

    void DecreaseRage()
    {
        if (rage > 0f)
        {
            timeElapsed += Time.deltaTime;

            if (timeElapsed >= 1f / 5f)
            {
                rage -= 1f;
                timeElapsed = 0f;
                if (isUsingRage)
                {
                    damage += 0.1f;
                    health += 0.1f;
                    if (health < initialHealth)
                    {
                        health += 1f;
                    }
                    else
                    {
                        health = initialHealth;
                    }
                    if (isLayer == isPlayer)
                    {
                        uiManager.DisplayHealth(health, true);
                    }
                    else if (isLayer == isEnemy)
                    {
                        uiManager.DisplayHealth(health, false);
                    }
                }

                if (isLayer == isPlayer)
                {
                    uiManager.DisplayRage(rage, true);
                }
                else if (isLayer == isEnemy)
                {
                    uiManager.DisplayRage(rage, false);
                }
            }
        }
        else
        {
            isUsingRage = false;
            damage = initialDamage;
        }
    }
    void PerformInstantMoveSkill()
    {
        if (targetEnemy != null)
        {
            Vector3 targetDirection = targetEnemy.position - transform.position;
            Vector3 newPosition = targetEnemy.position + targetDirection.normalized * 2;
            transform.position = newPosition;
        }
    }
    protected void AttackPoint()
    {
        if (RightArmAttackPoint.activeInHierarchy)
        {
            DetectCollisionRightArm();
        }
        if (LeftArmAttackPoint.activeInHierarchy)
        {
            DetectCollisionLeftArm();
        }
        if (RightForeArmAttackPoint.activeInHierarchy)
        {
            DetectCollisionRightForeArm();
        }
        if (LeftLegAttackPoint.activeInHierarchy)
        {
            DetectCollisionLeftLeg();
        }
        if (RightLegAttackPoint.activeInHierarchy)
        {
            DetectCollisionRightLeg();
        }
    }
}
