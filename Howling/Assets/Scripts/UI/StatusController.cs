using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StatusController : MonoBehaviour
{
    [SerializeField]
    private int hp;
    private int currentHp;

    [SerializeField]
    private int sp;
    private int currentSp;

    [SerializeField]
    private int spIncreaseSpeed;

    [SerializeField]
    private int spRechargeTime;
    private int currentSpRechargeTime;

    [SerializeField]
    private int spRecoverTime;
    private int currentSpRecoverTime;

    private bool isSpUsed;

    [SerializeField]
    private float hungry;
    private float currentHungry;

    [SerializeField]
    private int hungryDecreaseTime;
    private int currentHungryDecreaseTime;

    [SerializeField]
    private float thirsty;
    private float currentThirsty;

    [SerializeField]
    private int thirstyDecreaseTime;
    private int currentThirstyDecreaseTime;

    [SerializeField]
    private Image[] imgStatusGauge;

    private const int HP = 0, SP = 1, HUNGRY = 2, THIRSTY = 3;

    public bool isDie;
    EffectCameraController effectCameraController;
    HowlingSceneManager sceneManager;

    void Start()
    {
        StatusInitial();
        isDie = false;
        effectCameraController = FindObjectOfType<EffectCameraController>();
        sceneManager = FindObjectOfType<HowlingSceneManager>();
    }

    public void StatusInitial()
    {
        currentHp = hp;
        currentSp = sp;
        currentHungry = hungry;
        currentThirsty = thirsty;
    }

    public void ResetForRespawn()
    {
        StatusInitial();
        isDie = false;
        effectCameraController.DieCameraOff();
    }

    private void StatusCheats()
    {
        if (Input.GetKey(KeyCode.F1))
        {
            StatusInitial();
            //DieCheats();
        }
        else if (Input.GetKey(KeyCode.F2))
        {
            Die();
        }
    }

    void Update()
    {
        Hungry();
        Thirsty();
        MpRechargeTime();
        MpRecover();
        StatusCheats();
        GaugeUpdate();
    }

    public int GetHP()
    {
        return currentHp;
    }
    public void SetHP(int saveHp)
    {
        currentHp = saveHp;
    }

    public int GetSP()
    {
        return currentSp;
    }
    public void SetSP(int saveSp)
    {
        currentSp = saveSp;
    }

    public float GetHungry()
    {
        return currentHungry;
    }
    public void SetHungry(float saveHungry)
    {
        currentHungry = saveHungry;
    }

    public float GetThirsty()
    {
        return currentThirsty;
    }
    public void SetThirsty(float saveThirsty)
    {
        currentThirsty = saveThirsty;
    }

    private void Hungry()
    {
        if (currentHungry > 0)
        {
            if (currentHungryDecreaseTime <= hungryDecreaseTime)
                currentHungryDecreaseTime++;
            else
            {
                currentHungry -= Time.deltaTime * 60f;
                currentHungryDecreaseTime = 0;
            }
        }
        //else
        //{
        //    //Debug.Log("배고픔 수치가 0이 되었습니다");
        //}
    }

    private void Thirsty()
    {
        if (currentThirsty > 0)
        {
            if (currentThirstyDecreaseTime <= thirstyDecreaseTime)
                currentThirstyDecreaseTime++;
            else
            {
                currentThirsty -= Time.deltaTime * 40f;
                currentThirstyDecreaseTime = 0;
            }
        }
        //else
        //{
        //    Debug.Log("목마름 수치가 0이 되었습니다");
        //}
    }

    private void MpRechargeTime()
    {
        if (isSpUsed)
        {
            if (currentSpRechargeTime < spRechargeTime)
                currentSpRechargeTime++;
            else
                isSpUsed = false;
        }
    }

    private void MpRecover()
    {
        if (!isSpUsed && currentSp < sp)
        {
            if (currentSpRecoverTime < spRecoverTime)
                currentSpRecoverTime++;
            else
            {
                currentSp += spIncreaseSpeed;
                currentSpRecoverTime = 0;
            }
        }
    }

    private void GaugeUpdate()
    {
        imgStatusGauge[HP].fillAmount = (float)currentHp / hp;
        imgStatusGauge[SP].fillAmount = (float)currentSp / sp;
        imgStatusGauge[HUNGRY].fillAmount = currentHungry / hungry;
        imgStatusGauge[THIRSTY].fillAmount = currentThirsty / thirsty;
    }

    public void IncreaseHp(int cnt)
    {
        //Debug.Log("현재 HP: " + currentHp);
        if (currentHp + cnt < hp)
        {
            currentHp += cnt;
        }
        else
        {
            currentHp = hp;
            //Debug.Log(currentHp);
        }
    }

    public void DecreaseHp(int cnt)
    {
        currentHp -= cnt;

        if (currentHp <= 0)
        {
            currentHp = 0;
            Die();
        }
        //    Debug.Log("캐릭터의 hp가 0이 되었습니다!");
    }

    public void IncreaseMp(int cnt)
    {
        if (currentSp + cnt < sp)
            currentSp += cnt;
        else
            currentSp = sp;
    }

    public void DecreaseMp(int cnt)
    {
        isSpUsed = true;
        currentSpRechargeTime = 0;

        if (currentSp - cnt > 0)
            currentSp -= cnt;
        else
            currentSp = 0;
    }

    public void IncreaseHungry(int cnt)
    {
        if (currentHungry + cnt < hungry)
            currentHungry += cnt;
        else
            currentHungry = hungry;
    }

    public void DecreaseHungry(int cnt)
    {
        if (currentHungry - cnt < 0)
        {
            currentHungry = 0;
            Die();
        }
        else
            currentHungry -= cnt;
    }

    public void IncreaseThirsty(int cnt)
    {
        if (currentThirsty + cnt < thirsty)
            currentThirsty += cnt;
        else
            currentThirsty = thirsty;
    }

    public void DecreaseThirsty(int cnt)
    {
        if (currentThirsty - cnt < 0)
        {
            currentThirsty = 0;
            Die();
        }
        else
            currentThirsty -= cnt;
    }

    public int GetCurrentMp()
    {
        return currentSp;
    }

    public void HitEnemy(int enemATK)
    {
        DecreaseHp(enemATK);
        //Debug.Log("맞았당! HP: " + currentHp);
        
    }

    public void Die()
    {
        if (!isDie)
        {
            isDie = true;
            effectCameraController.DieCameraOn();
        }
    }

    //public void DieCheats()
    //{
    //    if (isDie)
    //    {
    //        isDie = false;
    //        effectCameraController.DieCameraOff();
    //    }
    //}
}
