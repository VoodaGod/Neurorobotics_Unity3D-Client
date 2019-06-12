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
		[Tooltip("If not set, will be searched in scene")]
		DiscrepancySoundHandler discrepancySoundHandler;

		[SerializeField]
		SteamVR_TrackedObject leftHandTrackedObject, rightHandTrackedObject, leftFootTrackedObject, rightFootTrackedObject;

		public bool lineEffectEnabled = true;
		public bool lineEffectHands = true;
		public bool lineEffectFeet = true;
		public bool fadeToBlackEffect = true;
		public bool blurEffect = true;
		public bool hapticEffectHands = true;
		public bool hapticEffectFeet = true;
		public bool geigerSoundEffect = true;

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
			discrepancyHapticHandler.SetTrackedObjectForJoint(TrackedJoint.FootLeft, leftFootTrackedObject);
			discrepancyHapticHandler.SetTrackedObjectForJoint(TrackedJoint.FootRight, rightFootTrackedObject);

			discrepancySoundHandler.SetTransformParentForJoint(TrackedJoint.HandLeft, leftHandTrackedObject.gameObject);
			discrepancySoundHandler.SetTransformParentForJoint(TrackedJoint.HandRight, rightHandTrackedObject.gameObject);
			discrepancySoundHandler.SetTransformParentForJoint(TrackedJoint.FootLeft, leftFootTrackedObject.gameObject);
			discrepancySoundHandler.SetTransformParentForJoint(TrackedJoint.FootRight, rightFootTrackedObject.gameObject);

			foreach (Discrepancy disc in discrepancyList)
			{
				if (disc.joint == TrackedJoint.HandLeft || disc.joint == TrackedJoint.HandRight)
				{
					if (lineEffectEnabled && lineEffectHands && (disc.duration > toleranceTimeHands)){
						discrepancyLineHandler.DrawLine(disc);
					}
					if (hapticEffectHands && (disc.duration > toleranceTimeHands)){
						discrepancyHapticHandler.HandleRumble(disc);
					}
					if (geigerSoundEffect && (disc.duration > toleranceTimeHands)){
						discrepancySoundHandler.HandleGeigerSounds(disc);
					}
				}

				if (disc.joint == TrackedJoint.FootLeft || disc.joint == TrackedJoint.FootRight)
				{
					if (lineEffectEnabled && lineEffectFeet && (disc.duration > toleranceTimeFeet)){
						discrepancyLineHandler.DrawLine(disc);
					}
					if (hapticEffectFeet && (disc.duration > toleranceTimeFeet)){
						discrepancyHapticHandler.HandleRumble(disc);
					}
					if (geigerSoundEffect && (disc.duration > toleranceTimeFeet)){
						discrepancySoundHandler.HandleGeigerSounds(disc);
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
			
			if (discrepancySoundHandler== null)
			{
				discrepancySoundHandler = GameObject.FindObjectOfType<DiscrepancySoundHandler>();
				if (discrepancySoundHandler == null){
					Debug.LogError("no DiscrepancySoundHandler found");
				}
			}

			if (leftHandTrackedObject == null || rightHandTrackedObject == null || leftFootTrackedObject == null || rightHandTrackedObject == null){
				Debug.LogError("trackedObject(s) not set");
			}
		}
	}
}