using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] Vector3 difValue;
    private Transform player;
    private Vector3 tempPos;
    [SerializeField] private GameObject[] player_type;
    [SerializeField] private GameObject[] player_setting;
    [SerializeField] private GameObject[] knight_setting;
    [SerializeField] private GameObject[] ranger_setting;
    //[SerializeField] private GameObject[] hashashin_setting;
    [HideInInspector] public bool playerFollowing;

    private void Awake()
    {
        int weaponType = PlayerPrefs.GetInt("weaponType");
        // 선택된 플레이어 활성화
        for (int i = 0; i < player_type.Length; i++)
        {
            if (i + 1 == weaponType)
            {
                player_type[i].SetActive(true);
                player = player_type[i].transform;
            }
            else player_type[i].SetActive(false);
        }
        tempPos = transform.position;
        tempPos.x = player.position.x;
        tempPos.y = player.position.y;

        transform.position = tempPos;

        difValue = transform.position - player.position;
        difValue = new Vector3(Mathf.Abs(difValue.x), Mathf.Abs(difValue.y), 0f);

        for (int i = 0; i < player_setting.Length; i++)
        {
            player_setting[i].SetActive(true);
        }
        if (weaponType == 1)
        {
            // knight setting
            for (int i = 0; i < knight_setting.Length; i++)
            {
                knight_setting[i].SetActive(true);
            }
        }
        else if (weaponType == 2)
        {
            // ranger setting
            for (int i = 0; i < ranger_setting.Length; i++)
            {
                ranger_setting[i].SetActive(true);
            }
        }
        playerFollowing = true;
    }

    // Update is called once per frame
    /*void LateUpdate()
    {
        if (playerFollowing)
        {
            tempPos = transform.position;
            tempPos.x = player.position.x;
            tempPos.y = player.position.y;

            transform.position = tempPos;
        }
    }*/
    private void Update()
    {
        if (playerFollowing)
        {
            tempPos = Vector3.Lerp(this.transform.position, player.position + difValue, speed);
            this.transform.position = new Vector3(tempPos.x, tempPos.y, -10f);
        }
        //this.transform.position = Vector3.Lerp(this.transform.position, player.position + difValue, speed);
    }
}
