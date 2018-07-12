using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPCamera : MonoBehaviour {
    public bool lockCursor;
    public float mouseSensitivity = 10;
    public Transform target;
    public float distFromTarget = 2;
    public Vector2 pitchMinMax = new Vector2(-40, 85);

    public float rotationSmoothTime = 0.12f;
    Vector3 rotationSmoothVel;
    Vector3 currentRotation;

    float yaw;
    float pitch;

    float curYaw;
    public Animator player;
	// Use this for initialization
	void Start () {
		if(lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
	}
	
	// Update is called once per frame
	void LateUpdate () {
        if (target != null)
        {
            yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
            pitch = Mathf.Clamp(pitch, pitchMinMax[0], pitchMinMax[1]);

            currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVel, rotationSmoothTime);

            transform.eulerAngles = currentRotation;

            transform.position = target.position - transform.forward * distFromTarget;
        }
	}

    public void LockCursor()
    {
        lockCursor = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
