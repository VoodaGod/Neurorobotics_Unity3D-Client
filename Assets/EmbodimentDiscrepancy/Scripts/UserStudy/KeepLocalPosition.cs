using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepLocalPosition : MonoBehaviour {

	Vector3 originalLocalPosition;
	Quaternion originalLocalRotation;
	// Use this for initialization
	void Start () {
		originalLocalPosition = transform.localPosition;
		originalLocalRotation = transform.localRotation;
	}
	
	// Update is called once per frame
	void Update () {
		transform.localPosition = originalLocalPosition;
		transform.localRotation = originalLocalRotation;
	}
}
