using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscrepancyHandler : MonoBehaviour {

	[SerializeField]
	GameObject DiscrepancyLinePrefab;

	public bool lineEffectEnabled = true;
	public bool lineEffectHands = true;
	public bool lineEffectFeet = true;

	[SerializeField]
	SteamVR_Camera steamvrCam;
	public bool fadeToBlackEffect = true;
	[SerializeField]
	float headMaxDistance = 0.3f;
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

		if (joint == DiscrepancyTracker.TrackedJoints.Head)
		{
			if (fadeToBlackEffect)
			{
				float alpha = Mathf.Lerp(0, 1, distance / headMaxDistance);
				SteamVR_Fade.Start(new Color(0, 0, 0, alpha), 0);
			}
		}
	}

	public void StopHandlingDiscrepancyHand(DiscrepancyTracker.TrackedJoints joint)
	{
		if (lineDict.ContainsKey(joint)){
			lineDict[joint].StopDrawLine();
		}
	}


	// Use this for initialization
	void Start () 
	{
		if (DiscrepancyLinePrefab == null){
			Debug.LogError("DiscrepancyLinePrefab not set");
		}

		if (steamvrCam == null)
		{
			steamvrCam = GameObject.FindObjectOfType<SteamVR_Camera>();
			if (steamvrCam == null){
				Debug.LogError("no SteamVR_Camera found");
			}
			if (steamvrCam.gameObject.GetComponent<SteamVR_Fade>() == null){
				steamvrCam.gameObject.AddComponent<SteamVR_Fade>();
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
