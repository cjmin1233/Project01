using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] Vector3 difValue;
    private Transform player;
    private Vector3 tempPos;
    [HideInInspector] public bool playerFollowing;

    private void OnEnable()
    {
        int weaponType = PlayerPrefs.GetInt("weaponType");
        player = GameObject.FindGameObjectWithTag("Player").transform;
        tempPos = transform.position;
        tempPos.x = player.position.x;
        tempPos.y = player.position.y;

        transform.position = tempPos;

        difValue = transform.position - player.position;
        difValue = new Vector3(Mathf.Abs(difValue.x), Mathf.Abs(difValue.y), 0f);

        playerFollowing = true;
    }
    private void FixedUpdate()
    {
        if (playerFollowing)
        {
            tempPos = Vector3.Lerp(this.transform.position, player.position + difValue, speed);
            this.transform.position = new Vector3(tempPos.x, tempPos.y, -10f);
        }
    }
}
