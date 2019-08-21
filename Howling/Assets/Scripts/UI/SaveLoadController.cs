using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class SaveData
{
    // 시스템
    public float gameTime;
    public int gameDay;

    // 플레이어
    public Vector3 playerPos;
    public Vector3 playerRot;
    public int playerHp;
    public int playerSp;
    public float playerHungry;
    public float playerThirsty;

    // 아이템 - 인벤토리
    public List<int> invenArrayNumber = new List<int>();
    public List<string> invenItemName = new List<string>();
    public List<int> invenItemNumber = new List<int>();
    //// 아이템 - 상자
    //public List<int> boxArrayNumber1 = new List<int>();
    //public List<string> boxItemName1 = new List<string>();
    //public List<int> boxItemNumber1 = new List<int>();
    //public List<int> boxArrayNumber2 = new List<int>();
    //public List<string> boxItemName2 = new List<string>();
    //public List<int> boxItemNumber2 = new List<int>();
    //public List<int> boxArrayNumber3 = new List<int>();
    //public List<string> boxItemName3 = new List<string>();
    //public List<int> boxItemNumber3 = new List<int>();
    //public List<int> boxArrayNumber4 = new List<int>();
    //public List<string> boxItemName4 = new List<string>();
    //public List<int> boxItemNumber4 = new List<int>();


    //// 늑대
    public Vector3 wolfPos;
    public Vector3 wolfRot;
    public float wolfHp;
    public float wolfHungry;
    public float wolfFriendly;
}

public class SaveLoadController : MonoBehaviour
{
    private SaveData saveData = new SaveData();

    private string SAVE_DATA_DIRECTORY;
    private string SAVE_FILENAME = "/SaveFile.txt";

    [SerializeField] private DayNightCycle gameClock;
    [SerializeField] private GameObject myPlayer;
    [SerializeField] private StatusController myPlayerStatus;
    private Howling.Inventory myInven;
    //private BoxSlotManager myBox;
    [SerializeField] private WolfAgent myWolf;

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
        //myBox = FindObjectOfType<BoxSlotManager>();

        // 저장할 데이터
        saveData.gameTime = gameClock.GetTime();
        saveData.gameDay = gameClock.day;

        saveData.playerPos = myPlayer.transform.position;
        saveData.playerRot = myPlayer.transform.eulerAngles;
        saveData.playerHp = myPlayerStatus.GetHP();
        saveData.playerSp = myPlayerStatus.GetSP();
        saveData.playerHungry = myPlayerStatus.GetHungry();
        saveData.playerThirsty = myPlayerStatus.GetThirsty();
        

        Howling.Slot[] invenSlots = myInven.GetInvenSlots();
        for (int i = 0; i < invenSlots.Length; i++)
        {
            if (invenSlots[i].item != null)
            {
                saveData.invenArrayNumber.Add(i);
                saveData.invenItemName.Add(invenSlots[i].item.ItemName);
                saveData.invenItemNumber.Add(invenSlots[i].itemCount);
            }
        }

        saveData.wolfPos = myWolf.transform.position;
        saveData.wolfRot = myWolf.transform.eulerAngles;
        saveData.wolfHp = myWolf.Hp;
        saveData.wolfHungry = myWolf.Hungry;
        saveData.wolfFriendly = myWolf.Friendly;

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
            gameClock.SetTime(saveData.gameTime, saveData.gameDay);

            myPlayer.transform.position = saveData.playerPos;
            myPlayer.transform.eulerAngles = saveData.playerRot;
            myPlayerStatus.SetHP(saveData.playerHp);
            myPlayerStatus.SetSP(saveData.playerHp);
            myPlayerStatus.SetHungry(saveData.playerHungry);
            myPlayerStatus.SetThirsty(saveData.playerThirsty);


            for (int i = 0; i < saveData.invenItemName.Count; ++i)
            {
                myInven.LoadToInven(saveData.invenArrayNumber[i], saveData.invenItemName[i], saveData.invenItemNumber[i]);
            }

            myWolf.transform.position = saveData.wolfPos;
            myWolf.transform.eulerAngles = saveData.wolfRot;
            myWolf.Hp = saveData.wolfHp;
            myWolf.Hungry = saveData.wolfHungry;
            myWolf.Friendly = saveData.wolfFriendly;
            //
            Debug.Log("로드 완료 !");
        }
    }
}
