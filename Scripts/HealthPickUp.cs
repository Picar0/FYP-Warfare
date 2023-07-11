using UnityEngine;
using System.Collections;
using TMPro;

public class HealthPickUp : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public PlayerHealthBar healthBar;
    public TextMeshProUGUI healthText;
    public float textDisplayDuration = 3f;
    private Coroutine textDisplayCoroutine;

    private void OnTriggerEnter(Collider other)
    {
        if(playerHealth.currentHealth == playerHealth.maxHealth)
        {

            if (textDisplayCoroutine != null)
            {
                StopCoroutine(textDisplayCoroutine);
            }

            healthText.gameObject.SetActive(true);
            textDisplayCoroutine = StartCoroutine(DisableHealthTextAfterDuration());
        }
        else
        {
            playerHealth.currentHealth = playerHealth.maxHealth;
            healthBar.HealthBarFiller();
            Destroy(gameObject);
        }

        
    }
    private IEnumerator DisableHealthTextAfterDuration()
    {
        yield return new WaitForSeconds(textDisplayDuration);

        healthText.gameObject.SetActive(false);
    }
}
