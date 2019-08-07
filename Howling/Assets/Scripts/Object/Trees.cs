using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class OldTree
{
    public int i;
    public Transform transform;

    public OldTree(int n, Transform t)
    {
        i = n;
        transform = t;
    }
}

public class Trees : MonoBehaviour
{
    public GameObject Tree1;
    public GameObject Tree2;
    public GameObject Tree3;

    // Start is called before the first frame update
    void Start()
    {
        OldTree[] oldTrees = new OldTree[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name == "Tree1")
                oldTrees[i] = new OldTree(1, transform.GetChild(i).transform);
            else if (transform.GetChild(i).name == "Tree2")
                oldTrees[i] = new OldTree(2, transform.GetChild(i).transform);
            else if (transform.GetChild(i).name == "Tree3")
                oldTrees[i] = new OldTree(3, transform.GetChild(i).transform);
        }

        for(int i = transform.childCount - 1; i>= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < oldTrees.Length; i++)
        {
            GameObject gameObject;

            Transform newTransform = oldTrees[i].transform;
            newTransform.localScale = new Vector3(1f, 1f, 1f);

            Debug.Log(newTransform.localScale);

            if (oldTrees[i].i == 1)
                gameObject = Instantiate<GameObject>(Tree1, newTransform);
            else if (oldTrees[i].i == 2)
                gameObject = Instantiate<GameObject>(Tree2, newTransform);
            else
                gameObject = Instantiate<GameObject>(Tree3, newTransform);

            gameObject.transform.parent = this.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
