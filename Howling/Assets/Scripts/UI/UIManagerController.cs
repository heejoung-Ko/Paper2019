using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManagerController : MonoBehaviour
{
    // 플레이어 움직임 끄기 위해
    public GameObject PlayerController;

    // 창 끄고 켜기 위해
    public GameObject MakingUI;
    public GameObject MapUI;
    public GameObject MenuUI;

    enum UIState
    {
        NONE, MAKING, MAP, MENU
    };

    UIState state;

    // Start is called before the first frame update
    void Start()
    {
        state = UIState.NONE;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            // 조합 상태 일때
            if (state == UIState.MAKING)
            {
                exitMaking();
            }
            // 아무 상태도 아닐때
            else if(state == UIState.NONE)
            {
                enterMaking();
            }
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            // 지도 상태 일때
            if (state == UIState.MAP)
            {
                exitMap();
            }
            // 아무 상태도 아닐때
            else if (state == UIState.NONE)
            {
                enterMap();
            }
        }
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            if(state == UIState.MENU)
            {
                exitMenu();
            } else if(state == UIState.NONE)
            {
                enterMenu();
            }
        }

    }

    void enterMaking()
    {
        state = UIState.MAKING;

        enterUI();

        MakingUI.SetActive(true);
    }

    void exitMaking()
    {
        state = UIState.NONE;

        exitUI();

        MakingUI.GetComponent<Making>().UnsetRecipe();
        MakingUI.SetActive(false);
    }

    void enterMap()
    {
        state = UIState.MAP;

        MapUI.SetActive(true);
    }

    void exitMap()
    {
        state = UIState.NONE;

        MapUI.SetActive(false);
    }

    void enterMenu()
    {
        state = UIState.MENU;

        MenuUI.SetActive(true);
        Time.timeScale = 0f;
    }

    void exitMenu()
    {
        state = UIState.NONE;

        MenuUI.SetActive(false);
        Time.timeScale = 1f;
    }

    void enterUI()
    {
        PlayerController.GetComponent<Howling.PlayerMoveScript>().enabled = false;
        PlayerController.GetComponent<Howling.PlayerAtk>().enabled = false;
        PlayerController.GetComponentInChildren<Howling.PlayerHandController>().enabled = false;
    }

    void exitUI()
    {
        PlayerController.GetComponent<Howling.PlayerMoveScript>().enabled = true;
        PlayerController.GetComponent<Howling.PlayerAtk>().enabled = true;
        PlayerController.GetComponentInChildren<Howling.PlayerHandController>().enabled = true;
    }
}
