using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    public Image healthBar;
    public PlayerHealth playerHealth;

    private float maxHealth;
    private float lerpSpeed = 3f;
    private float currentHealth;

    private void Start()
    {
        maxHealth = playerHealth.maxHealth;
        currentHealth = playerHealth.GetCurrentHealth();
    }

    private void Update()
    {
        HealthBarFiller();
        ColorChanger();
    }

    public void HealthBarFiller()
    {
        currentHealth = playerHealth.GetCurrentHealth();
        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, currentHealth / maxHealth, lerpSpeed * Time.deltaTime);
    }

    private void ColorChanger()
    {
        Color healthColor = Color.Lerp(Color.red, Color.green, currentHealth / maxHealth);
        healthBar.color = healthColor;
    }
}
