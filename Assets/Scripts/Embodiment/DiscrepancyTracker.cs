using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DiscrepancyHandler))]
public class DiscrepancyTracker : MonoBehaviour
{

	[SerializeField]
	[Tooltip("Set to tracked object of avatar")]
	Transform localHeadPos, localHandPosLeft, localHandPosRight, localFootPosLeft, localFootPosRight;

	Dictionary<TrackedJoints, Transform> localPosDict = new Dictionary<TrackedJoints, Transform>();

	Dictionary<TrackedJoints, Transform> remotePosDict = new Dictionary<TrackedJoints, Transform>();

	[SerializeField]
	[Tooltip("If not set, will be searched in scene")]
	UserAvatarService userAvatarService;

	[SerializeField]
	[Tooltip("If not set, will be searched in scene")]
	DiscrepancyHandler discrepancyHandler;

	[SerializeField]
	[Range(0,1)]
	float toleranceDistanceGlobal = 0.1f;

	Dictionary<TrackedJoints, float> timerDict = new Dictionary<TrackedJoints, float>();

	public enum TrackedJoints{
		Head, HandLeft, HandRight, FootLeft, FootRight
	}

	void Start()
	{
		if (userAvatarService == null){
			userAvatarService = GameObject.FindObjectOfType<UserAvatarService>();
			if (userAvatarService == null){
				Debug.LogError("no UserAvatarService found");
			}
		}

		if (discrepancyHandler == null){
			discrepancyHandler = GameObject.FindObjectOfType<DiscrepancyHandler>();
			if (discrepancyHandler == null){
				Debug.LogError("no DiscrepancyHandler found");
			}
		}

		if (localHeadPos == null || localHandPosLeft == null || localHandPosRight == null || localFootPosLeft == null || localFootPosRight == null){
			Debug.LogError("local avatar joint(s) not set");
		}

		localPosDict[TrackedJoints.Head] = localHeadPos;
		localPosDict[TrackedJoints.HandLeft] = localHandPosLeft;
		localPosDict[TrackedJoints.HandRight] = localHandPosRight;
		localPosDict[TrackedJoints.FootLeft] = localFootPosLeft;
		localPosDict[TrackedJoints.FootRight] = localFootPosRight;
	}

	void Update()
	{
		if (userAvatarService.avatar != null)
		{
			//find remote joint objects
			foreach (KeyValuePair<TrackedJoints, Transform> entry in localPosDict)
			{
				string name = userAvatarService.avatar.name + "::avatar_ybot::" + localPosDict[entry.Key].name;
				TrackedJoints joint = entry.Key;
				if (!remotePosDict.ContainsKey(joint))
				{
					remotePosDict[joint] = GameObject.Find(name).transform;
				}
				if (remotePosDict[joint] == null)
				{
					Debug.LogError("could not find " + name);
				}
			}

			//check & handle discrepancies
			foreach (KeyValuePair<TrackedJoints, Transform> entry in localPosDict)
			{
				TrackedJoints joint = entry.Key;
				Transform localPos = entry.Value;
				Transform remotePos = remotePosDict[joint];
				float dist = (localPos.position - remotePos.position).magnitude;
				if (dist > toleranceDistanceGlobal)
				{
					if (!timerDict.ContainsKey(joint)){
						timerDict[joint] = 0;
					}
					timerDict[joint] += Time.deltaTime;
					HandleDiscrepancy(joint, localPos, remotePos, dist - toleranceDistanceGlobal, timerDict[joint]);
				}
				else{
					timerDict[joint] = 0;
				}
			}
		}
	}

	void HandleDiscrepancy(TrackedJoints joint, Transform localPos, Transform remotePos, float distance, float time){
		discrepancyHandler.HandleDiscrepancy(joint, localPos, remotePos, distance, time);
	}
}
