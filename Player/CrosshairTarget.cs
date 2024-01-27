using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairTarget : MonoBehaviour
{
    private Camera _camera;
    private GameObject _player;
    private float maxRayDistance = 1000.0f;
    private RaycastHit hitInfo;

    private void Awake()
    {
        if (_camera == null)
        {
            _camera = Camera.main;
        }
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        //offset to cast ray based on the distance from the camera to the player
        float cameraDistanceOffset = Vector3.Distance(_camera.transform.position, _player.transform.position);

        //cast a ray from the center of the camera plus the offset, limited to maxRayDistance
        bool isHit = Physics.Raycast(_camera.transform.position + _camera.transform.forward * cameraDistanceOffset, _camera.transform.forward, out hitInfo, maxRayDistance);

        //set the crosshairTarget only if the ray hits something
        if (isHit)
        {
            transform.position = hitInfo.point;
        }
        else
        {
            // set the crosshairTarget to the maximum distance if the ray doesn't hit anything
            transform.position = _camera.transform.position + _camera.transform.forward * maxRayDistance;
        }
      
    }

}
