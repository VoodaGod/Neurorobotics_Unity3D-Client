using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]

public class UnityIKControl : MonoBehaviour {

    protected Animator animator;
    public bool ikActive = true;

    public Transform headTarget = null;
    public Transform bodyTarget = null;
    public Transform leftHandTarget = null;
    public Transform rightHandTarget = null;
    public Transform leftFootTarget = null;
    public Transform rightFootTarget = null;


    public Transform lookAtObj = null;

    public Vector3 bodyTargetOffset = new Vector3(0, -0.75f, 0);
    public Vector3 bodyHeadOffset = new Vector3(0, -1.0f, 0);
    /*public Vector3 handTranslationOffset = new Vector3(0, 0, 0.05f);
    public Quaternion leftHandRotationOffset = new Quaternion(0, 0, 0, 0);
    public Quaternion rightHandRotationOffset = new Quaternion(0, 0, 0, 0);*/


    // Use this for initialization
    void Start () {
        animator = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    //a callback for calculating IK
    void OnAnimatorIK()
    {
        if (animator)
        {

            //if the IK is active, set the position and rotation directly to the goal. 
            if (ikActive)
            {
                // position body
                if (bodyTarget != null)
                {
                    this.transform.position = bodyTarget.position + bodyTargetOffset;
                }
                else
                {
                    this.transform.position = headTarget.position + bodyHeadOffset;
                }

                // Set the look target position, if one has been assigned
                if (lookAtObj != null)
                {
                    animator.SetLookAtWeight(1);
                    animator.SetLookAtPosition(lookAtObj.position);
                }

                // Set the right hand target position and rotation, if one has been assigned
                if (rightHandTarget != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandTarget.position /*+ rightHandTarget.TransformDirection(handTranslationOffset)*/);
                    animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandTarget.rotation /** rightHandRotationOffset*/);
                }

                // Set the left hand target position and rotation, if one has been assigned
                if (rightHandTarget != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                    animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position /*+ leftHandTarget.TransformDirection(handTranslationOffset)*/);
                    animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandTarget.rotation /** leftHandRotationOffset*/);
                }

                // Set the right foot target position and rotation, if one has been assigned
                if (rightFootTarget != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);
                    animator.SetIKPosition(AvatarIKGoal.RightFoot, rightFootTarget.position);
                    animator.SetIKRotation(AvatarIKGoal.RightFoot, rightFootTarget.rotation);
                }

                // Set the right foot target position and rotation, if one has been assigned
                if (leftFootTarget != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);
                    animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootTarget.position);
                    animator.SetIKRotation(AvatarIKGoal.LeftFoot, leftFootTarget.rotation);
                }
            }
        }
    }
}
