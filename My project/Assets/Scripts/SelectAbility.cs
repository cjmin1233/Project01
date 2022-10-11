using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectAbility : MonoBehaviour
{
    public GameObject Canvas;

    public GameObject[] abilityButtons;
    //public int[] abilityArray;
    public Transform[] buttonLocations;

    int totalWeight;
    int remainAbility;
    int current_weight;
    
    public void RandomAbility()
    {
        totalWeight = 0;
        remainAbility = 0;

        for(int i = 0; i < abilityButtons.Length; i++)
        {
            abilityButtons[i].gameObject.GetComponent<Ability>().isAppeared = false;
            current_weight = abilityButtons[i].gameObject.GetComponent<Ability>().weight;
            if (current_weight != 0)
            {
                totalWeight += current_weight;
                remainAbility++;
            }
        }


        if (remainAbility == 0)
        {
            Debug.Log("There's no remain ability");
        }
        else
        {
            //
            Debug.Log("Remain ability is : " + remainAbility);
            if (remainAbility > 3) remainAbility = 3;
            Canvas.gameObject.SetActive(true);

            for(int j = 0; j < remainAbility; j++)
            {
                int rand = Random.Range(0, totalWeight);
                int weight = 0;

                for(int i = 0; i < abilityButtons.Length; i++)
                {
                    current_weight = abilityButtons[i].gameObject.GetComponent<Ability>().weight;
                    if (current_weight == 0 || abilityButtons[i].gameObject.GetComponent<Ability>().isAppeared)
                    {
                        continue;
                    }
                    else
                    {
                        weight += current_weight;
                        if (rand < weight)
                        {
                            rand = i;
                            Debug.Log("Random index is : " + rand);
                            break;
                        }
                    }
                }

                //
                Vector3 buttonTransfrom = new Vector3(abilityButtons[rand].gameObject.transform.position.x, abilityButtons[rand].gameObject.transform.position.y, abilityButtons[rand].gameObject.transform.position.z);
                Vector3 locationVector = new Vector3(buttonLocations[j].transform.position.x, buttonLocations[j].transform.position.y, buttonLocations[j].transform.position.z);
                abilityButtons[rand].transform.Translate(locationVector - buttonTransfrom);
                abilityButtons[rand].gameObject.SetActive(true);
                abilityButtons[rand].gameObject.GetComponent<Ability>().isAppeared = true;
                totalWeight -= abilityButtons[rand].gameObject.GetComponent<Ability>().weight;

            }
        }












        /*
        int availableAbility = 0, index = 0;
        while (index < abilityArray.Length)
        {
            if (abilityArray[index] == 0 && availableAbility < 3) availableAbility++;
            index++;
        }
        if (availableAbility == 0)
        {
            Debug.Log("There's no ability remain");
            return;
        }
        Debug.Log(availableAbility);
        while (availableAbility > 0)
        {
            int rand = Random.Range(0, abilityButtons.Length);
            if (abilityArray[rand] == 1) continue;
            //Debug.Log(Ability_tier_1[rand]);
            Vector3 buttonTransfrom = new Vector3(abilityButtons[rand].gameObject.transform.position.x, abilityButtons[rand].gameObject.transform.position.y, abilityButtons[rand].gameObject.transform.position.z);
            Vector3 locationVector = new Vector3(buttonLocations[availableAbility-1].transform.position.x, buttonLocations[availableAbility-1].transform.position.y, buttonLocations[availableAbility-1].transform.position.z);
            abilityButtons[rand].transform.Translate(locationVector-buttonTransfrom);
            abilityButtons[rand].gameObject.SetActive(true);
            availableAbility--;
        }*/
    }
    public void GetAbility(int index)
    {
        abilityButtons[index].gameObject.GetComponent<Ability>().isSelected = true;

        Debug.Log("Selected Ability : " + index + "th ability.");
        int a_type = abilityButtons[index].gameObject.GetComponent<Ability>().Type;
        int a_tier = abilityButtons[index].gameObject.GetComponent<Ability>().Tier;
        Debug.Log("Ability Type : " + a_type);
        Debug.Log("Ability Tier : " + a_tier);
        Debug.Log("Ability Level : " + abilityButtons[index].gameObject.GetComponent<Ability>().level);
        Debug.Log("Ability Weight : " + abilityButtons[index].gameObject.GetComponent<Ability>().weight);

        abilityButtons[index].gameObject.GetComponent<Ability>().weight = 0;

        // Apply ability

        // *************

        for(int i = 0; i < abilityButtons.Length; i++)
        {
            if (abilityButtons[i].gameObject.GetComponent<Ability>().Type == a_type && abilityButtons[i].gameObject.GetComponent<Ability>().isSelected == false)
            {
                abilityButtons[i].gameObject.GetComponent<Ability>().weight += 1;
                if (abilityButtons[i].gameObject.GetComponent<Ability>().weight == a_tier + 1)
                {
                    abilityButtons[i].gameObject.GetComponent<Ability>().weight += 1;
                }
            }
            //if (abilityButtons[i].gameObject.GetComponent<Ability>().appeared) abilityButtons[i].gameObject.GetComponent<Ability>().appeared = false;
            abilityButtons[i].gameObject.SetActive(false);
        }
    }
}
