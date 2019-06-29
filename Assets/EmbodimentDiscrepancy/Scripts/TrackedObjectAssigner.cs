using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackedObjectAssigner : MonoBehaviour {

	public UserAvatarVisualsIKControl userAvatarVisualsIKControl;

	public Transform avatarLeftHandTransform;
	public Transform avatarRightHandTransform;
	public Transform avatarLeftFootTransform;
	public Transform avatarRightFootTransform;
	public Transform avatarBodyTransform;

	public List<SteamVR_TrackedObject> trackedObjects = new List<SteamVR_TrackedObject>();
	public Transform[] IKTargets;


	SteamVR_TrackedObject GetClosestTrackedObject(Transform origin)
	{
		float minDistance = -1;
		SteamVR_TrackedObject closest = null;
		foreach (SteamVR_TrackedObject trackedObject in trackedObjects)
		{
			float distance = Vector3.Distance(origin.position, trackedObject.transform.position);
			if (distance < minDistance || minDistance == -1){
				closest = trackedObject;
				minDistance = distance;
			}
		}
		return closest;
	}

	void AssignTrackersToRoles()
	{
		SteamVR_TrackedObject closest;
		closest = GetClosestTrackedObject(avatarLeftHandTransform);
		userAvatarVisualsIKControl.leftHandTarget = IKTargets[trackedObjects.IndexOf(closest)];
		closest = GetClosestTrackedObject(avatarRightHandTransform);
		userAvatarVisualsIKControl.rightHandTarget = IKTargets[trackedObjects.IndexOf(closest)];
		closest = GetClosestTrackedObject(avatarLeftFootTransform);
		userAvatarVisualsIKControl.leftFootTarget = IKTargets[trackedObjects.IndexOf(closest)];
		closest = GetClosestTrackedObject(avatarRightFootTransform);
		userAvatarVisualsIKControl.rightFootTarget = IKTargets[trackedObjects.IndexOf(closest)];
		closest = GetClosestTrackedObject(avatarBodyTransform);
		userAvatarVisualsIKControl.bodyTarget = IKTargets[trackedObjects.IndexOf(closest)];
	}


	// Use this for initialization
	void Start () {


	}

	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp(KeyCode.A))
		{
			AssignTrackersToRoles();
			userAvatarVisualsIKControl.GetComponent<Animator>().enabled = true;
		}
	}
}
