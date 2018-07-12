using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PublicAI : MonoBehaviour {
    public Transform player;
    public Transform enemy;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 mid = new Vector3((player.position.x + enemy.position.x) / 2, transform.position.y, (player.position.z + enemy.position.z) / 2);

        transform.LookAt(mid);
	}
}
