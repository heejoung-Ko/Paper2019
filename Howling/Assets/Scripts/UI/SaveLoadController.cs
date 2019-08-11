using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class SaveData
{
    public Vector3 playerPos;

}

public class SaveLoadController : MonoBehaviour
{
    private SaveData saveData = new SaveData();

    private string SAVE_DATA_DIRECTORY;
    private string SAVE_FILENAME = "/SaveFile.txt";

    private PlayerController myPlayer;

    // Start is called before the first frame update
    void Start()
    {
        SAVE_DATA_DIRECTORY = Application.dataPath + "/Saves/";

        // 저장 디렉토리가 없으면 생성
        if(!Directory.Exists(SAVE_DATA_DIRECTORY) {
            Directory.CreateDirectory(SAVE_DATA_DIRECTORY);
        }
    }
    
    public void SaveData()
    {
        myPlayer = FindObjectOfType<PlayerController>();

        saveData.playerPos = myPlayer.transform.position;

        string json = JsonUtility.ToJson(saveData);

        File.WriteAllText(SAVE_DATA_DIRECTORY + SAVE_FILENAME, json);

        Debug.Log("저장 완료 !");
        Debug.Log(json);
    }
}
