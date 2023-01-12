using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform player;
    private Vector3 tempPos;
    [SerializeField] private GameObject[] player_type;
    [SerializeField] private GameObject[] pool_container;

    private void Awake()
    {
        int weaponType = PlayerPrefs.GetInt("weaponType");
        //animator.SetInteger("WeaponType", weaponType);
        Debug.Log("Camera detected : " + weaponType + "th character.");
        for (int i = 0; i < player_type.Length; i++)
        {
            if (i + 1 == weaponType)
            {
                player_type[i].SetActive(true);
                player = player_type[i].transform;
            }
            else player_type[i].SetActive(false);
        }
        for (int i = 0; i < pool_container.Length; i++)
        {
            pool_container[i].SetActive(true);
        }
    }
    // Start is called before the first frame update
    /*void Start()
    {
        int weaponType = PlayerPrefs.GetInt("weaponType");
        //animator.SetInteger("WeaponType", weaponType);
        Debug.Log("Camera detected : " + weaponType + "th character.");
        for(int i = 0; i < player_type.Length; i++)
        {
            if (i + 1 == weaponType)
            {
                player_type[i].SetActive(true);
                player = player_type[i].transform;
            }
            else player_type[i].SetActive(false);
        }
        for(int i = 0; i < pool_container.Length; i++)
        {
            pool_container[i].SetActive(true);
        }
        //player = GameObject.FindWithTag("Player").transform;
    }*/
    
    // Update is called once per frame
    void LateUpdate()
    {
        tempPos = transform.position;
        tempPos.x = player.position.x;
        tempPos.y = player.position.y;

        transform.position = tempPos;
        
    }
}
