using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void SetFireSize()
    {
        if(state == 0)
        {
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
        small.SetActive(false);
        midium.SetActive(false);
        big.SetActive(false);
    }

    private void SetSmall()
    {
        small.SetActive(true);
        midium.SetActive(false);
        big.SetActive(false);
    }
    
    private void SetMidium()
    {
        small.SetActive(false);
        midium.SetActive(true);
        big.SetActive(false);
    }
    
    private void SetBig()
    {
        small.SetActive(false);
        midium.SetActive(false);
        big.SetActive(true);
    }
}
