using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderSetup : MonoBehaviour {

	public string avatarColliderLayerName;
	public string IKTargetsColliderLayerName;
	public string EnvironmentLayerName;
	public string IKTargetsLayerName;

	// Use this for initialization
	void Start () {
		Physics.IgnoreLayerCollision(LayerMask.NameToLayer(avatarColliderLayerName), LayerMask.NameToLayer(IKTargetsColliderLayerName));
		Physics.IgnoreLayerCollision(LayerMask.NameToLayer(EnvironmentLayerName), LayerMask.NameToLayer(IKTargetsLayerName));
	}
	
	// Update is called once per frame
	void Update () {
	}
}
