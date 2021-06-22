using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class ActivateEnemy : MonoBehaviour
{
    // Start is called before the first frame update
    public RoomManager roomManager;
    void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        print(roomManager.currentRoom.transform.GetComponentsInChildren<Transform>());
        var enemies = roomManager.currentRoom.transform.GetComponentsInChildren<Transform>().Where(t => t.tag == "Enemy").ToArray();
        foreach (var enemy in enemies)
        {
            print(enemy.gameObject.GetComponentInChildren<Enemy>().inHurt);
            print(enemy.gameObject.GetComponentInChildren<Enemy>().currentHealth);
            if (!enemy.gameObject.GetComponentInChildren<Enemy>().inHurt && enemy.gameObject.GetComponentInChildren<Enemy>().currentHealth > 0)
            {
                enemy.gameObject.GetComponent<NavMeshAgent>().enabled = true;
                enemy.gameObject.GetComponent<AgentScript>().enabled = true;
            }
        }
    }

}
