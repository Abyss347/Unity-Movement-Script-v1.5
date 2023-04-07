using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraRelease : MonoBehaviour
{
    public GameObject PC;
    public GameObject CameraPosition;
    private Transform Cam;


    private void Start()
    {
        PC = GameObject.FindGameObjectWithTag("Player");
        CameraPosition = GameObject.FindGameObjectWithTag("CameraPosition");
    }

    void Awake()
    {
        //cursorlock goodness
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
    }


    void Update()
    {
        transform.position = CameraPosition.transform.position;
    }
} 