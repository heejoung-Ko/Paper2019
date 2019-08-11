using Howling;
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
    public GameObject BoxUI;
    public GameObject MenuUI;

    bool isBox = false;

    enum UIState
    {
        NONE, MAKING, MAP, MENU, BOX
    };

    [SerializeField]

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
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isBox)
                exitBox();
        }
        if (state == UIState.BOX && isBox == false)
        {
            isBox = true;
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

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        MenuUI.SetActive(true);
        Time.timeScale = 0f;
    }

    void exitMenu()
    {
        state = UIState.NONE;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        MenuUI.SetActive(false);
        Time.timeScale = 1f;
    }

    public void enterBox()
    {
        state = UIState.BOX;

        enterUI();

        BoxUI.SetActive(true);
        //BoxUI.GetComponent<BoxController>().BoxStart();

        Time.timeScale = 0f;
    }

    void exitBox()
    {
        isBox = false;

        state = UIState.NONE;

        exitUI();

        //BoxUI.GetComponent<BoxController>().BoxEnd();

        BoxUI.SetActive(false);

        Time.timeScale = 1f;
    }

    void enterUI()
    {
        PlayerController.GetComponent<PlayerMoveScript>().enabled = false;
        PlayerController.GetComponent<PlayerAtk>().enabled = false;
        PlayerController.GetComponentInChildren<PlayerHandController>().enabled = false;
        PlayerController.GetComponentInChildren<ActionController>().enabled = false;
    }

    void exitUI()
    {
        PlayerController.GetComponent<PlayerMoveScript>().enabled = true;
        PlayerController.GetComponent<PlayerAtk>().enabled = true;
        PlayerController.GetComponentInChildren<PlayerHandController>().enabled = true;
        PlayerController.GetComponentInChildren<ActionController>().enabled = true;
    }
}
