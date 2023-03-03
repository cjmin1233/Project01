using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBackgroundMover : MonoBehaviour
{
    public float offset;
    [SerializeField] private GameObject target;
    private Vector3 origin;

    private void Start()
    {
        origin = target.transform.position;
    }

    private void FixedUpdate()
    {
        float x_dif = target.transform.position.x - origin.x;
        float y_dif = target.transform.position.y - origin.y;

        transform.localPosition = new Vector3(offset * x_dif, offset * y_dif, 10f);
    }
}
