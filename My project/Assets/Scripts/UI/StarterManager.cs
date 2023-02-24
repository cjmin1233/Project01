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
        Data gameData = DataManager.Instance.data;
        int weaponType;
        bool newGame = true;
        if (gameData.weaponType > 0)
        {
            // ���� �����Ͱ� �����ϴ� ���
            weaponType = gameData.weaponType;
            PlayerPrefs.SetInt("weaponType", weaponType);
            newGame = false;
        }
        // ���� ���� �����Ͱ� ���ų� ���ο� ������ ���
        else weaponType = PlayerPrefs.GetInt("weaponType");

        //int weaponType = PlayerPrefs.GetInt("weaponType");
        //if (weaponType > 0) Instantiate(players[weaponType - 1]);

        var player = Instantiate(players[weaponType - 1]);
        GameManager.Instance.AddToList(player);
        if (!newGame)
        {
            // ���� �����Ͱ� �����ϴ� ��� ���� ��ġ��ǥ�� �÷��̾� ����.
            Vector3 pos = new Vector3();
            pos.x = gameData.position[0];
            pos.y = gameData.position[1];
            pos.z = gameData.position[2];
            player.transform.position = pos;
        }
        DataManager.Instance.SaveGameData();

        var cam = Instantiate(startCamera);
        GameManager.Instance.AddToList(cam);
        var obj = Instantiate(PlayerAfterImagePool);
        GameManager.Instance.AddToList(obj);
        obj = Instantiate(DamageTextPool);
        GameManager.Instance.AddToList(obj);

        if (weaponType == 1)
        {
            var wind_pool = Instantiate(SwordWindPool);
            GameManager.Instance.AddToList(wind_pool);
        }
        else if (weaponType == 2)
        {
            var arrow_pool = Instantiate(ArrowPool);
            GameManager.Instance.AddToList(arrow_pool);
            var shower_pool = Instantiate(ArrowShowerPool);
            GameManager.Instance.AddToList(shower_pool);
        }

    }
}
