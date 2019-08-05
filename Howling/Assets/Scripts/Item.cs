using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "New Item/Item")]
public class Item : ScriptableObject    
{
    public string ItemName;         // 아이템 이름
    public Sprite ItemImage;        // 아이템 이미지
    public GameObject ItemPrefab;   // 아이템 프리펩

    public int weaponType;       // 무기 유형

    public ItemType itemType;       
    public enum ItemType            // 아이템 유형
    {
        Equipment,                  // 장비
        Used,                       // 소모품
        Ingredient,                 // 재료
        ETC                         // 기타
    }

}
