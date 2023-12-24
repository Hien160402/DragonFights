using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthScript : MonoBehaviour
{
    public Animator playerAnim;
    public float health = 100f;
    private bool characterDied;
    public bool is_Player;
    private UIManager health_UI;
    private UIManager enemyHealthUI;

    void Awake()
    {
        if (is_Player)
        {
            health_UI = GetComponent<UIManager>();
        }
        else
        {
            enemyHealthUI = GetComponent<UIManager>();
        }
    }
    public void ApplyDamage(float damage,bool knockDown)
    {
        if (characterDied)
            return;

        health -= damage;

        if (is_Player)
        {
            health_UI.DisplayHealth(health,true);
        }
        else
        {
            enemyHealthUI.DisplayHealth(health,false);
        }

        if (health <= 0f)
        {
            playerAnim.SetTrigger("died");
            characterDied = true;
            if (is_Player)
            {
            }
            return;
        }
        if (!is_Player)
        {
            if (knockDown)
            {
                if (Random.Range(0, 2) > 0)
                {
                    playerAnim.SetTrigger("died");
                }
            }
            else
            {
                if (Random.Range(0, 3) >= 1)
                {
                    playerAnim.SetTrigger("hit");
                }
            }
        }
    }
}
