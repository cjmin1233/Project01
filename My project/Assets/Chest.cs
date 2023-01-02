using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] private List<GameObject> items;
    public void OpenChest()
    {
        int rand = Random.Range(0, items.Count);
        Instantiate(items[rand], transform.position, transform.rotation);
    }
}
