using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    [SerializeField] private GameObject ui_container;
    //[SerializeField] private GameObject 
    private void Awake()
    {
        Instance = this;
        Instantiate(ui_container);
    }
}
