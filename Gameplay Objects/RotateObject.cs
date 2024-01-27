using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [SerializeField] private int rotationSpeed;

  
    void Update()
    {
        transform.Rotate(0, rotationSpeed, 0, Space.World);
    }
}
