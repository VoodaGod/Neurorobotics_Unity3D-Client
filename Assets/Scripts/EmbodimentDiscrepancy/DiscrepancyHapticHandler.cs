using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EmbodimentDiscrepancy
{
	public class DiscrepancyHapticHandler : MonoBehaviour
	{
		[SerializeField]
		[Tooltip("how many rumble pulses can be sent out per second")]
		int maxRumblesPerSecond = 10;
		
		[SerializeField]
		[Tooltip("seconds after which maxRumblesPerSecond is reached")]
		float maxRumblesTime = 5;

		[SerializeField]
		[Tooltip("maximum pulse length in seconds")]
		float maxRumblePulseDuration = 0.1f;

		[SerializeField]
		[Tooltip("distance at which maxRumblePulseDuration is reached")]
		float maxRumblePulseDistance = 1;

		[SerializeField]
		[Tooltip("strength of each pulse")]
		[Range(0,1)]
		float pulseStrength = 1;

		Dictionary<TrackedJoint, SteamVR_TrackedObject> trackedObjectDict = new Dictionary<TrackedJoint, SteamVR_TrackedObject>();
		//Dictionary<TrackedJoint, Rumble> rumbleDict = new Dictionary<TrackedJoint, Rumble>();
		[SerializeField]
		List<Rumble> rumbleList = new List<Rumble>();


		class Rumble
		{
			public int trackedObjectIndex;
			public float timeTillNextPulse;
			public float pulseLength;
			public bool sentOut;
		}

		public void HandleRumble(Discrepancy disc)
		{
			SteamVR_TrackedObject trackedObject;
			if (trackedObjectDict.ContainsKey(disc.joint)){
				trackedObject = trackedObjectDict[disc.joint];
			}
			else
			{
				Debug.Log("haptic not set up for " + disc.joint.ToString());
				return;
			}

			//create rumble for joint if it doesn't exist
			Rumble rumble = null;
			for (int i = 0; i < rumbleList.Count; i++)
			{
				if (rumbleList[i].trackedObjectIndex == (int)trackedObject.index){
					rumble = rumbleList[i];
				}
			}
			if(rumble == null){
				rumble = new Rumble { trackedObjectIndex = (int)trackedObject.index };
				rumbleList.Add(rumble);
			}

			//only apply new rumble when next pulse allowed
			if (rumble.sentOut && rumble.timeTillNextPulse <= 0)
			{
				//length of pulses dependant on distance of discrepancy
				//larger distance -> longer pulse
				rumble.pulseLength = (Mathf.Lerp(0, 1, disc.distance / maxRumblePulseDistance) * (maxRumblePulseDuration));
				//frequency of pulses dependant on duration of discrepancy
				//longer discrepancy -> more frequent pulses
				rumble.timeTillNextPulse = Mathf.Lerp(0, 1, disc.duration / maxRumblesTime) * (1 / maxRumblesPerSecond);
				rumble.sentOut = false;
			}
		}

		//adapted from https://steamcommunity.com/app/358720/discussions/0/405693392914144440/#c357284767229628161
		//length is how long the vibration should go for
		//strength is vibration strength from 0-1
		IEnumerator LongVibration(float length, float strength, int trackedObjectIndex) {
			for(float i = 0; i < length; i += Time.deltaTime) {
				SteamVR_Controller.Input((int)trackedObjectIndex).TriggerHapticPulse((ushort)Mathf.Lerp(0, 3999, strength));
				yield return null;
			}
		}

		public void SetTrackedObjectForJoint(TrackedJoint joint, SteamVR_TrackedObject trackedObject){
			trackedObjectDict[joint] = trackedObject;
		}

		void Update()
		{
			foreach (Rumble rumble in rumbleList)
			{
				//send out pulse
				if (!rumble.sentOut){
					//SteamVR_Controller.Input(rumble.trackedObjectIndex).TriggerHapticPulse((ushort)rumble.pulseLength);
					StartCoroutine(LongVibration(rumble.pulseLength, pulseStrength, rumble.trackedObjectIndex));
					rumble.sentOut = true;
				}
				//update timer
				rumble.timeTillNextPulse -= Time.deltaTime;
			}

		}
	}
}
