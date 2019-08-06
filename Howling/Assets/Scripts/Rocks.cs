using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class OldRock
{
    public int i;
    public Transform transform;

    public OldRock(int n, Transform t)
    {
        i = n;
        transform = t;
    }
}

public class Rocks : MonoBehaviour
{
    public GameObject Rock1;
    public GameObject Rock2;
    public GameObject Rock3;
    public GameObject Rock4;

    // Start is called before the first frame update
    void Start()
    {
        OldRock[] oldRocks = new OldRock[transform.GetChildCount()];

        for (int i = 0; i < transform.GetChildCount(); i++)
        {
            if (transform.GetChild(i).name == "Rock1")
                oldRocks[i] = new OldRock(1, transform.GetChild(i).transform);
            else if (transform.GetChild(i).name == "Rock2")
                oldRocks[i] = new OldRock(2, transform.GetChild(i).transform);
            else if (transform.GetChild(i).name == "Rock3")
                oldRocks[i] = new OldRock(3, transform.GetChild(i).transform);
            else if (transform.GetChild(i).name == "Rock4")
                oldRocks[i] = new OldRock(4, transform.GetChild(i).transform);
        }

        for (int i = transform.GetChildCount() - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < oldRocks.Length; i++)
        {
            GameObject gameObject;

            Transform newTransform = oldRocks[i].transform;
            newTransform.localScale = new Vector3(1f, 1f, 1f);

            Debug.Log(newTransform.localScale);

            if (oldRocks[i].i == 1)
                gameObject = Instantiate<GameObject>(Rock1, newTransform);
            else if (oldRocks[i].i == 2)
                gameObject = Instantiate<GameObject>(Rock2, newTransform);
            else if (oldRocks[i].i == 3)
                gameObject = Instantiate<GameObject>(Rock3, newTransform);
            else
                gameObject = Instantiate<GameObject>(Rock4, newTransform);
            gameObject.transform.parent = this.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
