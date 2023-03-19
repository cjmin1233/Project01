using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUp_UI : MonoBehaviour
{
    private void OnEnable()
    {
        UI_Container.Instance.popup_ui_counter++;
    }
    private void OnDisable()
    {
        UI_Container.Instance.popup_ui_counter--;
    }
}
