using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    //[SerializeField] private GameObject EnterIcon;
    [SerializeField] private Transform[] destination;
    //[SerializeField] private int toggleTextIndex;
    private GameObject Player;
    bool isPickUp;
    /*void Awake()
    {
        //EnterIcon.SetActive(false);
    }*/
    // Update is called once per frame
    void Update()
    {
        if (isPickUp && Input.GetKeyDown(KeyCode.E)) PickUp();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            Player = collision.gameObject;
            //EnterIcon.SetActive(true);
            //EnterIcon.GetComponent<Animator>().SetBool("IsEnabled", true);
            UI_Container.Instance.EnableToggleText(4);
            isPickUp = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            UI_Container.Instance.DisableToggleText(4);
            isPickUp = false;
        }
    }
    void PickUp()
    {
        //StartCoroutine(UI_Container.Instance.FadeFlow());
        //UI_Container.Instance.StartFadeFlow();
        Debug.Log("This is portal");
        int rand = Random.Range(0, destination.Length);
        StartCoroutine(GameManager.Instance.TransportFlow(destination[rand].position));
        //GameManager.Instance.PlayerTransport(destination[rand].position);
        //Player.transform.position = destination[rand].position;
        UI_Container.Instance.DisableToggleText(4);
        //UI_Container.Instance.fade_in_start = true;
    }
}
