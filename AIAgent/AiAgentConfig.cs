using UnityEngine;

[CreateAssetMenu()]
public class AiAgentConfig : ScriptableObject
{
   // public float maxTime = 1.0f;
    public float maxDistance = 1.0f;
    public float dieForce = 3.0f; 
    public float searchRange = 10.0f; // used in ai search state 
}
