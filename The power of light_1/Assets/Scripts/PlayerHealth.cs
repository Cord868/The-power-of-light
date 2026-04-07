using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;

    public HealthUI healthUI;

    private SpriteRenderer spriteRenderer;

    public static event Action OnPlayedDied;

    void Start()
    {
        ResetHealth();

        spriteRenderer = GetComponent<SpriteRenderer>();
        GameController.OnReset += ResetHealth;
        HealthItem.OnHealthCollect += Heal;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy)
        {
            TakeDamage(enemy.damage);
            SoundEffectManager.Play("PlayerHit");
        }
        Trap trap = collision.GetComponent<Trap>();
        if (trap && trap.damage > 0)
        {
            TakeDamage(trap.damage);
        }
        else if (trap)
        {
            SoundEffectManager.Play("Bounce");
        }
    }

    void Heal(int amout)
    {
        currentHealth += amout;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        healthUI.UpdateHearts(currentHealth);
    }

    void ResetHealth()
    {
        currentHealth = maxHealth;
        healthUI.SetMaxHearts(maxHealth);
    }

    private void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthUI.UpdateHearts(currentHealth);

        //Мигание красным цветом, при получении урона 
        StartCoroutine(FlashRed());

        if (currentHealth <= 0)
        {
            //Игрок умерает -- появляется экран "Игра окончена" со список пройденых уровней и с кнопкой "ПОВТОРИТЬ"
            OnPlayedDied.Invoke();
        }
    }

    private IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = Color.white;
    }
}
