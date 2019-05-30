using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscrepancyHandler : MonoBehaviour {

	[SerializeField]
	[Tooltip("Set to instance in scene, not a prefab. If not set, will be searched in scene")]
	DiscrepancyLineHandler discrepancyLineHandler;

	public bool lineEffectEnabled = true;
	public bool lineEffectHands = true;
	public bool lineEffectFeet = true;

	[SerializeField]
	[Tooltip("seconds before discrepancy is handled")]
	[Range(0,5)]
	public float toleranceTime = 1;

	[SerializeField]
	[Tooltip("If not set, will be searched in scene")]
	DiscrepancyHeadEffects discrepancyHeadEffects;
	public bool fadeToBlackEffect = true;

	public void HandleDiscrepancy(DiscrepancyTracker.TrackedJoints joint, Transform localPos, Transform remotePos, float distance, float time)
	{
		if (joint == DiscrepancyTracker.TrackedJoints.HandLeft || joint == DiscrepancyTracker.TrackedJoints.HandRight)
		{
			if (lineEffectEnabled && lineEffectHands && (time > toleranceTime)){
				discrepancyLineHandler.DrawLine(joint, localPos, remotePos, distance);
			}
		}

		if (joint == DiscrepancyTracker.TrackedJoints.FootLeft || joint == DiscrepancyTracker.TrackedJoints.FootRight)
		{
			if (lineEffectEnabled && lineEffectFeet && (time > toleranceTime)){
				discrepancyLineHandler.DrawLine(joint, localPos, remotePos, distance);
			}
		}

		if (joint == DiscrepancyTracker.TrackedJoints.Head)
		{
			if (fadeToBlackEffect){
				discrepancyHeadEffects.FadeToBlack(distance);
			}
		}
	}

	// Use this for initialization
	void Start () 
	{
		if (discrepancyLineHandler == null){
			discrepancyLineHandler = GameObject.FindObjectOfType<DiscrepancyLineHandler>();
			if (discrepancyLineHandler == null){
				Debug.LogError("no DiscrepancyLineHandler found");
			}
		}

		if (discrepancyHeadEffects == null){
			discrepancyHeadEffects = GameObject.FindObjectOfType<DiscrepancyHeadEffects>();
			if (discrepancyHeadEffects == null){
				Debug.LogError("no DiscrepancyHeadEffects found");
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
