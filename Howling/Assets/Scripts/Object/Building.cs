using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    int hp = 3;

    [SerializeField]
    private Recipe recipe;

    [SerializeField]
    private BoxCollider col;

    [SerializeField]
    private GameObject basic_resource;
    [SerializeField]
    private GameObject fract_resource;
    [SerializeField]
    private GameObject effect_resource;

    [SerializeField]
    private float destroyTime; // 파편 제거 시간

    public void Hittied()
    {
        hp--;
        if (hp <= 0)
            Destruction();
    }

    private void Destruction()
    {
        basic_resource.SetActive(false);

        Vector3 position = new Vector3(col.bounds.center.x, col.bounds.center.y + 0.5f, col.bounds.center.z);

       if(recipe.Material1 != null)
        {
            DropMaterial(recipe.Material1, recipe.Material1_num);
        }

        if (recipe.Material2 != null)
        {
            DropMaterial(recipe.Material2, recipe.Material2_num);
        }
        if (recipe.Material3 != null)
        {
            DropMaterial(recipe.Material3, recipe.Material3_num);
        }

        // var fract_clone = Instantiate(fract_resource, col.bounds.center, Quaternion.identity);
        // fract_clone.SetActive(true);
        // Destroy(fract_clone, destroyTime);

        Destroy(gameObject);
    }

    private void DropMaterial(Item item, int cnt) {
        for (int i = 0; i < cnt; i++)
        {
            int ran = Random.Range(0, 10);
            if (ran < 3)
                Instantiate(item.ItemPrefab, col.bounds.center, Quaternion.identity);
        }
    }
}
