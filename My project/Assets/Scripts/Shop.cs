using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private GameObject food;
    [SerializeField] private GameObject potion;
    [SerializeField] private GameObject book;
    [SerializeField] private GameObject upgrade;
    [SerializeField] private Transform[] items_pos;
    [HideInInspector] public GameObject[] items = new GameObject[3];
    private void Awake()
    {
        items[0] = Instantiate(food, items_pos[0].position + new Vector3(0, 1, 0), items_pos[0].rotation);
        items[0].GetComponent<BoxCollider2D>().enabled = false;
        items[1] = Instantiate(potion, items_pos[1].position + new Vector3(0, 1, 0), items_pos[2].rotation);
        items[1].GetComponent<BoxCollider2D>().enabled = false;

        int rand = Random.Range(0, 2);
        if(rand>0) items[2] = Instantiate(upgrade, items_pos[2].position + new Vector3(0, 1, 0), items_pos[2].rotation);
        else items[2] = Instantiate(book, items_pos[2].position + new Vector3(0, 1, 0), items_pos[2].rotation);
        items[2].GetComponent<BoxCollider2D>().enabled = false;
    }
}
