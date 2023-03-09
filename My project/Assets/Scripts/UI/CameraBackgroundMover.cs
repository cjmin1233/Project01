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
        SettingOrigin();
    }

    private void FixedUpdate()
    {
        float x_dif = target.transform.position.x - origin.x;
        float y_dif = target.transform.position.y - origin.y;

        transform.localPosition = new Vector3(offset * x_dif, offset * y_dif, 10f);
    }
    public void SettingOrigin()
    {
        Transform player_pos = GameObject.FindGameObjectWithTag("Player").transform;
        if (player_pos != null) origin = player_pos.position;
        else origin = target.transform.position;
    }
}
