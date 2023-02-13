using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop_Item : MonoBehaviour
{
    [SerializeField] private int index;
    [SerializeField] private int price;
    bool isPickUp;
    private GameObject player;
    private void Update()
    {
        if (isPickUp && Input.GetKeyDown(KeyCode.E)) Purchase();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            player = collision.gameObject;
            //EquipIcon.gameObject.SetActive(true);
            //EquipIcon.GetComponent<Animator>().SetBool("IsEnabled", true);
            isPickUp = true;
            UI_Container.Instance.EnablePurchaseText(price);
            //UI_Container.Instance.EnableToggleText(itemIndex);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            //EquipIcon.gameObject.SetActive(false);
            //EquipIcon.GetComponent<Animator>().SetBool("IsEnabled", false);
            //UI_Container.Instance.DisableToggleText(itemIndex);
            isPickUp = false;
            UI_Container.Instance.DisablePurchaseText();
        }
    }
    private void Purchase()
    {
        /*if (this.tag.Equals("Book"))
        {
            UI_Container.Instance.RandomAbility();
            //Player.GetComponent<SelectAbility>().RandomAbility();
            //Destroy(gameObject);
        }*/
        if (player.GetComponent<Player>().CheckGold() >= price)
        {
            // 골드가 있는 경우
            player.GetComponent<Player>().Purchase(price);
            gameObject.SetActive(false);
            GameObject item = GetComponentInParent<Shop>().items[index];
            item.GetComponent<BoxCollider2D>().enabled = true;

        }
        else Debug.Log("골드가 부족합니다");
    }

}
