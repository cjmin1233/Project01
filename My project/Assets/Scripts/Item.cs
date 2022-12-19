using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    //public SelectAbility selectAbility;
    //public Button button;
    public GameObject EquipIcon;
    //public GameObject Canvas;
    private GameObject Player;
    private Rigidbody2D rb;
    bool isPickUp;
    float theta;
    float floatingAmplitude=.2f;
    void Start()
    {
        EquipIcon.gameObject.SetActive(false);
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isPickUp && Input.GetKeyDown(KeyCode.E)) PickUp();
        Floating();
;   }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            Player = collision.gameObject;
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
            //Player.GetComponent<SelectAbility>().RandomAbility();
        }
        else if(this.tag.Equals("Chest"))
        {
            Debug.Log("this is chest");
            Player.GetComponent<SelectAbility>().RandomAbility();
        }
    }
    private void Floating()
    {
        theta = Time.time;
        rb.velocity = new Vector2(rb.velocity.x, floatingAmplitude * Mathf.Sin(theta*2));
    }
}
