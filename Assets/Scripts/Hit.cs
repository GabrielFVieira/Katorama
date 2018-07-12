using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hit : MonoBehaviour
{
    public bool collided;
    public string partName;
    public GameObject enemy;
    public string enemyPart;
    public GameObject thisObject;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "EnemyBodyPart" && other.GetComponent<BodyPart>().thisObject != thisObject)
        {
            collided = true;
            enemy = other.GetComponent<BodyPart>().thisObject;
            enemyPart = other.GetComponent<BodyPart>().partName;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "EnemyBodyPart" && other.GetComponent<BodyPart>().thisObject != thisObject     )
        {
            collided = true;
            enemy = other.GetComponent<BodyPart>().thisObject;
            enemyPart = other.GetComponent<BodyPart>().partName;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "EnemyBodyPart")
        {
            collided = false;
            enemy = null;
            enemyPart = "";
        }
    }
}
