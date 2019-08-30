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
    public GameObject SupplyBoxUI;
    //public BoxSlotManager BoxSlotM;
    public GameObject MenuUI;

    bool isBox = false;
    bool isSupply = false;

    [HideInInspector] public bool isGameOver;

    enum UIState
    {
        NONE, MAKING, MAP, MENU, BOX, SUPPLY
    };

    [SerializeField]

    UIState state;

    void Start()
    {
        //BoxSlotM.GetComponent<BoxSlotManager>();
        state = UIState.NONE;
        isGameOver = false;
    }

    void Update()
    {
        if (isGameOver) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            switch(state)
            {
                case UIState.MAKING:
                    exitMaking();
                    break;
                case UIState.MAP:
                    exitMap();
                    break;
                case UIState.BOX:
                    exitBox();
                    break;
                case UIState.MENU:
                    exitMenu();
                    break;
                case UIState.SUPPLY:
                    exitSupplyBox();
                    break;
            }
            return;
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            // 조합 상태 일때
            if (state == UIState.MAKING)
            {
                exitMaking();
            }
            // 아무 상태도 아닐때
            else if (state == UIState.NONE)
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
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isSupply)
                exitSupplyBox();
        }
        if (state == UIState.SUPPLY && isSupply == false)
        {
            isSupply = true;
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (state == UIState.MENU)
            {
                exitMenu();
            }
            else if (state == UIState.NONE)
            {
                enterMenu();
            }
        }
    }

    void enterMaking()
    {
        state = UIState.MAKING;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        enterUI();

        MakingUI.SetActive(true);
    }

    void exitMaking()
    {
        state = UIState.NONE;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

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
        enterUI();

        Time.timeScale = 0f;    // 시간 정지
    }

    void exitMenu()
    {
        state = UIState.NONE;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        MenuUI.SetActive(false);
        exitUI();

        Time.timeScale = 1f;
    }

    public void enterBox()
    {
        if (isGameOver) return;

        state = UIState.BOX;

        enterUI();

        BoxUI.SetActive(true);

        // Box Slot Manager -> Box Controller
        //BoxUI.boxControllerSlots_line1 = BoxSlotM.boxSlots_line1;
        //BoxUI.boxControllerSlots_line2 = BoxSlotM.boxSlots_line2;
        //BoxUI.boxControllerSlots_line3 = BoxSlotM.boxSlots_line3;
        //BoxUI.boxControllerSlots_line4 = BoxSlotM.boxSlots_line4;

        //BoxUI.GetComponent<BoxController>().BoxStart();

        Time.timeScale = 0f;
    }

    void exitBox()
    {
        isBox = false;

        state = UIState.NONE;

        exitUI();

        // Box Controller -> Box Slot Manager
        //BoxSlotM.boxSlots_line1 = BoxUI.boxControllerSlots_line1;
        //BoxSlotM.boxSlots_line2 = BoxUI.boxControllerSlots_line2;
        //BoxSlotM.boxSlots_line3 = BoxUI.boxControllerSlots_line3;
        //BoxSlotM.boxSlots_line4 = BoxUI.boxControllerSlots_line4;

        BoxUI.SetActive(false);

        Time.timeScale = 1f;
    }

    public void enterSupplyBox(GameObject go)
    {
        if (isGameOver) return;

        state = UIState.SUPPLY;

        enterUI();

        SupplyBoxUI.SetActive(true);
        

        Time.timeScale = 0f;
    }

    void exitSupplyBox()
    {
        isSupply = false;

        state = UIState.NONE;

        exitUI();

        SupplyBoxUI.SetActive(false);

        Time.timeScale = 1f;
    }


    public void enterUI()
    {
        PlayerController.GetComponent<PlayerMoveScript>().enabled = false;
        PlayerController.GetComponent<PlayerAtk>().enabled = false;
        PlayerController.GetComponentInChildren<PlayerHandController>().enabled = false;
        PlayerController.GetComponentInChildren<ActionController>().enabled = false;
    }

    public void exitUI()
    {
        PlayerController.GetComponent<PlayerMoveScript>().enabled = true;
        PlayerController.GetComponent<PlayerAtk>().enabled = true;
        PlayerController.GetComponentInChildren<PlayerHandController>().enabled = true;
        PlayerController.GetComponentInChildren<ActionController>().enabled = true;
    }
}
