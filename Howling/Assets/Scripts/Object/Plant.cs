using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    [Header("Plant Points (30 Max)")]
    public float maxEnergy;
    public float matureSize;
    public float growthRate;
    public float seedSpreadRadius;

    [Header("Monitoring")]
    public float energy;
    public float size;
    public float age;

    [Header("Seedling")]
    public GameObject seedingSpawn;

    [Header("Species Parameters")]
    public float energyGrowthRate = .01f;
    public float ageRate = .001f;
    private Transform Environment;

    // Start is called before the first frame update
    void Start()
    {
        size = 1;
        energy = 1;
        age = 0;
        Environment = transform.parent;
        TransformSize();
    }

    private void FixedUpdate()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        if (CanGrow) Grow();
        if (CanReproduce) Reproduce();
        if (Dead)
        {
            Destroy(this);
            Debug.Log("Tree Dead!!");
        }

        age += ageRate;
        energy += energyGrowthRate;
    }

    void TransformSize()
    {
        transform.localScale = Vector3.one * size;
    }

    bool CanGrow
    {
        get
        {
            return energy > ((maxEnergy / 2) + 1);
        }
    }

    bool CanReproduce
    {
        get
        {
            if (size >= matureSize && CanGrow) return true;
            else return false;
        }
    }

    bool Dead
    {
        get
        {
            return energy < 0 || age > matureSize;
        }
    }

    void Grow()
    {
        if (size > matureSize) return;
        energy = energy / 2;
        size += growthRate * Random.value;
        TransformSize();
    }

    void Reproduce()
    {
        var vec = Random.insideUnitCircle * seedSpreadRadius
            + new Vector2(transform.position.x, transform.position.z);
        Instantiate(seedingSpawn, new Vector3(vec.x, transform.position.y, vec.y), Quaternion.identity, Environment);
        energy = energy / 2;
    }
}
