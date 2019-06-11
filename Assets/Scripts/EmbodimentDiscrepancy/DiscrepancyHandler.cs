using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EmbodimentDiscrepancy
{

	public enum TrackedJoint
	{
		Head, HandLeft, HandRight, FootLeft, FootRight
	}

	public struct Discrepancy
	{
		public TrackedJoint joint;
		public Transform trackedPos;
		public Transform simulatedPos;
		public float distance;
		public float duration;
	}

	public class DiscrepancyHandler : MonoBehaviour
	{

		[SerializeField]
		[Tooltip("Set to instance in scene, not a prefab. If not set, will be searched in scene")]
		DiscrepancyLineHandler discrepancyLineHandler;

		[SerializeField]
		[Tooltip("If not set, will be searched in scene")]
		DiscrepancyHeadEffects discrepancyHeadEffects;

		[SerializeField]
		[Tooltip("If not set, will be searched in scene")]
		DiscrepancyHapticHandler discrepancyHapticHandler;

		[SerializeField]
		SteamVR_TrackedObject leftHandTrackedObject, rightHandTrackedObject;

		public bool lineEffectEnabled = true;
		public bool lineEffectHands = true;
		public bool lineEffectFeet = true;
		public bool fadeToBlackEffect = true;
		public bool blurEffect = true;
		public bool hapticEffectHands = true;

		[SerializeField]
		[Tooltip("seconds before discrepancy is handled")]
		[Range(0, 3)]
		public float toleranceTimeHands = 1;

		[SerializeField]
		[Tooltip("seconds before discrepancy is handled")]
		[Range(0, 3)]
		public float toleranceTimeFeet = 1;

		[SerializeField]
		[Tooltip("seconds before discrepancy is handled")]
		[Range(0, 1)]
		public float toleranceTimeHead = 0.5f;

		List<Discrepancy> discrepancyList = new List<Discrepancy>();


		public void HandleDiscrepancy(Discrepancy disc)
		{
			discrepancyList.Add(disc);
		}

		void Update()
		{
			discrepancyHapticHandler.SetTrackedObjectForJoint(TrackedJoint.HandLeft, leftHandTrackedObject);
			discrepancyHapticHandler.SetTrackedObjectForJoint(TrackedJoint.HandRight, rightHandTrackedObject);

			foreach (Discrepancy disc in discrepancyList)
			{
				if (disc.joint == TrackedJoint.HandLeft || disc.joint == TrackedJoint.HandRight)
				{
					if (lineEffectEnabled && lineEffectHands && (disc.duration > toleranceTimeHands)){
						discrepancyLineHandler.DrawLine(disc);
					}
					if(hapticEffectHands){
						discrepancyHapticHandler.HandleRumble(disc);
					}
				}

				if (disc.joint == TrackedJoint.FootLeft || disc.joint == TrackedJoint.FootRight)
				{
					if (lineEffectEnabled && lineEffectFeet && (disc.duration > toleranceTimeFeet)){
						discrepancyLineHandler.DrawLine(disc);
					}
				}

				if (disc.joint == TrackedJoint.Head)
				{
					if (fadeToBlackEffect && disc.duration > toleranceTimeHead){
						discrepancyHeadEffects.FadeToBlack(disc);
					}
					else{
						discrepancyHeadEffects.FadeToBlack(distance: 0);
					}
					
					if (blurEffect && disc.duration > toleranceTimeHead){
						discrepancyHeadEffects.Blur(disc);
					}
					else{
						discrepancyHeadEffects.Blur(distance: 0);
					}
				}
			}
			discrepancyList.Clear();
		}

		// Use this for initialization
		void Start()
		{
			if (discrepancyLineHandler == null)
			{
				discrepancyLineHandler = GameObject.FindObjectOfType<DiscrepancyLineHandler>();
				if (discrepancyLineHandler == null){
					Debug.LogError("no DiscrepancyLineHandler found");
				}
			}

			if (discrepancyHeadEffects == null)
			{
				discrepancyHeadEffects = GameObject.FindObjectOfType<DiscrepancyHeadEffects>();
				if (discrepancyHeadEffects == null){
					Debug.LogError("no DiscrepancyHeadEffects found");
				}
			}

			if (discrepancyHapticHandler== null)
			{
				discrepancyHapticHandler = GameObject.FindObjectOfType<DiscrepancyHapticHandler>();
				if (discrepancyHapticHandler == null){
					Debug.LogError("no DiscrepancyHapticHandler found");
				}
			}

			if (leftHandTrackedObject == null || rightHandTrackedObject == null){
				Debug.LogError("trackedObject(s) not set");
			}
		}
	}
}