using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public Image player_health_UI;
    public Image player_energy_UI;
    public Image player_rage_UI;
    public Image enemy_health_UI;
    public Image enemy_energy_UI;
    public Image enemy_rage_UI;
    public Gradient gradient;
    private float player_energy = 15f;
    private float enemy_energy = 15f;
    private float health = 0;
    private float health_enemy = 0;
    private float player_rage = 0f;
    private float enemy_rage = 0f;
    public int playerWins = 0;
    public int enemyWins = 0;
    public int round = 1;
    public GameObject P1W1;
    public GameObject P1W2;
    public GameObject P2W1;
    public GameObject P2W2;
    public TMP_Text countdownText;
    public TMP_Text timeText;
    public TMP_Text roundText;
    public bool isCountdownFinished = false;
    public TMP_Text winMessageText;
    public GameObject winPanel;
    public bool GameIsPause = false;
    public GameObject pausePanel;
    public GameObject optionPanel;
    public AudioManager audioManager;
    public float time = 300f;
    public bool timepause;
    public bool startcountdown;
    public int temp;
    private void Start()
    {
        timepause = true;
        startcountdown = false;
        temp = 0;
        audioManager = GameObject.Find("AudioPlayer").GetComponent<AudioManager>();
        StartCoroutine(CountdownCoroutine());
    }
    void Awake()
    {
        player_health_UI = GameObject.FindWithTag("HealthUI").GetComponent<Image>();
        enemy_health_UI = GameObject.FindWithTag("EnemyHealthUI").GetComponent<Image>();
        player_energy_UI = GameObject.FindWithTag("EnergyUI").GetComponent<Image>();
        enemy_energy_UI = GameObject.FindWithTag("EnemyEnergyUI").GetComponent<Image>();
        player_rage_UI = GameObject.FindWithTag("RageUI").GetComponent<Image>();
        enemy_rage_UI = GameObject.FindWithTag("EnemyRageUI").GetComponent<Image>();
        DisplayEnergy(player_energy, true);
        DisplayEnergy(enemy_energy, false);
        DisplayEnergy(player_rage, true);
        DisplayEnergy(enemy_rage, false);
    }
    void Update()
    {
        if(startcountdown && temp==0)
        {
            StartCoroutine(Timer());
        }
        if (round == 1)
        {
            roundText.text = $"R{round}";
        }
        if (round == 2)
        {
            roundText.text = $"R{round}";
        }
        if (round == 3)
        {
            roundText.text = $"R{round}";
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPause)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
        if (time == 0f)
        {
            if (player_health_UI != null && enemy_health_UI != null)
            {
                if (player_health_UI.fillAmount > enemy_health_UI.fillAmount)
                {
                    if (playerWins == 0 && round == 1 || playerWins == 0 && round == 2)
                    {
                        playerWins = 1;
                        RestartCountdown();
                        DisplayBO3();
                    }
                    else if (playerWins == 1 && round == 2 || playerWins == 1 && round == 3)
                    {
                        playerWins = 2;
                        DisplayBO3();
                    }
                }
                if (player_health_UI.fillAmount < enemy_health_UI.fillAmount)
                {
                    if (enemyWins == 0 && round == 1 || enemyWins == 0 && round == 2)
                    {
                        enemyWins = 1;
                        RestartCountdown();
                        DisplayBO3();
                    }
                    else if (enemyWins == 1 && round == 2 || enemyWins == 1 && round == 3)
                    {
                        enemyWins = 2;
                        DisplayBO3();
                    }
                }
                if (player_health_UI.fillAmount == enemy_health_UI.fillAmount)
                {
                    if (enemyWins == 0 && playerWins == 0 && round == 1)
                    {
                        playerWins = 1;
                        enemyWins = 1;
                        RestartCountdown();
                        DisplayBO3();
                    }
                    else if (enemyWins == 0 && playerWins == 1 && round == 2)
                    {
                        enemyWins = 1;
                        playerWins = 2;
                        DisplayBO3();
                    }
                    else if (enemyWins == 1 && playerWins == 0 && round == 2)
                    {
                        enemyWins = 2;
                        playerWins = 1;
                        DisplayBO3();
                    }
                    else if (playerWins == 1 && enemyWins == 1 && round == 3)
                    {
                        enemyWins = 2;
                        playerWins = 2;
                        DisplayBO3();
                    }
                }
            }
        }
    }
    public void DisplayBO3()
    {

        if (playerWins == 1)
        {
            P1W1.SetActive(true);
        }
        else if (playerWins == 2)
        {
            P1W2.SetActive(true);
        }
        if (enemyWins == 1)
        {
            P2W1.SetActive(true);
        }
        else if (enemyWins == 2)
        {
            P2W2.SetActive(true);
        }
    }
    public void DisplayHealth(float value, bool isPlayer)
    {
        if (isPlayer)
        {
            if (health == 0)
                health = value;

            value /= health;
        }
        else
        {
            if (health_enemy == 0)
                health_enemy = value;

            value /= health_enemy;
        }
        if (value < 0f)
            value = 0f;

        if (isPlayer && player_health_UI != null)
        {
            player_health_UI.fillAmount = value;
        }
        else if (!isPlayer && enemy_health_UI != null)
        {
            enemy_health_UI.fillAmount = value;
        }
    }
    public void DisplayEnergy(float value, bool isPlayer)
    {
        value /= 100f;
        if (value < 0f)
            value = 0f;

        if (isPlayer && player_energy_UI != null)
        {
            player_energy_UI.fillAmount = value;
            player_energy_UI.color = gradient.Evaluate(value);
        }
        else if (!isPlayer && enemy_energy_UI != null)
        {
            enemy_energy_UI.fillAmount = value;
            enemy_energy_UI.color = gradient.Evaluate(value);
        }
    }
    public void DisplayRage(float value, bool isPlayer)
    {
        value /= 100f;
        if (value < 0f)
            value = 0f;

        if (isPlayer && player_rage_UI != null)
        {
            player_rage_UI.fillAmount = value;
            //player_rage_UI.color = gradient.Evaluate(value);
        }
        else if (!isPlayer && enemy_rage_UI != null)
        {
            enemy_rage_UI.fillAmount = value;
            //enemy_rage_UI.color = gradient.Evaluate(value);
        }
    }
    public IEnumerator CountdownCoroutine()
    {
        isCountdownFinished = false;
        countdownText.gameObject.SetActive(true);

        if (playerWins == 1 || enemyWins == 1)
        {
            time = 305;
            timeText.text = "";
            round = 2;
            audioManager.PlaySFX("Round2");
            countdownText.text = $"Round {round}";
        }

        if (playerWins == 1 && enemyWins == 1)
        {
            time = 305;
            timeText.text = "";
            round = 3;
            audioManager.PlaySFX("FinalRound");
            countdownText.text = $"Final Round";
        }

        if (enemyWins == 1 || playerWins == 1)
        {
            yield return new WaitForSeconds(2f);
        }
        int countdown = 3;
        while (countdown > 0)
        {
            if (countdown == 3)
                audioManager.PlaySFX("Three");
            else if (countdown == 2)
                audioManager.PlaySFX("Two");
            else if (countdown == 1)
                audioManager.PlaySFX("One");

            countdownText.text = countdown.ToString();
            yield return new WaitForSeconds(1f);
            countdown--;
        }

        audioManager.PlaySFX("Fight");
        countdownText.text = "Fight!";

        yield return new WaitForSeconds(1f);

        countdownText.gameObject.SetActive(false);
        isCountdownFinished = true;
        startcountdown = true;
    }
    public IEnumerator Timer()
    {
        temp = 1;
        while (time > 0f && timepause)
        {
            if (round == 1)
            {
                time--;
                timeText.text = time.ToString();
                yield return new WaitForSecondsRealtime(1f);
            }
            if (round == 2  )
            {
                time--;
                timeText.text = time.ToString();
                yield return new WaitForSecondsRealtime(1f);
            }
            if (round == 3  )
            {
                time--;
                timeText.text = time.ToString();
                yield return new WaitForSecondsRealtime(1f);
            }
        }
    }
    public void RestartCountdown()
    {
        if (isCountdownFinished)
        {
            StopCoroutine(CountdownCoroutine());
            StartCountdown();
        }
    }
    public void StartCountdown()
    {
        if (isCountdownFinished)
        {
            StartCoroutine(CountdownCoroutine());
        }
    }
    public void DisplayWinMessage(string message)
    {
        winMessageText.text = message;
        winPanel.SetActive(true);
    }
    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
        playerWins = 0;
        enemyWins = 0;
    }
    public void Resume()
    {
        timepause = true;
        temp = 0;
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        GameIsPause = false;
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audioSource in audioSources)
        {
            audioSource.UnPause();
        }
    }
    void Pause()
    {
        timepause = false;
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        GameIsPause = true;
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audioSource in audioSources)
        {
            audioSource.Pause();
        }
    }
    public void Quit()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
    public void Restart()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
    public void Option()
    {
        optionPanel = GameObject.Find("Canvas");
        optionPanel = optionPanel.transform.Find("OptionPanel").gameObject;
        optionPanel.SetActive(true);
    }
}

