using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    public GameObject BaseUI;
    [SerializeField] private SaveLoadController SaveLoad;
    public void ClickSave()
    {
        Debug.Log("세이브");
        SaveLoad.SaveData();
    }

    public void ClickLoad()
    {
        Debug.Log("로드");
        SaveLoad.LoadData();
    }

    public void ClickExit()
    {
        Debug.Log("종료~~");
        Application.Quit();
    }
 
}
