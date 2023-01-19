using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleTextContainer : MonoBehaviour
{
    public static ToggleTextContainer Instance { get; private set; }
    [SerializeField] private GameObject[] toggleText;
    private void OnEnable()
    {
        Instance = this;
    }
    public void EnableToggleText(int idx)
    {
        //Debug.Log("Enable toggle text : " + idx);
        toggleText[idx].GetComponent<Animator>().SetBool("IsEnabled", true);
    }
    public void DisableToggleText(int idx)
    {
        //Debug.Log("Disable toggle text : " + idx);
        toggleText[idx].GetComponent<Animator>().SetBool("IsEnabled", false);
    }

}
