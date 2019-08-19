using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeaponAtk
{
    public Item item;
    [Tooltip("HP, MP, HUNGRY, THIRSTY만 가능합니다.")]
    public string[] type;
    public int[] num;
}

public class WeaponAtkDB : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
