using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    //TestAbility testAbility;
    public Text description;
    public Image image;
    public void SetCardDisplay(TestAbility example)
    {
        description.text = example.a_Description;
        image.sprite = example.artwork;
    }
}
