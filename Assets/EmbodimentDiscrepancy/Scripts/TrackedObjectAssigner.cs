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
	public Transform leftHandIKTargetPrefab;
	public Transform rightHandIKTargetPrefab;
	public Transform leftFootIKTargetPrefab;
	public Transform rightFootIKTargetPrefab;
	public Transform bodyIKTargetPrefab;


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
		Transform IKTarget;

		closest = GetClosestTrackedObject(avatarLeftHandTransform);
		IKTarget = Instantiate(leftHandIKTargetPrefab, closest.transform);
		userAvatarVisualsIKControl.leftHandTarget = IKTarget;

		closest = GetClosestTrackedObject(avatarRightHandTransform);
		IKTarget = Instantiate(rightHandIKTargetPrefab, closest.transform);
		userAvatarVisualsIKControl.rightHandTarget = IKTarget;

		closest = GetClosestTrackedObject(avatarLeftFootTransform);
		IKTarget = Instantiate(leftFootIKTargetPrefab, closest.transform);
		userAvatarVisualsIKControl.leftFootTarget = IKTarget;

		closest = GetClosestTrackedObject(avatarRightFootTransform);
		IKTarget = Instantiate(rightFootIKTargetPrefab, closest.transform);
		userAvatarVisualsIKControl.rightFootTarget = IKTarget;

		closest = GetClosestTrackedObject(avatarBodyTransform);
		IKTarget = Instantiate(bodyIKTargetPrefab, closest.transform);
		userAvatarVisualsIKControl.bodyTarget = IKTarget;
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
