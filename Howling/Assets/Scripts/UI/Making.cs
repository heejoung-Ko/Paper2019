using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Making : MonoBehaviour
{
    // 플레이어 움직임 끄기 위해
    public GameObject PlayerController;

    // 조합창 끄고 켜기 위해
    public GameObject MakingUI;

    // 조합 상태인지 아닌지 체크
    bool isMaking;

    // Start is called before the first frame update
    void Start()
    {
        isMaking = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            // 조합 상태 일때
            if (isMaking)
            {
                exitMaking();
            }
            else
            {
                enterMaking();
            }
        }
    }

    void enterMaking()
    {
        isMaking = true;
        PlayerController.GetComponent<Howling.PlayerMoveScript>().enabled = false;
        PlayerController.GetComponent<Howling.PlayerAtk>().enabled = false;
        PlayerController.GetComponentInChildren<Howling.PlayerHandController>().enabled = false;
        MakingUI.SetActive(true);
    }

    void exitMaking()
    {
        isMaking = false;
        PlayerController.GetComponent<Howling.PlayerMoveScript>().enabled = true;
        PlayerController.GetComponent<Howling.PlayerAtk>().enabled = true;
        PlayerController.GetComponentInChildren<Howling.PlayerHandController>().enabled = true;
        MakingUI.SetActive(false);
    }
}
