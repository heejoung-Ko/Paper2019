using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HowlingSceneManager : MonoBehaviour
{
    public string sceneName = "HowlingScene";

    public void ClickStart()
    {
        Debug.Log("게임 로딩중 ... ..");
        SceneManager.LoadScene(sceneName);

    }

    public void ClickLoad()
    {
        Debug.Log("게임 로드 중 . .. ..!");
    }

    public void ClickExit()
    {
        Debug.Log("게임 종료 !");
        Application.Quit();
    }
}
