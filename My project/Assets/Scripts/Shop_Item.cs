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
            isPickUp = true;
            UiManager.Instance.EnablePurchaseText(price);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            isPickUp = false;
            UiManager.Instance.DisablePurchaseText();
        }
    }
    private void Purchase()
    {
        if (player.GetComponent<Player>().CheckGold() >= price)
        {
            // ��尡 �ִ� ���
            player.GetComponent<Player>().Purchase(price);
            gameObject.SetActive(false);
            GameObject item = GetComponentInParent<Shop>().items[index];
            item.GetComponent<BoxCollider2D>().enabled = true;

        }
        else UiManager.Instance.EnableAlermText("��尡 �����մϴ�.");
    }

}
