using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WolfReward : MonoBehaviour
{
    void Start()
    {
        
    }

    public void SetPenalty(float status, int state)
    {

    }

    public float GetReward(float status)
    {
        return 1 - (status * 0.01f);
    }
}
