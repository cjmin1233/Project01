using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollViewController : MonoBehaviour
{
    private ScrollRect scrollRect;
    float space = 50f;
    //public GameObject uiPrefab;
    //public List<RectTransform> uiObjects = new List<RectTransform>();
    //public List<GameObject> AbilityList = new List<GameObject>();
    [HideInInspector] public List<GameObject> SelectedAbilityList = new List<GameObject>();
    /*
    void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
    }
    public void AddNewUiObject()
    {
        var newUi = Instantiate(uiPrefab, scrollRect.content).GetComponent<RectTransform>();
        uiObjects.Add(newUi);

        float y = 0f;
        for (int i = 0; i < uiObjects.Count; i++)
        {
            uiObjects[i].anchoredPosition = new Vector2(0f, -y);
            y += uiObjects[i].sizeDelta.y + space;
        }

        scrollRect.content.sizeDelta = new Vector2(scrollRect.content.sizeDelta.x, y);
    }
     */
    public void AddToBook(GameObject gameObject)
    {
        if (scrollRect == null) scrollRect = GetComponent<ScrollRect>();
        /*int index = gameObject.GetComponent<Ability>().index;
        Debug.Log("The selected ability index is : " + index);
        var newUi = Instantiate(AbilityList[index], scrollRect.content);
        SelectedAbilityList.Add(newUi);*/

        //*********************
        if (gameObject.GetComponent<Ability>().level == 1)
        {
            // 새로운 어빌리티 선택
            var newUi = Instantiate(gameObject, scrollRect.content);
            newUi.SetActive(true);
            newUi.GetComponent<Button>().enabled = false;
            SelectedAbilityList.Add(newUi);
        }
        else
        {
            // 선택한 능력 강화
            for(int i = 0; i < SelectedAbilityList.Count; i++)
            {
                if (SelectedAbilityList[i].GetComponent<Ability>().index == gameObject.GetComponent<Ability>().index)
                {
                    SelectedAbilityList[i].GetComponent<Ability>().level = gameObject.GetComponent<Ability>().level;
                    break;
                }
            }
        }


        UpdateBook();
        /*
        GameObject newUi = Instantiate(gameObject, scrollRect.content);
        newUi.SetActive(true);
        AbilityList.Add(newUi);

        float y = 0f;
        for (int i = 0; i < AbilityList.Count; i++)
        {
            //uiObjects[i].anchoredPosition = new Vector2(0f, -y);
            //y += uiObjects[i].sizeDelta.y + space;
            RectTransform rectTransform = AbilityList[i].GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(0f, -y);
            y += rectTransform.sizeDelta.y + space;
        }

        scrollRect.content.sizeDelta = new Vector2(scrollRect.content.sizeDelta.x, y);
        */
    }
    private void UpdateBook()
    {
        float y = 0f;
        for (int i = 0; i < SelectedAbilityList.Count; i++)
        {
            //uiObjects[i].anchoredPosition = new Vector2(0f, -y);
            //y += uiObjects[i].sizeDelta.y + space;
            RectTransform rectTransform = SelectedAbilityList[i].GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(0f, -y);
            y += rectTransform.sizeDelta.y + space;
        }

        scrollRect.content.sizeDelta = new Vector2(scrollRect.content.sizeDelta.x, y);

    }
}
