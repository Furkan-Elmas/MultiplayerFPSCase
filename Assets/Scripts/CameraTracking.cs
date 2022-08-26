using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CameraTracking : NetworkBehaviour
{
    [SerializeField] float _verticalSensitivity = 1.0f;
    [SerializeField] float _verticalOffset = -2.0f;
    [SerializeField] float _horizontalOffset = 3.0f;

    Camera _mainCam;
    GameObject _target;
    float _angle;
    float _xRotation;


    void Start()
    {
        if (!isLocalPlayer)
            return;
            
        if (Camera.main != null)
            _mainCam = Camera.main;
        else
            _mainCam = new Camera();

        _target = NetworkClient.localPlayer.gameObject;
        _mainCam.transform.position = _target.transform.position - new Vector3(-0.5f, _verticalOffset, _horizontalOffset);
        _mainCam.transform.SetParent(transform);
    }

    void LateUpdate()
    {
        if (!isLocalPlayer)
            return;

        float mouseY = Input.GetAxis("Mouse Y") * _verticalSensitivity;
        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -5f, 15f);

        _mainCam.transform.localRotation = Quaternion.Euler(_xRotation, _mainCam.transform.rotation.y, _mainCam.transform.rotation.z);
    }
}
