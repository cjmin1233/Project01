using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseFocusMover : MonoBehaviour
{
    public float offset;
    private Vector3 origin;

    // Start is called before the first frame update
    void Start()
    {
        origin = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(origin.x + (offset * Input.mousePosition.x),
            origin.y + (offset * Input.mousePosition.y), origin.z);
    }
}
