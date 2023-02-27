using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBackgroundMover : MonoBehaviour
{
    public float offset;
    private GameObject cam;
    private Vector3 origin;

    private void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera");
        origin = cam.transform.position;
    }

    private void FixedUpdate()
    {
        float x_dif = cam.transform.position.x - origin.x;
        float y_dif = cam.transform.position.y - origin.y;

        transform.localPosition = new Vector3(offset * x_dif, offset * y_dif, 10f);
    }
}
