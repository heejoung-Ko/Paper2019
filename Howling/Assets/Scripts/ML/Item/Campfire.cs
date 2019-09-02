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
    private AudioSource _audioSource;

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
        if(!_audioSource.isPlaying)
        {
            _audioSource.Play();
        }

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
            if(_audioSource.isPlaying)
            {
                _audioSource.Stop();
            }

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
        _audioSource.volume = 0.1f;

        fireSize = FireSizeType.SMALL;
        small.SetActive(true);
        midium.SetActive(false);
        big.SetActive(false);
    }
    
    private void SetMidium()
    {
        _audioSource.volume = 0.3f;

        fireSize = FireSizeType.MIDIUM;
        small.SetActive(false);
        midium.SetActive(true);
        big.SetActive(false);
    }
    
    private void SetBig()
    {
        _audioSource.volume = 0.5f;

        fireSize = FireSizeType.BIG;
        small.SetActive(false);
        midium.SetActive(false);
        big.SetActive(true);
    }
}
