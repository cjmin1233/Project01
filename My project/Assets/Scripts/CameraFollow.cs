using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
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

        for (int i = 0; i < player_type.Length; i++)
        {
            if (i + 1 == weaponType)
            {
                player_type[i].SetActive(true);
                player = player_type[i].transform;
            }
            else player_type[i].SetActive(false);
        }
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
    void LateUpdate()
    {
        if (playerFollowing)
        {
            tempPos = transform.position;
            tempPos.x = player.position.x;
            tempPos.y = player.position.y;

            transform.position = tempPos;
        }
    }
}
