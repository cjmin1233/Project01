using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    //public SelectAbility selectAbility;
    //public Button button;
    public GameObject EquipIcon;
    public GameObject Player;
    public GameObject Canvas;
    bool isPickUp;
    void Start()
    {
        EquipIcon.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isPickUp && Input.GetKeyDown(KeyCode.E)) PickUp();
;   }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            EquipIcon.gameObject.SetActive(true);
            EquipIcon.GetComponent<Animator>().SetBool("IsEnabled", true);
            isPickUp = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            //EquipIcon.gameObject.SetActive(false);
            EquipIcon.GetComponent<Animator>().SetBool("IsEnabled", false);
            isPickUp = false;
        }
    }
    void PickUp()
    {
        if (this.tag.Equals("Sword"))
        {
            //Debug.Log("This is Sword");
            Player.GetComponent<Animator>().SetInteger("WeaponType", 1);
        }
        else if (this.tag.Equals("Bow"))
        {
            //Debug.Log("This is Bow");
            Player.GetComponent<Animator>().SetInteger("WeaponType", 2);
            //Canvas.gameObject.SetActive(true);
            Player.GetComponent<SelectAbility>().RandomAbility();
        }

    }
}
