using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class SaveData
{
    public Vector3 playerPos;
    public Vector3 playerRot;

    public List<int> invenArrayNumber = new List<int>();
    public List<string> invenItemName = new List<string>();
    public List<int> invenItemNumber = new List<int>();
}

public class SaveLoadController : MonoBehaviour
{
    private SaveData saveData = new SaveData();

    private string SAVE_DATA_DIRECTORY;
    private string SAVE_FILENAME = "/SaveFile.txt";

    [SerializeField]
    private GameObject myPlayer;
    private Howling.Inventory myInven;

    // Start is called before the first frame update
    void Start()
    {
        SAVE_DATA_DIRECTORY = Application.dataPath + "/Saves/";

        // 저장 디렉토리가 없으면 생성
        if(!Directory.Exists(SAVE_DATA_DIRECTORY)) {
            Directory.CreateDirectory(SAVE_DATA_DIRECTORY);
        }
    }
    
    public void SaveData()
    {
        myInven = FindObjectOfType<Howling.Inventory>();

        // 저장할 데이터

        saveData.playerPos = myPlayer.transform.position;
        saveData.playerRot = myPlayer.transform.eulerAngles;

        Howling.Slot[] slots = myInven.GetSlots();
        for (int i = 0; i < slots.Length; i++)
        {
            if(slots[i].item != null)
            {
                saveData.invenArrayNumber.Add(i);
                saveData.invenItemName.Add(slots[i].item.ItemName);
                saveData.invenItemNumber.Add(slots[i].itemCount);
            }

        }
        //

        string json = JsonUtility.ToJson(saveData);

        File.WriteAllText(SAVE_DATA_DIRECTORY + SAVE_FILENAME, json);

        Debug.Log("저장 완료 !");
        Debug.Log(json);
    }

    public void LoadData()
    {
        if(!File.Exists(SAVE_DATA_DIRECTORY + SAVE_FILENAME))
        {
            Debug.Log("세이브 파일이 없다 !");
        } else
        {
            string loadJson = File.ReadAllText(SAVE_DATA_DIRECTORY + SAVE_FILENAME);
            saveData = JsonUtility.FromJson<SaveData>(loadJson);

            myInven = FindObjectOfType<Howling.Inventory>();

            // 로드할 데이터

            myPlayer.transform.position = saveData.playerPos;
            myPlayer.transform.eulerAngles = saveData.playerRot;

            for (int i = 0; i < saveData.invenItemName.Count; i++)
            {
                myInven.LoadToInven(saveData.invenArrayNumber[i], saveData.invenItemName[i], saveData.invenItemNumber[i]);
            }

            //
            Debug.Log("로드 완료 !");
        }
    }
}
