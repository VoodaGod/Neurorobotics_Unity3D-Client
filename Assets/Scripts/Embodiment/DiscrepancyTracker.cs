using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscrepancyTracker : MonoBehaviour
{
	[SerializeField]
	Transform localHandPosLeft;
	[SerializeField]
	Transform localHandPosRight;
	[SerializeField]
	Transform localFootPosLeft;
	[SerializeField]
	Transform localFootPosRight;

	Transform remoteHandPosLeft;
	Transform remoteHandPosRight;
	Transform remoteFootPosLeft;
	Transform remoteFootPosRight;

	[SerializeField]
	UserAvatarService userAvatarService;

	public float toleranceDistance = 0.1f;
	public float toleranceTime = 1f;

	void Start()
	{
		if (userAvatarService == null){
			Debug.Log("userAvatarService not set, looking in scene...");
			userAvatarService = Object.FindObjectOfType<UserAvatarService>();
			if (userAvatarService == null){
				Debug.LogError("no userAvatarService found");
			}
		}
		if (localHandPosLeft == null || localHandPosRight == null || localFootPosLeft == null || localFootPosRight == null){
			Debug.LogError("local avatar joints not set");
		}
	}


	float timerHandLeft, timerHandRight, timerFootLeft, timerFootRight;
	bool discrepancyHandLeft, discrepancyHandRight, discrepancyFootLeft, discrepancyFootRight;
	void FixedUpdate()
	{
		if (userAvatarService != null && userAvatarService.avatar != null)
		{
			#region //find remote joint objects
			string name;
			if (remoteHandPosLeft == null)
			{
				name = userAvatarService.avatar.name + "::avatar_ybot::" + localHandPosLeft.name;
				remoteHandPosLeft = GameObject.Find(name).transform;
				if (remoteHandPosLeft == null){
					Debug.LogError("could not find " + name);
				}
			}
			if (remoteHandPosRight == null)
			{
				name = userAvatarService.avatar.name + "::avatar_ybot::" + localHandPosRight.name;
				remoteHandPosRight = GameObject.Find(name).transform;
				if (remoteHandPosRight == null){
					Debug.LogError("could not find " + name);
				}
			}
			if (remoteFootPosLeft == null)
			{
				name = userAvatarService.avatar.name + "::avatar_ybot::" + localFootPosLeft.name;
				remoteFootPosLeft = GameObject.Find(name).transform;
				if (remoteFootPosLeft == null){
					Debug.LogError("could not find " + name);
				}
			}
			if (remoteFootPosRight == null)
			{
				name = userAvatarService.avatar.name + "::avatar_ybot::" + localFootPosRight.name;
				remoteFootPosRight = GameObject.Find(name).transform;
				if (remoteFootPosRight == null){
					Debug.LogError("could not find " + name);
				}
			}
			#endregion

			//check for discrepancies
			float dist = 0;
			dist = (localHandPosLeft.position - remoteHandPosLeft.position).magnitude;
			if (dist > toleranceDistance)
			{
				timerHandLeft += Time.fixedDeltaTime;
				if (timerHandLeft >= toleranceTime)
				{
					discrepancyHandLeft = true;
					Debug.Log("Discrepancy at left hand");
				}
			} else if (dist <= toleranceDistance && discrepancyHandLeft) 
			{
				discrepancyHandLeft = false;
				timerHandLeft = 0;
			}

			dist = (localHandPosRight.position - remoteHandPosRight.position).magnitude;
			if (dist > toleranceDistance)
			{
				timerHandRight += Time.fixedDeltaTime;
				if (timerHandRight >= toleranceTime)
				{
					discrepancyHandRight = true;
					Debug.Log("Discrepancy at rigth hand");
				}
			} else if (dist <= toleranceDistance && discrepancyHandRight) 
			{
				discrepancyHandRight = false;
				timerHandRight = 0;
			}

			dist = (localFootPosLeft.position - remoteFootPosLeft.position).magnitude;
			if (dist > toleranceDistance) 
			{
				timerFootLeft += Time.fixedDeltaTime;
				if (timerFootLeft >= toleranceTime)
				{
					discrepancyFootLeft = true;
					Debug.Log("Discrepancy at left foot");
				}
			} else if (dist <= toleranceDistance && discrepancyFootLeft) 
			{
				discrepancyFootLeft = false;
				timerFootLeft = 0;
			}

			dist = (localFootPosRight.position - remoteFootPosRight.position).magnitude;
			if (dist > toleranceDistance) 
			{
				timerFootRight += Time.fixedDeltaTime;
				if (timerFootRight >= toleranceTime)
				{
					discrepancyFootRight = true;
					Debug.Log("Discrepancy at right foot");
				}
			} else if (dist <= toleranceDistance && discrepancyFootRight) 
			{
				discrepancyFootRight = false;
				timerFootRight = 0;
			}
		}
	}
}
