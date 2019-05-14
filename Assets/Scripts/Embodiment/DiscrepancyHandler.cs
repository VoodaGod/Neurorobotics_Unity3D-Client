using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscrepancyHandler : MonoBehaviour {

	[SerializeField]
	GameObject DiscrepancyLinePrefab;

	public bool lineEffectEnabled = true;
	public bool lineEffectHands = true;
	public bool lineEffectFeet = true;
	Dictionary<DiscrepancyTracker.TrackedJoints, DiscrepancyLine> lineDict = new Dictionary<DiscrepancyTracker.TrackedJoints, DiscrepancyLine>();

	public void HandleDiscrepancy(DiscrepancyTracker.TrackedJoints joint, Transform local, Transform remote, float distance)
	{
		if (joint == DiscrepancyTracker.TrackedJoints.HandLeft || joint == DiscrepancyTracker.TrackedJoints.HandRight)
		{
			if (lineEffectEnabled && lineEffectHands)
			{ 
				if (!lineDict.ContainsKey(joint)){
					lineDict[joint] = Instantiate(DiscrepancyLinePrefab).GetComponent<DiscrepancyLine>();
				}
				lineDict[joint].DrawLine(local, remote, distance);
			}
		}

		if (joint == DiscrepancyTracker.TrackedJoints.FootLeft || joint == DiscrepancyTracker.TrackedJoints.FootRight)
		{
			if (lineEffectEnabled && lineEffectFeet)
			{
				if (!lineDict.ContainsKey(joint)){
					lineDict[joint] = Instantiate(DiscrepancyLinePrefab).GetComponent<DiscrepancyLine>();
				}
				lineDict[joint].DrawLine(local, remote, distance);
			}
		}
	}

	public void StopHandlingDiscrepancyHand(DiscrepancyTracker.TrackedJoints joint){
		if (!lineDict.ContainsKey(joint)){
			Debug.LogError("cant stop handling discrepancy before handling it");
		}
		lineDict[joint].StopDrawLine();
	}


	// Use this for initialization
	void Start () {
		if (DiscrepancyLinePrefab == null){
			Debug.LogError("DiscrepancyLinePrefab not set");
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
