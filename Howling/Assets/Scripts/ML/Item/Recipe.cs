using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakingMaterial
{
    // 재료 이름
    public Item ItemName;
    // 재료 갯수
    public int num;
}

[CreateAssetMenu(fileName = "New Recipe", menuName = "New Recipe/Recipe")]
public class Recipe : ScriptableObject
{
    public Item Result;            // 조합 결과 아이템
    public string Explanation;     // 아이템 설명

    // 필요한 재료 1 ~ 3, 앞 번호부터 채우기 
    public Item Material1;
    public int Material1_num;
    public Item Material2;
    public int Material2_num;
    public Item Material3;
    public int Material3_num;
}
