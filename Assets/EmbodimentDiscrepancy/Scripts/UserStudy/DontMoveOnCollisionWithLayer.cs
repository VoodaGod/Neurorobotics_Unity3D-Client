using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontMoveOnCollisionWithLayer : MonoBehaviour {

	public string layerToIgnore;

	new Rigidbody rigidbody;

	// Use this for initialization
	void Start () {
		rigidbody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	int ignoredColliding = 0;
	int allowedColliding = 0;
	bool overRuling = false;

	void OnCollisionEnter(Collision other) 
	{
		if (other.gameObject.layer == LayerMask.NameToLayer(layerToIgnore))
		{
			ignoredColliding += 1;
			if (!overRuling) {
				rigidbody.constraints = RigidbodyConstraints.FreezeAll;
			}
		}
		else {
			allowedColliding += 1;
			overRuling = true;
			rigidbody.constraints = RigidbodyConstraints.None;
		}
	}

	void OnCollisionExit(Collision other) 
	{
		if (other.gameObject.layer == LayerMask.NameToLayer(layerToIgnore)) 
		{
			ignoredColliding -= 1;
			if (ignoredColliding == 0) {
				rigidbody.constraints = RigidbodyConstraints.None;
			}
		}
		else {
			allowedColliding -= 1;
			if (ignoredColliding > 0 && allowedColliding == 0) {
				overRuling = false;
				rigidbody.constraints = RigidbodyConstraints.FreezeAll;
			}
		}
	}
}
