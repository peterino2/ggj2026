using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBox : MonoBehaviour
{
    public Camera cam;
    private BoxCollider2D camBox;
    [SerializeField] private float sizex, sizey, ratio;

    void Start()
    { 
        cam = GetComponent<Camera>();
        camBox = GetComponent<BoxCollider2D>();
         sizey = cam.orthographicSize * 2;
        ratio = (float)Screen.width / (float)Screen.height;
        sizex =  sizey * ratio;
        camBox.size = new Vector2(sizex, sizey);
    }

    void Update()
    {
       
    }

}