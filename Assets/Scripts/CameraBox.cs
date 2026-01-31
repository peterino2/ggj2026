using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBox : MonoBehaviour
{
    public Camera cam;
    public Vector2 playSize;
    private BoxCollider2D camBox;
    private float sizex, sizey, ratio;

    void Start()
    { 
        cam = GetComponent<Camera>();
        camBox = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        sizey = cam.orthographicSize * 2f;
        ratio = (Screen.width / (float)Screen.height);
        sizex =  sizey * ratio;
        camBox.size = new Vector2(sizex, sizey);
    }

}