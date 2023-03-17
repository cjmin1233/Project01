using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataManager : MonoBehaviour
{
    static GameObject container;

    static DataManager instance;

    public static DataManager Instance
    {
        get
        {
            if (!instance)
            {
                container = new GameObject();
                container.name = "DataManager";
                instance = container.AddComponent(typeof(DataManager)) as DataManager;
                DontDestroyOnLoad(container);
            }
            return instance;
        }
    }

    string GameDataFileName = "GameData.json";

    public Data data = new Data();
    public void LoadGameData()
    {
        string filePath = Application.persistentDataPath + "/" + GameDataFileName;

        if (File.Exists(filePath))
        {
            string FromJsonData = File.ReadAllText(filePath);
            data = JsonUtility.FromJson<Data>(FromJsonData);
            /*for(int i = 0; i < data.test_arr.Length; i++)
            {
                print($"{i}번 test_arr : " + data.test_arr[i]);
            }*/
            /*print("curHP:" + data.curHP);
            print("maxHP:" + data.maxHP);
            print("gold:" + data.gold);*/
            print("불러오기 완료");
        }
    }
    public void SaveGameData()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        // 플레이어 위치, 체력, 최대체력, 골드 저장
        data.position[0] = player.transform.position.x;
        data.position[1] = player.transform.position.y;
        data.position[2] = player.transform.position.z;
        data.curHP = player.GetComponent<Player>().CurHP;
        data.maxHP = player.GetComponent<Player>().MaxHP;
        data.gold = player.GetComponent<Player>().CheckGold();
        data.sceneNumber = SceneManager.GetActiveScene().buildIndex;
        data.stageCleared = GameManager.Instance.stageCleared;


        string ToJsonData = JsonUtility.ToJson(data, true);
        string filePath = Application.persistentDataPath + "/" + GameDataFileName;

        File.WriteAllText(filePath, ToJsonData);

        print("저장 완료");
    }
}
