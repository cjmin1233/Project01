using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    //[SerializeField] private GameObject EnterIcon;
    [SerializeField] private Transform destination;
    //[SerializeField] private int toggleTextIndex;
    private GameObject Player;
    bool isPickUp;
    void Awake()
    {
        //EnterIcon.SetActive(false);
    }
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
        /*int weapontype = Player.GetComponent<Animator>().GetInteger("WeaponType");

        if (weapontype > 0)
        {

            *//*PlayerPrefs.SetInt("weaponType", weapontype);
            Scene scene = SceneManager.GetActiveScene();
            Debug.Log("Active Scene is '" + scene.name + "'.");
            Debug.Log("move to the next scene");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            Player.transform.position = new Vector3(0f, 0f, 0f);
            EnterIcon.GetComponent<Animator>().SetBool("IsEnabled", false);*//*
        }
        else Debug.Log("Choose a weapon");*/
        Debug.Log("This is portal");
        Player.transform.position = destination.position;
        UI_Container.Instance.DisableToggleText(4);
    }
}
