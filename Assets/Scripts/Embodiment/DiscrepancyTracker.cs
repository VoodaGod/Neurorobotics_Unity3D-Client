using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscrepancyTracker : MonoBehaviour
{
	[SerializeField]
	Transform remoteHandPosLeft;
	[SerializeField]
	Transform remoteHandPosRight;
	[SerializeField]
	Transform remoteFootPosLeft;
	[SerializeField]
	Transform remoteFootPosRight;

	[SerializeField]
	Transform localHandPosLeft;
	[SerializeField]
	Transform localHandPosRight;
	[SerializeField]
	Transform localFootPosLeft;
	[SerializeField]
	Transform localFootPosRight;


	[SerializeField]
	UserAvatarService userAvatarService;

	public float toleranceDistance = 0.1f;
	public float toleranceTime = 1f;

	void Start()
	{
	}


	float timerHandLeft, timerHandRight, timerFootLeft, timerFootRight;
	bool discrepancyHandLeft, discrepancyHandRight, discrepancyFootLeft, discrepancyFootRight;
	void FixedUpdate()
	{
		if (userAvatarService != null && userAvatarService.avatar != null)
		{
			if (remoteHandPosLeft == null)
			{
				remoteHandPosLeft = GameObject.Find(userAvatarService.avatar.name + "::avatar_ybot::" + localHandPosLeft.name).transform;
			}
			if (remoteHandPosRight == null)
			{
				remoteHandPosRight = GameObject.Find(userAvatarService.avatar.name + "::avatar_ybot::" + localHandPosRight.name).transform;
			}
			if (remoteFootPosLeft == null)
			{
				remoteFootPosLeft = GameObject.Find(userAvatarService.avatar.name + "::avatar_ybot::" + localFootPosLeft.name).transform;
			}
			if (remoteFootPosRight == null)
			{
				remoteFootPosRight = GameObject.Find(userAvatarService.avatar.name + "::avatar_ybot::" + localFootPosRight.name).transform;
			}

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
