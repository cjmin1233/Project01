using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] private GameObject[] items;
    public void OpenChest()
    {
        int rand = Random.Range(0, items.Length);
        Instantiate(items[rand], transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
