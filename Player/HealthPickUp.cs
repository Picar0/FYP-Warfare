using UnityEngine;
using System.Collections;
using TMPro;

public class HealthPickUp : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private float textDisplayDuration = 3f;
    [SerializeField] private ParticleSystem healEffect;
    [SerializeField] private GameObject healObject;
    private Coroutine textDisplayCoroutine;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            if (PHealth.instance.currentHealth == PHealth.instance.maxHealth)
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
                PHealth.instance.currentHealth = PHealth.instance.maxHealth;
                PlayerHealthBar.instance.HealthBarFiller();
                healEffect.Play();
                AudioManager.instance.PlayPlayerSFX("Heal");
                Destroy(healObject);
            }
        }
    }

    private IEnumerator DisableHealthTextAfterDuration()
    {
        yield return new WaitForSeconds(textDisplayDuration);

        healthText.gameObject.SetActive(false);
    }
}
