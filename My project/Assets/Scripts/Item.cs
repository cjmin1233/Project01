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
            isPickUp = true;
            UI_Container.Instance.EnableToggleText(itemIndex);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            UI_Container.Instance.DisableToggleText(itemIndex);
            isPickUp = false;
        }
    }
    void PickUp()
    {
        if (CompareTag("Book"))
        {
            if (UI_Container.Instance.GetRandomAbility()) Destroy(gameObject);
        }
        else if (CompareTag("Potion"))
        {
            Player.GetComponent<Player>().IncreaseMaxHP();
            Destroy(gameObject);
        }
        else if (CompareTag("Upgrade"))
        {
            if (UI_Container.Instance.UpgradeRandomAbility()) Destroy(gameObject);
        }
        else if (CompareTag("Chest"))
        {
            gameObject.GetComponent<Chest>().OpenChest();
            enabled = false;
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
