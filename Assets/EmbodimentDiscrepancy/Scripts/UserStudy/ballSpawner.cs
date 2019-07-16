using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ballSpawner : MonoBehaviour {

    public GameObject ballPrefab;
    public Transform spawnPos;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.B))
        {
            Instantiate(ballPrefab, spawnPos);
        }
		
	}
}
