using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FireSizeType
{
    NONE, SMALL, MIDIUM, BIG
}

public class Campfire : MonoBehaviour
{
    [SerializeField]
    float state = 0f;

    [SerializeField]
    GameObject small;

    [SerializeField]
    GameObject midium;

    [SerializeField]
    GameObject big;

    bool isFire = false;
    public FireSizeType fireSize;

    public void Update()
    {
        state -= Time.deltaTime;

        if (state < 0)
            state = 0;

        SetFireSize();
    }

    public void InputWood()
    {
        state += 30f;

        if (state > 100)
            state = 100;

        SetFireSize();
    }

    public bool GetIsFire()
    {
        return isFire;
    }

    private void SetFireSize()
    {
        isFire = true;
        if (state == 0)
        {
            isFire = false;
            SetNone();
        }
        else if(state <= 30f)
        {
            SetSmall();
        }
        else if(state <= 60f)
        {
            SetMidium();
        }
        else
        {
            SetBig();
        }
    }

    private void SetNone()
    {
        fireSize = FireSizeType.NONE;
        small.SetActive(false);
        midium.SetActive(false);
        big.SetActive(false);
    }

    private void SetSmall()
    {
        fireSize = FireSizeType.SMALL;
        small.SetActive(true);
        midium.SetActive(false);
        big.SetActive(false);
    }
    
    private void SetMidium()
    {
        fireSize = FireSizeType.MIDIUM;
        small.SetActive(false);
        midium.SetActive(true);
        big.SetActive(false);
    }
    
    private void SetBig()
    {
        fireSize = FireSizeType.BIG;
        small.SetActive(false);
        midium.SetActive(false);
        big.SetActive(true);
    }
}
