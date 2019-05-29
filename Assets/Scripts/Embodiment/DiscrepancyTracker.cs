using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DiscrepancyHandler))]
public class DiscrepancyTracker : MonoBehaviour
{
	
	[SerializeField]
	Transform localHeadPos, localHandPosLeft, localHandPosRight, localFootPosLeft, localFootPosRight;

	Dictionary<TrackedJoints, Transform> localPosDict = new Dictionary<TrackedJoints, Transform>();

	Dictionary<TrackedJoints, Transform> remotePosDict = new Dictionary<TrackedJoints, Transform>();

	[SerializeField]
	UserAvatarService userAvatarService;

	[SerializeField]
	DiscrepancyHandler discrepancyHandler;

	public float toleranceDistance = 0.1f;
	public float toleranceTime = 1f;

	Dictionary<TrackedJoints, bool> discrepancyDict = new Dictionary<TrackedJoints, bool>();
	Dictionary<TrackedJoints, float> timerDict = new Dictionary<TrackedJoints, float>();

	public enum TrackedJoints{
		Head, HandLeft, HandRight, FootLeft, FootRight
	}

	void Start()
	{
		if (userAvatarService == null)
		{
			Debug.Log("userAvatarService not set, looking in scene...");
			userAvatarService = Object.FindObjectOfType<UserAvatarService>();
			if (userAvatarService == null){
				Debug.LogError("no UserAvatarService found");
			}
		}
		if (localHeadPos == null || localHandPosLeft == null || localHandPosRight == null || localFootPosLeft == null || localFootPosRight == null){
			Debug.LogError("local avatar joint(s) not set");
		}

		TrackedJoints joint = TrackedJoints.Head;
		localPosDict[joint] = localHeadPos;
		discrepancyDict[joint] = false;
		timerDict[joint] = 0;
		joint = TrackedJoints.HandLeft;
		localPosDict[joint] = localHandPosLeft;
		discrepancyDict[joint] = false;
		timerDict[joint] = 0;
		joint = TrackedJoints.HandRight;
		localPosDict[joint] = localHandPosRight;
		discrepancyDict[joint] = false;
		timerDict[joint] = 0;
		joint = TrackedJoints.FootLeft;
		localPosDict[joint] = localFootPosLeft;
		discrepancyDict[joint] = false;
		timerDict[joint] = 0;
		joint = TrackedJoints.FootRight;
		localPosDict[joint] = localFootPosRight;
		discrepancyDict[joint] = false;
		timerDict[joint] = 0;
	}

	void FixedUpdate()
	{
		if (userAvatarService != null && userAvatarService.avatar != null)
		{
			//find remote joint objects
			foreach (KeyValuePair<TrackedJoints, Transform> entry in localPosDict)
			{
				string name = userAvatarService.avatar.name + "::avatar_ybot::" + localPosDict[entry.Key].name;
				TrackedJoints joint = entry.Key;
				if (!remotePosDict.ContainsKey(joint)){
					remotePosDict[joint] = GameObject.Find(name).transform;
				}
				if (remotePosDict[joint] == null){
					Debug.LogError("could not find " + name);
				}
			}

			//check discrepancies
			foreach (KeyValuePair<TrackedJoints, Transform> entry in localPosDict)
			{
				TrackedJoints joint = entry.Key;
				Transform localPos = entry.Value;
				Transform remotePos = remotePosDict[joint];
				float dist = (localPos.position - remotePos.position).magnitude;
				if (dist > toleranceDistance)
				{
					timerDict[joint] += Time.fixedDeltaTime;
                    //always handle discrepancy
                    if (joint == TrackedJoints.Head)
                    {
                        discrepancyDict[joint] = true;
                        //Debug.Log("Discrepancy at " + joint.ToString());
                        discrepancyHandler.HandleDiscrepancy(joint, localPos, remotePos, dist);
                    }
                    //handle after toleranceTime reached
                    else if (timerDict[joint] >= toleranceTime)
					{
						discrepancyDict[joint] = true;
						//Debug.Log("Discrepancy at " + joint.ToString());
						discrepancyHandler.HandleDiscrepancy(joint, localPos, remotePos, dist);
					}
                    
				}
				else if (dist <= toleranceDistance && discrepancyDict[joint])
				{
					discrepancyDict[joint]= false;
					timerDict[joint] = 0;
					discrepancyHandler.StopHandlingDiscrepancy(joint);
				}
			}
		}
	}
}
