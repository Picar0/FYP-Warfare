using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private Slider loadingSlider;

    public void SelectLevel(int levelNumber)
    {
        PlayerPrefs.SetInt("SelectedLevel", levelNumber);
        StartCoroutine(LoadLevel("TPSGameplay"));
    }

    IEnumerator LoadLevel(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        loadingScreen.SetActive(true);
        mainMenu.SetActive(false);

        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            loadingSlider.value = progress;

            yield return null;
        }
    }
}





