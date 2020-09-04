using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public RotationAxes axes = RotationAxes.MouseXAndY;
    public float sensitivityHor = 9.0f;
    public float sensitivityVert = 9.0f;
    public float minimumVert = -45.0f;
    public float maximumVert = 45.0f;
    private float rotationX = 0;
    private float minYRot, maxYRot, yRot;
    private bool clamp;

    public void ClampYRotation(float minYRot, float maxYRot)
    {
        yRot = transform.localEulerAngles.y;
        this.minYRot = minYRot;
        this.maxYRot = maxYRot;
        clamp = true;
    }

    public void StopYRotationClamping()
    {
        clamp = false;
    }

    // Use this for initialization
    void Start ()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Rigidbody body = GetComponent<Rigidbody>();
        if (body != null)
        {
            body.freezeRotation = true;
            Debug.Log(body != null);
        }
    }

    // Update is called once per frame
    void Update()
    {
        float rotationZ = transform.localEulerAngles.z;

        if (axes == RotationAxes.MouseX)
        {
            float delta = Input.GetAxis("Mouse X") * sensitivityHor;
            float rotationY = transform.localEulerAngles.y + delta;

            if (clamp)
            {
                rotationY = rotationY > 180.0f ? rotationY - 360.0f : rotationY;
                rotationY = Mathf.Clamp(rotationY, minYRot, maxYRot);
            }

            transform.localEulerAngles = new Vector3(rotationX, rotationY, rotationZ);
        }
        else if (axes == RotationAxes.MouseY)
        {
            rotationX -= Input.GetAxis("Mouse Y") * sensitivityVert;
            rotationX = Mathf.Clamp(rotationX, minimumVert, maximumVert);
            float rotationY = transform.localEulerAngles.y;
            transform.localEulerAngles = new Vector3(rotationX, rotationY, rotationZ);
        }
        else
        {
            rotationX -= Input.GetAxis("Mouse Y") * sensitivityVert;
            rotationX = Mathf.Clamp(rotationX, minimumVert, maximumVert);
            float delta = Input.GetAxis("Mouse X") * sensitivityHor;
            float rotationY = transform.localEulerAngles.y + delta;

            if (clamp)
            {
                rotationY = rotationY > 180.0f ? rotationY - 360.0f : rotationY;
                rotationY = Mathf.Clamp(rotationY, minYRot, maxYRot);
            }

            transform.localEulerAngles = new Vector3(rotationX, rotationY, rotationZ);
        }
    }
}
