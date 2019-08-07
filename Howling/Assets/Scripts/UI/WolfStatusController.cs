using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    private float hp = 100f;
    private float currentHp;

    private float hungry = 100f;
    private float currentHungry;

    private float friendly = 0f;
    private float currentFriendly;

    // 필요한 이미지
    [SerializeField]
    private Image[] images_Gauge;

    private const int HP = 0, FRIENDLY = 1, HUNGRY = 2;

    // Start is called before the first frame update
    void Start()
    {
        currentHp = GameObject.Find("wolf_agent").GetComponent<WolfAgent>().Hp;
        currentFriendly = GameObject.Find("wolf_agent").GetComponent<WolfAgent>().Friendly;
        currentHungry = GameObject.Find("wolf_agent").GetComponent<WolfAgent>().Hungry;

        //currentHungryDecreaseTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        currentHp = GameObject.Find("wolf_agent").GetComponent<WolfAgent>().Hp;
        currentFriendly = GameObject.Find("wolf_agent").GetComponent<WolfAgent>().Friendly;
        currentHungry = GameObject.Find("wolf_agent").GetComponent<WolfAgent>().Hungry;
        GaugeUpdate();
    }

    private void GaugeUpdate()
    {
        images_Gauge[HP].fillAmount = currentHp / hp;
        images_Gauge[FRIENDLY].fillAmount = currentFriendly / friendly;
        images_Gauge[HUNGRY].fillAmount = currentHungry / hungry;
    }
}
