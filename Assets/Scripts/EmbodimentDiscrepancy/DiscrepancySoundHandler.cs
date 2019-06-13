using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EmbodimentDiscrepancy
{
	public class DiscrepancySoundHandler : MonoBehaviour
	{
		class GeigerClicker 
		{
			public AudioSource audioSource;
			public List<AudioClip> geigerClickClips;
			public float intensity = 0;
			float timer = 0;
			int prevAudioClipIndex = 0;
			public void DoClick(float deltaTime, int maxClicksPerMinute, float variance)
			{
				timer += Time.deltaTime;
				if (intensity > 0)
				{
					float clicksPerMinute = Mathf.Lerp(0, maxClicksPerMinute, intensity);
					if (variance > 0){
						clicksPerMinute = Random.Range(clicksPerMinute - clicksPerMinute * variance, clicksPerMinute + (clicksPerMinute * variance));
					}
					clicksPerMinute = (clicksPerMinute < 1) ? 1 : clicksPerMinute; //should always be > 1
					if (timer > (60f / clicksPerMinute))
					{
						//choose random audioclip, without playing same one back to back
						int audioClipIndex = Random.Range(0, geigerClickClips.Count);
						audioClipIndex = (audioClipIndex == prevAudioClipIndex) ? audioClipIndex + 1 : audioClipIndex;
						audioClipIndex = (audioClipIndex >= geigerClickClips.Count) ? (geigerClickClips.Count - 1) : audioClipIndex;
						audioSource.PlayOneShot(geigerClickClips[audioClipIndex]);
						timer = 0;
						prevAudioClipIndex = audioClipIndex;
					}
				}
				intensity = 0;
			}
		}


		[SerializeField]
		List<AudioClip> geigerClickClipsSingle = new List<AudioClip>();

		[SerializeField]
		List<AudioClip> geigerClickClipsMulti = new List<AudioClip>();

		List<AudioClip> geigerClickClips = new List<AudioClip>();

		[SerializeField]
		[Tooltip("set to Prefabs/EmbodimentDiscrepancy/GeigerAudioSourcePrefab")]
		AudioSource geigerAudioSourcePrefab;

		[SerializeField]
		[Tooltip("only applies on Start(), set via property UseMultiClicks at runtime")]
		bool geigerUseMultiClicks = true;

		public bool GeigerUseMultiClicks
		{
			get{
				return geigerUseMultiClicks;
			}
			set
			{
				geigerUseMultiClicks = value;
				geigerClickClips.Clear();
				if (geigerUseMultiClicks){
					geigerClickClips.AddRange(geigerClickClipsSingle);
					geigerClickClips.AddRange(geigerClickClipsMulti);
				}
				else{
					geigerClickClips.AddRange(geigerClickClipsSingle);
				}
			}
		}

		[SerializeField]
		float geigerDistanceToMaxIntensity = 1;

		[SerializeField]
		int geigerMaxClicksPerMinute = 600;

		[SerializeField]
		[Range(0, 1)]
		float geigerVariance = 0.5f;

		Dictionary<TrackedJoint, AudioSource> geigerAudioSourceDict = new Dictionary<TrackedJoint, AudioSource>();
		Dictionary<TrackedJoint, GeigerClicker> geigerClickerDict = new Dictionary<TrackedJoint, GeigerClicker>();

		//where the audiosources for each joint should be
		public void SetTransformParentForJoint(TrackedJoint joint, GameObject obj)
		{
			if (!geigerAudioSourceDict.ContainsKey(joint)){
				geigerAudioSourceDict[joint] = Instantiate(original: geigerAudioSourcePrefab, parent: obj.transform);
			}
			else {
				geigerAudioSourceDict[joint].transform.position = obj.transform.position;
				geigerAudioSourceDict[joint].transform.SetParent(obj.transform);
			}
		}

		public void HandleGeigerSounds(Discrepancy disc)
		{
			if (!geigerClickerDict.ContainsKey(disc.joint)){
				geigerClickerDict[disc.joint] = new GeigerClicker { audioSource = geigerAudioSourceDict[disc.joint], geigerClickClips = this.geigerClickClips };
			}
			geigerClickerDict[disc.joint].intensity = Mathf.Lerp(0, 1, disc.distance / geigerDistanceToMaxIntensity);
		}

		void Start()
		{
			GeigerUseMultiClicks = geigerUseMultiClicks;
			if (geigerClickClips.Count < 1){
				Debug.LogError("no Geiger Click sounds set");
			}
		}

		void Update()
		{
			foreach (GeigerClicker geigerClicker in geigerClickerDict.Values){
				geigerClicker.DoClick(Time.deltaTime, geigerMaxClicksPerMinute, geigerVariance);
			}
		}
	}
}