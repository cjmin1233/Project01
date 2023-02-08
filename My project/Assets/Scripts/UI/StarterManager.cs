using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarterManager : MonoBehaviour
{
    public static StarterManager Instance { get; private set; }

    [SerializeField] private GameObject[] players;
    [SerializeField] private GameObject startCamera;
    [SerializeField] private GameObject PlayerAfterImagePool;
    [SerializeField] private GameObject DamageTextPool;
    [SerializeField] private GameObject SwordWindPool;
    [SerializeField] private GameObject ArrowPool;
    [SerializeField] private GameObject ArrowShowerPool;

    private void Awake()
    {
        Instance = this;
    }
    private void OnEnable()
    {
        DataManager.Instance.LoadGameData();

        int weaponType = PlayerPrefs.GetInt("weaponType");
        //if (weaponType > 0) Instantiate(players[weaponType - 1]);

        if (weaponType > 0)
        {
            var player = Instantiate(players[weaponType - 1]);
            GameManager.Instance.AddToList(player);
            DataManager.Instance.data.weaponType = weaponType;
            DataManager.Instance.SaveGameData();
        }

        /*Instantiate(startCamera);
        Instantiate(PlayerAfterImagePool);
        if (weaponType == 1)
        {
            Instantiate(SwordWindPool);
        }
        else if (weaponType == 2)
        {
            Instantiate(ArrowPool);
            Instantiate(ArrowShowerPool);
        }*/
        var cam = Instantiate(startCamera);
        GameManager.Instance.AddToList(cam);
        var obj = Instantiate(PlayerAfterImagePool);
        GameManager.Instance.AddToList(obj);
        obj = Instantiate(DamageTextPool);
        GameManager.Instance.AddToList(obj);
        /*        GameManager.Instance.objects.Add(Instantiate(startCamera));
                GameManager.Instance.objects.Add(Instantiate(PlayerAfterImagePool));
        */

        if (weaponType == 1)
        {
            var wind_pool = Instantiate(SwordWindPool);
            GameManager.Instance.AddToList(wind_pool);

            //GameManager.Instance.objects.Add(Instantiate(SwordWindPool));
        }
        else if (weaponType == 2)
        {
            var arrow_pool = Instantiate(ArrowPool);
            GameManager.Instance.AddToList(arrow_pool);
            var shower_pool = Instantiate(ArrowShowerPool);
            GameManager.Instance.AddToList(shower_pool);

            /*GameManager.Instance.objects.Add(Instantiate(ArrowPool));
            GameManager.Instance.objects.Add(Instantiate(ArrowShowerPool));*/
        }

    }
}
