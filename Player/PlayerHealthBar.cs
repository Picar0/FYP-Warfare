using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    public static PlayerHealthBar instance;
    [SerializeField] private Image healthBar;
    private float maxHealth;
    private float currentHealth;
    private float lerpSpeed = 3f;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        maxHealth = PHealth.instance.maxHealth;
        currentHealth = PHealth.instance.currentHealth;
    }

    private void Update()
    {
        HealthBarFiller();
        ColorChanger();
    }

    public void HealthBarFiller()
    {
        currentHealth = PHealth.instance.currentHealth;
        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, currentHealth / maxHealth, lerpSpeed * Time.deltaTime);
    }

    private void ColorChanger()
    {
        Color healthColor = Color.Lerp(Color.red, Color.green, currentHealth / maxHealth);
        healthBar.color = healthColor;
    }
}