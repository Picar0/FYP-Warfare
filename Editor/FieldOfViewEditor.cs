using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(AiSensor))]
public class FieldOfViewEditor : Editor
{
    void OnSceneGUI()
    {
        AiSensor aiSensor = (AiSensor)target;
        Handles.color = Color.red;

        Vector3 viewAngleA = aiSensor.DirFromAngle(-aiSensor.viewAngle / 2, false);
        Vector3 viewAngleB = aiSensor.DirFromAngle(aiSensor.viewAngle / 2, false);

        Vector3 scanPosition = aiSensor.transform.position + aiSensor.transform.forward * aiSensor.scanDistanceOffset;

        Handles.DrawWireArc(scanPosition, Vector3.up, viewAngleA, aiSensor.viewAngle, aiSensor.viewRadius);
        Handles.DrawLine(scanPosition, scanPosition + viewAngleA * aiSensor.viewRadius);
        Handles.DrawLine(scanPosition, scanPosition + viewAngleB * aiSensor.viewRadius);

        Handles.color = Color.yellow;
        foreach (Transform visibleTarget in aiSensor.visibleTargets)
        {
            if (visibleTarget != null)
            {
                Handles.DrawLine(scanPosition, visibleTarget.position);
            }
        }

        Handles.color = Color.green;
        foreach (Transform availableCoverPoint in aiSensor.availableCoverPoints)
        {
            if (availableCoverPoint != null)
            {
                Handles.DrawLine(scanPosition, availableCoverPoint.position);
            }
        }
    }
}
