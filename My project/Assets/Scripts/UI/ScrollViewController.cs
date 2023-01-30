using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollViewController : MonoBehaviour
{
    /*private ScrollRect scrollRect;
    float space = 50f;

    [HideInInspector] public List<GameObject> SelectedAbilityList = new List<GameObject>();
    
    public void AddToBook(GameObject gameObject)
    {
        if (scrollRect == null) scrollRect = GetComponent<ScrollRect>();


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
    }
    private void UpdateBook()
    {
        float y = 0f;
        for (int i = 0; i < SelectedAbilityList.Count; i++)
        {
            RectTransform rectTransform = SelectedAbilityList[i].GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(0f, -y);
            y += rectTransform.sizeDelta.y + space;
        }

        scrollRect.content.sizeDelta = new Vector2(scrollRect.content.sizeDelta.x, y);

    }*/
}
