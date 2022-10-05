using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    [SerializeField] private GameObject EnterIcon;
    [SerializeField] private GameObject Player;
    bool isPickUp;
    void Awake()
    {
        EnterIcon.gameObject.SetActive(false);
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
            EnterIcon.gameObject.SetActive(true);
            EnterIcon.GetComponent<Animator>().SetBool("IsEnabled", true);
            isPickUp = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            //EquipIcon.gameObject.SetActive(false);
            EnterIcon.GetComponent<Animator>().SetBool("IsEnabled", false);
            isPickUp = false;
        }
    }
    void PickUp()
    {
        int weapontype = Player.GetComponent<Animator>().GetInteger("WeaponType");

        if (weapontype > 0)
        {

            PlayerPrefs.SetInt("weaponType", weapontype);
            Scene scene = SceneManager.GetActiveScene();
            Debug.Log("Active Scene is '" + scene.name + "'.");
            Debug.Log("move to the next scene");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        }
        else Debug.Log("Choose a weapon");

    }
}
