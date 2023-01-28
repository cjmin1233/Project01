using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarterManager : MonoBehaviour
{
    public static StarterManager Instance { get; private set; }

    [SerializeField] private GameObject[] players;
    [SerializeField] private GameObject startCamera;
    [SerializeField] private GameObject PlayerAfterImagePool;
    [SerializeField] private GameObject SwordWindPool;
    [SerializeField] private GameObject ArrowPool;
    [SerializeField] private GameObject ArrowShowerPool;

    private void Awake()
    {
        Instance = this;

        int weaponType = PlayerPrefs.GetInt("weaponType");
        Instantiate(players[weaponType-1]);

        Instantiate(startCamera);
        Instantiate(PlayerAfterImagePool);
        if (weaponType == 1)
        {
            Instantiate(SwordWindPool);
        }
        else if (weaponType == 2)
        {
            Instantiate(ArrowPool);
            Instantiate(ArrowShowerPool);
        }

    }
}
