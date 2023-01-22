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
    //public TestAbility[] abilityBook;   // 어빌리티 저장
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
            // 남아있는 어빌리티 초기화
            abilityButtons[i].GetComponent<Ability>().isAppeared = false;
            current_weight = abilityButtons[i].GetComponent<Ability>().weight;
            // 가중치 남아있는 것들 개수 세기
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
        // 선택된 능력 확인
        /*Debug.Log("Selected Ability : " + index + "th ability.");
        Debug.Log("Ability Type : " + SelectedAbility.Type);
        Debug.Log("Ability Tier : " + SelectedAbility.Tier);
        Debug.Log("Ability Level : " + SelectedAbility.level);
        Debug.Log("Ability Weight : " + SelectedAbility.weight);*/
        //*******************************************

        SelectedAbility.weight = 0;    //선택된 능력 출현 확률 0으로
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
        else if (SelectedAbility.index == 6)
        {
            // 질풍 베기
            gameObject.GetComponent<PlayerAttack>().sword_wind_enable = true;
        }
        else if (SelectedAbility.index == 7)
        {
            // 폭풍 베기
            gameObject.GetComponent<PlayerAttack>().sword_storm_enable = true;
        }
        else if (SelectedAbility.index == 8)
        {
            // 흡혈
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
            
            if (type == SelectedAbility.Type && !isSelected && lv==1)  // 선택된 능력과 타입은 같으나 선택받지 못한 능력일 때
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
        // 선택된 어빌리티 인덱스 모음
        List<int> selected_list = new List<int>();

        for (int i = 0; i < abilityButtons.Length; i++)
        {
            // 남아있는 어빌리티 초기화
            abilityButtons[i].GetComponent<Ability>().isAppeared = false;
            // 선택된 어빌리티 갯수 세기
            if (abilityButtons[i].GetComponent<Ability>().isSelected)
            {
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
            Ability_UI.SetActive(true);
            int howmany = selected_list.Count;
            if (howmany > 3) howmany = 3;
            

            for (int j = 0; j < howmany; j++)
            {
                int rand = Random.Range(0, selected_list.Count);
                int target_index = selected_list[rand];

                // 강화할 어빌리티 출력
                abilityButtons[target_index].transform.position = buttonLocations[j].transform.position;
                abilityButtons[target_index].SetActive(true);
                abilityButtons[target_index].GetComponent<Ability>().isAppeared = true;

                // selected_list에서 뽑힌 것 제외하기
                selected_list.RemoveAt(rand);

            }
        }
    }
}
