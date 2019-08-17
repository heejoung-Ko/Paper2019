using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderMapCheck : MonoBehaviour
{
    public float replaceHeight = 13f;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name + " - UnderMap trigger enter!");

        if (other.tag == "item")
        {
            Vector3 itemPos = other.gameObject.transform.position;
            other.gameObject.transform.position = new Vector3(itemPos.x, replaceHeight, itemPos.z);
        }
        else if (other.tag == "agent")
        {
            WolfAgent agent = other.gameObject.GetComponent<WolfAgent>();
            agent.Done();
            agent.AgentReset();
        }
        else if (other.tag == "enemyCollider")
        {
            Enemy enemy = other.gameObject.GetComponentInParent<Enemy>();
            enemy.state = Enemy.EnemyState.die;
        }
        else
        {
            Destroy(other.gameObject);
        }
    }
}
