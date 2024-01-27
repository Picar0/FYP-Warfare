using UnityEngine;
using System.Collections;
using TMPro;
public class AmmoPickUp : MonoBehaviour
{
    [SerializeField] private int ammoRestored = 500;
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private float textDisplayDuration = 3f;
    [SerializeField] private ParticleSystem ammoEffect;
    [SerializeField] private GameObject ammoContainer;
    private Coroutine textDisplayCoroutine;
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            if (Weapon.instance.totalAmmoCount == ammoRestored)
            {
               if (textDisplayCoroutine != null)
               {
                   StopCoroutine(textDisplayCoroutine);
               }

               ammoText.gameObject.SetActive(true);
               textDisplayCoroutine = StartCoroutine(DisableHealthTextAfterDuration());
            }
            else
            {
               Weapon.instance.totalAmmoCount = ammoRestored;
               Weapon.instance.UpdateAmmoCountText();
                ammoEffect.Play();
                AudioManager.instance.PlayPlayerSFX("Ammo Pickup");
                Destroy(ammoContainer);
            }
        }
    }

    private IEnumerator DisableHealthTextAfterDuration()
    {
        yield return new WaitForSeconds(textDisplayDuration);

        ammoText.gameObject.SetActive(false);
    }
}