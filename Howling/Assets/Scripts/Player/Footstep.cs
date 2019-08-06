using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footstep : MonoBehaviour
{
    public string sand_sound;
    public string grass_sound;

    void OnTriggerEnter(Collider _col)
    {
        Debug.Log("바닥에 닿는다");
        if (_col.gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            SoundManager.instance.PlaySE(grass_sound);
        }

        //else if (_col.gameObject.layer == LayerMask.NameToLayer("sand"))
        //{
        //    SoundManager.instance.PlaySE(sand_sound);
        //}
    }
}
