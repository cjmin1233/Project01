using System.IO;
using UnityEngine;

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
            print("불러오기 완료");
        }
    }
    public void SaveGameData()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        data.position[0] = player.transform.position.x;
        data.position[1] = player.transform.position.y;
        data.position[2] = player.transform.position.z;

        string ToJsonData = JsonUtility.ToJson(data, true);
        string filePath = Application.persistentDataPath + "/" + GameDataFileName;

        File.WriteAllText(filePath, ToJsonData);

        print("저장 완료");
        for(int i = 0; i < data.ability.Length; i++)
        {
            print($"{i}번 어빌리티 잠금 해제 여부 : " + data.ability[i]);
        }
    }
}
