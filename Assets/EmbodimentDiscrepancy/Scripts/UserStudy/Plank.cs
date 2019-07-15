using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plank : MonoBehaviour {

    public Rigidbody target;
    new Rigidbody rigidbody;
    public float speed = 0.05f;


    // Use this for initialization
    void Start () {
        rigidbody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        if (target != null)
        {
            rigidbody.MovePosition(rigidbody.position + (speed * Vector3.Normalize(target.position - rigidbody.position)));

            float dist = Vector3.Distance(target.position, transform.position);
            if (dist < 0.1f)
            {
                Destroy(this.gameObject);

            }
        }
	}
}
