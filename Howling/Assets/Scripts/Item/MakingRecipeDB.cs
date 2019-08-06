using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakingMaterial
{
    // 재료 이름
    public string ItemName;
   // 재료 갯수
    int num;
}

[System.Serializable]
public class MakingRecipe
{
    public string ItemName;

    // 필요한 재료 목록
    public MakingMaterial[] materials;
}

public class MakingRecipeDB : MonoBehaviour
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
