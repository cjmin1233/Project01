using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    //[SerializeField] private GameObject EnterIcon;
    private GameObject Player;
    bool isPickUp;
    /*void Awake()
    {
        EnterIcon.SetActive(false);
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
            ToggleTextContainer.Instance.EnableToggleText(4);
            isPickUp = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            ToggleTextContainer.Instance.DisableToggleText(4);
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
            Player.transform.position = new Vector3(0f, 0f, 0f);
            ToggleTextContainer.Instance.DisableToggleText(4);
        }
        else Debug.Log("Choose a weapon");

    }
}
