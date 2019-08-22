using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemEffect
{
    public Item item;
    [Tooltip("HP, MP, HUNGRY, THIRSTY만 가능합니다.")]
    public string[] type;
    public int[] num;
}

public class ItemEffectDB : MonoBehaviour
{
    [SerializeField]
    private ItemEffect[] itemEffects;

    [SerializeField]
    private StatusController playerStatus = null;

    //[SerializeField]
    //private WeaponManager weaponManager;

    private const string HP = "HP", MP = "MP", HUNGRY = "HUNGRY", THIRSTY = "THIRSTY";

    public void UseItem(Item item)
    {
        if (playerStatus == null)
            playerStatus = GameObject.Find("UIManager").transform.Find("Canvas").Find("Status").GetComponent<StatusController>();

        //if (item.itemType == Item.ItemType.Equipment)
        //{
        //      StartCoroutine(weaponManager.ChangeWeaponCoroutine(item.weaponType, item.itemName));
        //}
        //else 

        if (item.itemType == Item.ItemType.Used || item.itemType == Item.ItemType.Recycle)
        {
            for (int i = 0; i < itemEffects.Length; ++i)
            {
                if (itemEffects[i].item == item)
                {
                    for (int j = 0; j < itemEffects[i].type.Length; ++j)
                    {
                        switch (itemEffects[i].type[j])
                        {
                            case HP:
                                playerStatus.IncreaseHp(itemEffects[i].num[j]);
                                Debug.Log(itemEffects[i].num[j] + "만큼 회복");
                                break;
                            case MP:
                                playerStatus.IncreaseMp(itemEffects[i].num[j]);
                                break;
                            case THIRSTY:
                                playerStatus.IncreaseThirsty(itemEffects[i].num[j]);
                                break;
                            case HUNGRY:
                                playerStatus.IncreaseHungry(itemEffects[i].num[j]);
                                break;
                            default:
                                Debug.Log("Wrong Status type. (HP, MP, HUNGRY, THIRSTY)");
                                break;
                        }
                        Debug.Log(item.ItemName + " 을 사용했습니다.");
                    }
                    return;
                }
            }
            Debug.Log("ItemEffectDB에 일치하는 itemName이 없습니다.");
        }
    }
}
