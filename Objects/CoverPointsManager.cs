using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverPointsManager : MonoBehaviour
{
    public static CoverPointsManager instance;
    public List<Transform> coverPoints = new List<Transform>();
    public List<Transform> occupiedCoverPoints = new List<Transform>();


    private void Awake()
    {
        instance = this;
    }

    public List<Transform> GetAvailableCoverPoints()
    {
        List<Transform> availableCoverPoints = new List<Transform>();

        foreach (Transform coverPoint in coverPoints)
        {
            if (!occupiedCoverPoints.Contains(coverPoint))
            {
                availableCoverPoints.Add(coverPoint);
            }
        }

        return availableCoverPoints;
    }


    public void OccupyCoverPoint(Transform coverPoint)
    {
        occupiedCoverPoints.Add(coverPoint);
    }

    public void ReleaseCoverPoint(Transform coverPoint)
    {
        occupiedCoverPoints.Remove(coverPoint);
    }
}
