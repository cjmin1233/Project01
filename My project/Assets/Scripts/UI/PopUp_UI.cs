using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUp_UI : MonoBehaviour
{
    private void OnEnable()
    {
        UiManager.Instance.popup_ui_counter++;
    }
    private void OnDisable()
    {
        UiManager.Instance.popup_ui_counter--;
    }
}
