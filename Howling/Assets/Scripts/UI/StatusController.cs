using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusController : MonoBehaviour
{
    [SerializeField]
    private int hp;
    private int currentHp;

    [SerializeField]
    private int mp;
    private int currentMp;

    [SerializeField]
    private int mpIncreaseSpeed;

    [SerializeField]
    private int mpRechargeTime;
    private int currentMpRechargeTime;

    [SerializeField]
    private int mpRecoverTime;
    private int currentMpRecoverTime;

    private bool isMpUsed;

    [SerializeField]
    private int hungry;
    private int currentHungry;

    [SerializeField]
    private int hungryDecreaseTime;
    private int currentHungryDecreaseTime;

    [SerializeField]
    private int thirsty;
    private int currentThirsty;

    [SerializeField]
    private int thirstyDecreaseTime;
    private int currentThirstyDecreaseTime;

    [SerializeField]
    private Image[] imgStatusGauge;

    private const int HP = 0, MP = 1, HUNGRY = 2, THIRSTY = 3;

    private float invincibleTime = 1f; // 무적 시간
    private float currentInvincibleTime = 0f;

    // Use this for initialization
    void Start()
    {
        currentHp = hp;
        currentMp = mp;
        currentHungry = hungry;
        currentThirsty = thirsty;
    }

    // Update is called once per frame
    void Update()
    {
        Hungry();
        Thirsty();
        MpRechargeTime();
        MpRecover();
        GaugeUpdate();

        if (currentInvincibleTime <= invincibleTime)
        {
            currentInvincibleTime += Time.deltaTime;
        }
    }

    private void Hungry()
    {
        if (currentHungry > 0)
        {
            if (currentHungryDecreaseTime <= hungryDecreaseTime)
                currentHungryDecreaseTime++;
            else
            {
                currentHungry--;
                currentHungryDecreaseTime = 0;
            }
        }
        else
        {
            Debug.Log("배고픔 수치가 0이 되었습니다");
        }
    }

    private void Thirsty()
    {
        if (currentThirsty > 0)
        {
            if (currentThirstyDecreaseTime <= thirstyDecreaseTime)
                currentThirstyDecreaseTime++;
            else
            {
                currentThirsty--;
                currentThirstyDecreaseTime = 0;
            }
        }
        else
        {
            Debug.Log("목마름 수치가 0이 되었습니다");
        }
    }

    private void MpRechargeTime()
    {
        if (isMpUsed)
        {
            if (currentMpRechargeTime < mpRechargeTime)
                currentMpRechargeTime++;
            else
                isMpUsed = false;
        }
    }

    private void MpRecover()
    {
        if (!isMpUsed && currentMp < mp)
        {
            if (currentMpRecoverTime < mpRecoverTime)
                currentMpRecoverTime++;
            else
            {
                currentMp += mpIncreaseSpeed;
                currentMpRecoverTime = 0;
            }
        }
    }

    private void GaugeUpdate()
    {
        imgStatusGauge[HP].fillAmount = (float)currentHp / hp;
        imgStatusGauge[MP].fillAmount = (float)currentMp / mp;
        imgStatusGauge[HUNGRY].fillAmount = (float)currentHungry / hungry;
        imgStatusGauge[THIRSTY].fillAmount = (float)currentThirsty / thirsty;
    }

    public void IncreaseHp(int cnt)
    {
        if (currentHp + cnt < hp)
            currentHp += cnt;
        else
            currentHp = hp;
    }

    public void DecreaseHp(int cnt)
    {
        currentHp -= cnt;

        if (currentHp <= 0)
            Debug.Log("캐릭터의 hp가 0이 되었습니다!");
    }

    public void IncreaseMp(int cnt)
    {
        if (currentMp + cnt < mp)
            currentMp += cnt;
        else
            currentMp = mp;
    }

    public void DecreaseMp(int cnt)
    {
        isMpUsed = true;
        currentMpRechargeTime = 0;

        if (currentMp - cnt > 0)
            currentMp -= cnt;
        else
            currentMp = 0;
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
            currentHungry = 0;
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
            currentThirsty = 0;
        else
            currentThirsty -= cnt;
    }

    public int GetCurrentMp()
    {
        return currentMp;
    }

    public void HitEnemy(int enemATK)
    {
        if (currentInvincibleTime > invincibleTime)
        {
            DecreaseHp(enemATK);
            currentInvincibleTime = 0f;
            Debug.Log("맞았당! HP: " + hp);
        }
    }
}
