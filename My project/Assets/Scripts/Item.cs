using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    //public GameObject EquipIcon;
    private GameObject Player;
    private Rigidbody2D rb;
    [SerializeField] private int itemIndex;
    bool isPickUp;
    float theta;
    float floatingAmplitude=.2f;
    private void Awake()
    {
        //EquipIcon.gameObject.SetActive(false);
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (isPickUp && Input.GetKeyDown(KeyCode.E)) PickUp();
        Floating();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            Player = collision.gameObject;
            //EquipIcon.gameObject.SetActive(true);
            //EquipIcon.GetComponent<Animator>().SetBool("IsEnabled", true);
            isPickUp = true;
            UI_Container.Instance.EnableToggleText(itemIndex);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            //EquipIcon.gameObject.SetActive(false);
            //EquipIcon.GetComponent<Animator>().SetBool("IsEnabled", false);
            UI_Container.Instance.DisableToggleText(itemIndex);
            isPickUp = false;
        }
    }
    void PickUp()
    {
        if (CompareTag("Book"))
        {
            UI_Container.Instance.RandomAbility();
            //Player.GetComponent<SelectAbility>().RandomAbility();
            //Destroy(gameObject);
        }
        else if (CompareTag("Potion"))
        {
            Player.GetComponent<Player>().IncreaseMaxHP();
            Destroy(gameObject);
        }
        else if (CompareTag("Upgrade"))
        {
            UI_Container.Instance.UpgradeAbility();
            //Player.GetComponent<SelectAbility>().UpgradeAbility();
            //Destroy(gameObject);
        }
        else if (CompareTag("Chest"))
        {
            gameObject.GetComponent<Chest>().OpenChest();
            //Player.GetComponent<SelectAbility>().UpgradeAbility();
            //Destroy(gameObject);
            this.enabled = false;
        }
        else if (CompareTag("Coin"))
        {
            Player.GetComponent<Player>().GetGold(100);
            Destroy(gameObject);
        }
        else if (CompareTag("Food"))
        {
            Player.GetComponent<Player>().Heal(50);
            Destroy(gameObject);
        }
    }
    private void Floating()
    {
        theta = Time.time;
        rb.velocity = new Vector2(rb.velocity.x, floatingAmplitude * Mathf.Sin(theta*2));
    }
}
