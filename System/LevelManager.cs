using UnityEngine;
using System.Collections;
using UnityEngine.Playables;
using UnityEngine.UI;


public class LevelManager : MonoBehaviour
{
    // Levels Data
    [System.Serializable]
    private class LevelObjects
    {
        public GameObject cutscene;
        public GameObject gameplay;
        public GameObject props;
        public GameObject mapBlockers;
        public GameObject finishPoint;
        public GameObject objectiveText;
    }

    // An array to store instances of LevelObjects
    [SerializeField] private LevelObjects[] levels;

    //Gameplay references
    [SerializeField] private GameObject crosshairTarget;

    [SerializeField] private GameObject gameUI;

    [SerializeField] private GameObject objectiveTitle;

    [SerializeField] private GameObject cutsceneUI;

    [SerializeField] private Button skipButton;

    private float objectiveDelay = 3.0f;
    private float SkipButtonDelay = 5.0f;
    private int selectedLevel;
    private bool cutsceneSkipped = false;

    void Start()
    {
        // Default to level 1 
        selectedLevel = PlayerPrefs.GetInt("SelectedLevel", 1);
        ActivateLevel(selectedLevel);

        // Enable Skip Button after a 5-second delay
        StartCoroutine(EnableSkipButtonAfterDelay(SkipButtonDelay));
    }

    private IEnumerator EnableSkipButtonAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Enable Skip Button
        cutsceneUI.SetActive(true);

        // Register the button click event
        if (skipButton != null)
        {
            skipButton.onClick.AddListener(SkipCutscene);
        }
    }

    private void ActivateLevel(int levelNumber)
    {
        if (levelNumber >= 1 && levelNumber <= levels.Length)
        {
            LevelObjects selectedLevelObjects = levels[levelNumber - 1];
            if (selectedLevelObjects.cutscene != null)
            {
                // Enable cutscene
                selectedLevelObjects.cutscene.SetActive(true);

                PlayableDirector cutsceneDirector = selectedLevelObjects.cutscene.GetComponent<PlayableDirector>();

                // Subscribe to the cutscene's played event
                cutsceneDirector.stopped += OnCutsceneFinished;
            }
        }
        else
        {
            Debug.LogError("Level not found: " + levelNumber);
        }
    }

    // On Cutscene Finished
    private void OnCutsceneFinished(PlayableDirector director)
    {
        // Unsubscribe from the event to avoid memory leaks
        director.stopped -= OnCutsceneFinished;

        // Disable cutscene
        levels[selectedLevel - 1].cutscene.SetActive(false);


        // Enable other levels data
        LevelObjects selectedLevelObjects = levels[selectedLevel - 1];

        selectedLevelObjects.gameplay.SetActive(true);

        selectedLevelObjects.props.SetActive(true);

        selectedLevelObjects.mapBlockers.SetActive(true);

        crosshairTarget.SetActive(true);

        gameUI.SetActive(true);

        cutsceneUI.SetActive(false);

    }

    //Cutscene skip button
    private void SkipCutscene()
    {
        if (selectedLevel >= 1 && selectedLevel <= levels.Length && !cutsceneSkipped)
        {
            cutsceneSkipped = true;

            // Unsubscribe from the cutscene finished event
            PlayableDirector cutsceneDirector = levels[selectedLevel - 1].cutscene.GetComponent<PlayableDirector>();
            cutsceneDirector.stopped -= OnCutsceneFinished;

            // Disable the cutscene
            levels[selectedLevel - 1].cutscene.SetActive(false);

            // Enable other levels data
            LevelObjects selectedLevelObjects = levels[selectedLevel - 1];

            selectedLevelObjects.gameplay.SetActive(true);

            selectedLevelObjects.props.SetActive(true);

            selectedLevelObjects.mapBlockers.SetActive(true);

            crosshairTarget.SetActive(true);

            gameUI.SetActive(true);

            cutsceneUI.SetActive(false);

            objectiveTitle.SetActive(true);

            selectedLevelObjects.objectiveText.SetActive(true);

            StartCoroutine(HideObjectiveAfterDelay(objectiveDelay));

        }
        else
        {
            Debug.LogError("Level not found or cutscene already skipped: " + selectedLevel);
        }
    }


    private IEnumerator HideObjectiveAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        objectiveTitle.SetActive(false);
        LevelObjects selectedLevelObjects = levels[selectedLevel - 1];
        selectedLevelObjects.objectiveText.SetActive(false);

    }
}
