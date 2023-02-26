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
        UI_Container.Instance.StartFadeFlow();
        Scene scene = SceneManager.GetActiveScene();
        Debug.Log("Active Scene is '" + scene.name + "'.");
        Debug.Log("move to the next scene");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        Player.transform.position = new Vector3(0f, 0f, 0f);
        UI_Container.Instance.DisableToggleText(4);
        UI_Container.Instance.fade_in_start = true;
    }
}
