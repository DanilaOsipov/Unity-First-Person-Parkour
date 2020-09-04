using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

static class MouseLookManager
{
    private static MouseLook player = GameObject.Find("Player").GetComponent<MouseLook>();
    public static MouseLook Player => player ?? new MouseLook();

    private static MouseLook mainCamera = GameObject.Find("Main Camera").GetComponent<MouseLook>();
    public static MouseLook MainCamera => mainCamera ?? new MouseLook();

    private static Transform character = GameObject.Find("Player").transform;

    public static void PausePlayerRotation()
    {
        Player.enabled = false;
        MainCamera.axes = RotationAxes.MouseXAndY;
    }

    public static void ResumePlayerRotation()
    {
        Player.enabled = true;
        MainCamera.axes = RotationAxes.MouseY;

        Vector3 euler = character.eulerAngles;
        euler.y = MainCamera.transform.eulerAngles.y;
        character.rotation = Quaternion.Euler(euler);

        euler = MainCamera.transform.localEulerAngles;
        euler.y = 0;
        MainCamera.transform.localEulerAngles = euler;
    }
}

