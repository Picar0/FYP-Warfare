using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Level Data")]
public class LevelData : ScriptableObject
{
    public GameObject cutscene;
    public GameObject gameplay;
    public GameObject props;
    public GameObject mapBlockers;
    public GameObject finishPoint;
}
