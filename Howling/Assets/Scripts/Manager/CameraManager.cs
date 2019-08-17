using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Camera playerCamera; // main camera
    public Camera wolfCamera;


    public void ShowPlayerView()
    {
        playerCamera.enabled = true;
        wolfCamera.enabled = false;
    }
    
    public void ShowWolfView()
    {
        playerCamera.enabled = false;
        wolfCamera.enabled = true;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.C))
        {
            ShowWolfView();
        }
        if (Input.GetKey(KeyCode.P))
        {
            ShowPlayerView();
        }
    }
}
