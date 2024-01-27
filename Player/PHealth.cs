using UnityEngine;


public class PHealth : MonoBehaviour
{
    public static PHealth instance;
    public int maxHealth = 100;
    public int currentHealth;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
    }
}
