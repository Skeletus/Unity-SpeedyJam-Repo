using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZapRangeCollider : MonoBehaviour
{
    public ElectricityController controller;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter(Collider c)
    {
        Debug.Log("triggered: " + c.tag);
        //If player is hit by enemy, deal damage to player
        if (c.tag == "Enemy")
        {
            controller.AddEnemyInRange(c.gameObject);
        }
    }
    void OnTriggerExit(Collider c)
    {
        //If player is hit by enemy, deal damage to player
        if (c.tag == "Enemy")
        {
            controller.RemoveEnemyInRage(c.gameObject);
        }
    }
}
