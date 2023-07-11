using UnityEngine;
using System.Collections;
using TMPro;
public class AmmoPickUp : MonoBehaviour
{
    public Weapon playerWeapon;
    int ammoRestored = 500;
    public TextMeshProUGUI ammoText;
    public float textDisplayDuration = 3f;
    private Coroutine textDisplayCoroutine;
    private void OnTriggerEnter(Collider other)
  {
        if (playerWeapon.totalAmmoCount == ammoRestored)
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
             playerWeapon.totalAmmoCount = ammoRestored;
             playerWeapon.UpdateAmmoCountText();
             Destroy(gameObject);
        }
  }
       

        private IEnumerator DisableHealthTextAfterDuration()
        {
            yield return new WaitForSeconds(textDisplayDuration);

            ammoText.gameObject.SetActive(false);
        }
    }
