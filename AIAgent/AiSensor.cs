using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AiSensor : MonoBehaviour
{
    public float viewRadius;
    [Range(0, 360)] public float viewAngle;
    public float scanDistanceOffset;
    public float heightOffset; // Adjust this value as needed

    [HideInInspector] public List<Transform> visibleTargets = new List<Transform>();//list for Targets
    [HideInInspector] public List<Transform> availableCoverPoints;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    private void Start()
    {
        StartCoroutine("FindTargetsWithDelay", .2f);
    }

    private IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets(); // finding target
            //FindCoverPoints();
        }
    }

    private void FindVisibleTargets()
    {
        visibleTargets.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            Vector3 scanPosition = transform.position + transform.forward * scanDistanceOffset + transform.up * heightOffset;

            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(scanPosition, target.position);

                if (!Physics.Raycast(scanPosition, dirToTarget, dstToTarget, obstacleMask))
                {
                    visibleTargets.Add(target);
                }
            }
        }
    }

    /*
    private void FindCoverPoints()
    {
        availableCoverPoints = CoverPointsManager.instance.GetAvailableCoverPoints();

        for (int i = availableCoverPoints.Count - 1; i >= 0; i--)
        {
            Transform coverPoint = availableCoverPoints[i];
            Vector3 dirToCoverPoint = (coverPoint.position - transform.position).normalized;

            Vector3 scanPosition = transform.position + transform.forward * scanDistanceOffset;

            float distanceToCoverPoint = Vector3.Distance(scanPosition, coverPoint.position);

            // Check if the cover point is within the view radius and view angle
            if (distanceToCoverPoint <= viewRadius && Vector3.Angle(transform.forward, dirToCoverPoint) <= viewAngle / 2)
            {
                float dstToCoverPoint = Vector3.Distance(scanPosition, coverPoint.position);

                if (Physics.Raycast(scanPosition, dirToCoverPoint, dstToCoverPoint, obstacleMask))
                {
                    // Remove cover point if there's an obstacle in the way
                    availableCoverPoints.RemoveAt(i);
                }
            }
            else
            {
                // Remove cover point if it's outside the view radius or view angle
                availableCoverPoints.RemoveAt(i);
            }
    
        }
    
    }
    */



    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
