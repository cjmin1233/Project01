using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectAbility : MonoBehaviour
{
    public GameObject Ability_UI;

    public GameObject[] abilityButtons;
    public Transform[] buttonLocations;

    int totalWeight;
    int remainAbility;
    int current_weight;

    /*****************
    //public TestAbility[] abilityBook;   // �����Ƽ ����
    //public GameObject[] threeChoices;

    //int baseWeight = 1;

    bool isZselected = false;
    bool isXselected = false;
    bool canAppearTier2 = false;
    bool canAppearTier3 = false;
    Color color;
    ******************/

    private void Start()
    {
        for (int i = 0; i < abilityButtons.Length; i++)
        {
            abilityButtons[i].GetComponent<Ability>().index = i;
        }
    }
    public void RandomAbility()
    {
        totalWeight = 0;
        remainAbility = 0;

        for (int i = 0; i < abilityButtons.Length; i++)
        {
            // �����ִ� �����Ƽ �ʱ�ȭ
            abilityButtons[i].GetComponent<Ability>().isAppeared = false;
            current_weight = abilityButtons[i].GetComponent<Ability>().weight;
            // ����ġ �����ִ� �͵� ���� ����
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
            Ability_UI.SetActive(true);

            for (int j = 0; j < remainAbility; j++)
            {
                int rand = Random.Range(0, totalWeight);
                int weight = 0;

                for (int i = 0; i < abilityButtons.Length; i++)
                {
                    current_weight = abilityButtons[i].GetComponent<Ability>().weight;
                    if (current_weight == 0 || abilityButtons[i].GetComponent<Ability>().isAppeared)
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

                abilityButtons[rand].transform.position = buttonLocations[j].transform.position;
                abilityButtons[rand].SetActive(true);
                abilityButtons[rand].GetComponent<Ability>().isAppeared = true;
                totalWeight -= abilityButtons[rand].GetComponent<Ability>().weight;

            }
        }

    }
    public void GetAbility(Ability SelectedAbility)
    {
        int index = SelectedAbility.index;
        SelectedAbility.isSelected = true;
        SelectedAbility.isAppeared = false;
        SelectedAbility.level++;
        // ���õ� �ɷ� Ȯ��
        /*Debug.Log("Selected Ability : " + index + "th ability.");
        Debug.Log("Ability Type : " + SelectedAbility.Type);
        Debug.Log("Ability Tier : " + SelectedAbility.Tier);
        Debug.Log("Ability Level : " + SelectedAbility.level);
        Debug.Log("Ability Weight : " + SelectedAbility.weight);*/
        //*******************************************

        SelectedAbility.weight = 0;    //���õ� �ɷ� ���� Ȯ�� 0����
        float lv = (float)SelectedAbility.level;
        // Apply ability
        if (SelectedAbility.index == 0)
        {
            // power up z attack
            gameObject.GetComponent<PlayerAttack>().swordDamage_z_multiplier = 1f + 0.1f * lv;
        }
        else if (SelectedAbility.index == 1)
        {
            // power up x attack
            gameObject.GetComponent<PlayerAttack>().swordDamage_x_multiplier = 1f + 0.1f * lv;
        }
        else if (SelectedAbility.index == 2)
        {
            // speed up z attack
            gameObject.GetComponent<PlayerAttack>().Speed_Z = 1f + 0.2f * lv;
        }
        else if (SelectedAbility.index == 3)
        {
            // speed up z attack
            gameObject.GetComponent<PlayerAttack>().Speed_X = 1f + 0.2f * lv;
        }
        else if (SelectedAbility.index == 5)
        {
            // run speed upgrade
            gameObject.GetComponent<Player>().IncreaseRunSpeed();
        }

        /*
        if (SelectedAbility.Type == 0)
        {
            this.gameObject.GetComponent<PlayerAttack>().Speed_Z *= 1.5f;
        }*/
        // *************
        for (int i = 0; i < abilityButtons.Length; i++)
        {
            GameObject ability = abilityButtons[i];
            int type = ability.GetComponent<Ability>().Type;
            int tier = ability.GetComponent<Ability>().Tier;
            bool isSelected = ability.GetComponent<Ability>().isSelected;
            
            if (type == SelectedAbility.Type && !isSelected && lv==1)  // ���õ� �ɷ°� Ÿ���� ������ ���ù��� ���� �ɷ��� ��
            {
                ability.GetComponent<Ability>().weight += 1;
                if (tier == SelectedAbility.Tier + 1)
                {
                    ability.GetComponent<Ability>().weight += 1;
                }
            }
            //if (abilityButtons[i].gameObject.GetComponent<Ability>().appeared) abilityButtons[i].gameObject.GetComponent<Ability>().appeared = false;
            ability.SetActive(false);
        }
        
    }
    public void UpgradeAbility()
    {
        //totalWeight = 0;
        //remainAbility = 0;
        // ���õ� �����Ƽ �ε��� ����
        List<int> selected_list = new List<int>();

        for (int i = 0; i < abilityButtons.Length; i++)
        {
            // �����ִ� �����Ƽ �ʱ�ȭ
            abilityButtons[i].GetComponent<Ability>().isAppeared = false;
            // ���õ� �����Ƽ ���� ����
            if (abilityButtons[i].GetComponent<Ability>().isSelected)
            {
                //remainAbility++;
                selected_list.Add(i);
            }
        }


        if (selected_list.Count==0)
        {
            Debug.Log("There's no selected ability");
        }
        else
        {
            //
            Debug.Log("Selected ability is : " + selected_list.Count);
            //if (remainAbility > 3) remainAbility = 3;
            Ability_UI.SetActive(true);
            int howmany = selected_list.Count;
            if (howmany > 3) howmany = 3;
            Debug.Log(selected_list);
            for (int j = 0; j < howmany; j++)
            {
                int rand = Random.Range(0, selected_list.Count);
                int target_index = selected_list[rand];
/*
                for (int i = 0; i < abilityButtons.Length; i++)
                {
                    if (abilityButtons[i].GetComponent<Ability>().isSelected && abilityButtons[i].GetComponent<Ability>().isAppeared)
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
                }*/

                // ��ȭ�� �����Ƽ ���
                abilityButtons[target_index].transform.position = buttonLocations[j].transform.position;
                abilityButtons[target_index].SetActive(true);
                abilityButtons[target_index].GetComponent<Ability>().isAppeared = true;

                // selected_list���� ���� �� �����ϱ�
                selected_list.RemoveAt(rand);

            }
        }
    }










    /*
GameObject SelectedAbility = abilityButtons[index];
SelectedAbility.gameObject.GetComponent<Ability>().isSelected = true;

// ���õ� �ɷ� Ȯ��
Debug.Log("Selected Ability : " + index + "th ability.");
int a_type = SelectedAbility.gameObject.GetComponent<Ability>().Type;
int a_tier = SelectedAbility.gameObject.GetComponent<Ability>().Tier;
int a_level = SelectedAbility.gameObject.GetComponent<Ability>().level;
int a_weight = SelectedAbility.gameObject.GetComponent<Ability>().weight;
Debug.Log("Ability Type : " + a_type);
Debug.Log("Ability Tier : " + a_tier);
Debug.Log("Ability Level : " + a_level);
Debug.Log("Ability Weight : " + a_weight);
//*******************************************

SelectedAbility.gameObject.GetComponent<Ability>().weight = 0;    //���õ� �ɷ� ���� Ȯ�� 0����

// Apply ability

if (a_type == 0)
{
    this.gameObject.GetComponent<PlayerAttack>().Speed_Z *= 1.5f;
}
// *************

for(int i = 0; i < abilityButtons.Length; i++)
{
    GameObject ability = abilityButtons[i];
    int type = ability.gameObject.GetComponent<Ability>().Type;
    int tier = ability.gameObject.GetComponent<Ability>().Tier;
    bool isSelected = ability.gameObject.GetComponent<Ability>().isSelected;

    if (type == a_type && !isSelected)  // ���õ� �ɷ°� Ÿ���� ������ ���ù��� ���� �ɷ��� ��
    {
        ability.gameObject.GetComponent<Ability>().weight += 1;
        if (tier == a_tier + 1)
        {
            ability.gameObject.GetComponent<Ability>().weight += 1;
        }
    }
    //if (abilityButtons[i].gameObject.GetComponent<Ability>().appeared) abilityButtons[i].gameObject.GetComponent<Ability>().appeared = false;
    ability.gameObject.SetActive(false);
}*/
    /*
    void Trash()
    {
        
totalWeight = 0;
remainAbility = 0;


for (int i = 0; i < abilityBook.Length; i++)
{
    abilityBook[i].isAppeared = false;
    bool canAppear = canAppearThis(i);

    current_weight = baseWeight;
    if (canAppear)
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

    for (int j = 0; j < remainAbility; j++)
    {
        int rand = Random.Range(0, totalWeight);
        int weight = 0;

        for (int i = 0; i < abilityBook.Length; i++)
        {
            current_weight = baseWeight;
            bool canAppear = canAppearThis(i);
            if (!canAppear)
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

for(int i = 0; i < threeChoices.Length; i++)
{

    int rand = Random.Range(0, 100);
    if (rand >= 90)
    {
        color = new Color(1.0f, 0f, 0f, 1f);
        threeChoices[i].gameObject.GetComponent<Image>().color = color;
    }
    else
    {
        color = new Color(1.0f, 1.0f, 1.0f, 1f);
        threeChoices[i].gameObject.GetComponent<Image>().color = color;
    }


    //threeChoices[i].gameObject.SetActive(true);
    abilityBook[i].isAppeared = true;
    threeChoices[i].gameObject.GetComponent<CardDisplay>().SetCardDisplay(abilityBook[i]);
}

    }*/
    /*
    private bool canAppearThis(int index){
        bool canAppear = true;
        if (abilityBook[index].isSelected)  //�ɷ��� �̹� ���õ� ���
        {
            canAppear = false;
        }
        else if (abilityBook[index].isAppeared) //�ɷ��� ���õ����� �ʾ����� �̹� ȭ�鿡 ��Ÿ�� ���
        {
            canAppear = false;
        }
        else if (abilityBook[index].Tier == 2 && !canAppearTier2)
        {
            canAppear = false;
            //
        }
        else if (abilityBook[index].Tier == 3 && !canAppearTier3)
        {
            canAppear = false;
            //
        }
        return canAppear;
    }*/
}
