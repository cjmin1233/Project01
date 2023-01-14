using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hashashin_Enemy_Finder : MonoBehaviour
{
    [SerializeField] private GameObject player;
    /*private void Awake()
    {
        Debug.Log("this is awake ");
        gameObject.SetActive(false);
    }*/
    private void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = collision.tag;
        string name = collision.name;

        if(tag=="Enemy" || tag == "Boss")
        {
            Debug.Log("enemy found");
            player.transform.position = collision.GetComponent<Transform>().position;
            player.transform.Translate(new Vector3(0, 1f, 0));
            //gameObject.SetActive(false);
        }
    }
}
