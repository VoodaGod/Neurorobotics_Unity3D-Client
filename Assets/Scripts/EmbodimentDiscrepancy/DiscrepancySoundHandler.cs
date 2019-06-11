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
		[Tooltip("set to Prefabs/EmbodimentDiscrepancy/AudioSourcePrefab")]
		AudioSource audioSourcePrefab;

		[SerializeField]
		[Tooltip("only applies on Start(), set via property UseMultiClicks at runtime")]
		bool useMultiClicks = true;

		public bool UseMultiClicks
		{
			get{
				return useMultiClicks;
			}
			set
			{
				useMultiClicks = value;
				geigerClickClips.Clear();
				if (useMultiClicks){
					geigerClickClips.AddRange(geigerClickClipsSingle);
					geigerClickClips.AddRange(geigerClickClipsMulti);
				}
				else{
					geigerClickClips.AddRange(geigerClickClipsSingle);
				}
			}
		}

		[SerializeField]
		float distanceToMaxIntensity = 1;

		[SerializeField]
		int maxClicksPerMinute = 600;

		[SerializeField]
		[Range(0, 1)]
		float variance = 0.5f;

		Dictionary<TrackedJoint, AudioSource> audioSourceDict = new Dictionary<TrackedJoint, AudioSource>();
		Dictionary<TrackedJoint, GeigerClicker> geigerClickerDict = new Dictionary<TrackedJoint, GeigerClicker>();

		//where the audiosources for each joint should be
		public void SetTransformParentForJoint(TrackedJoint joint, GameObject obj)
		{
			if (!audioSourceDict.ContainsKey(joint)){
				audioSourceDict[joint] = Instantiate(original: audioSourcePrefab, parent: obj.transform);
			}
			else {
				audioSourceDict[joint].transform.position = obj.transform.position;
				audioSourceDict[joint].transform.SetParent(obj.transform);
			}
		}

		public void HandleGeigerSounds(Discrepancy disc)
		{
			if (!geigerClickerDict.ContainsKey(disc.joint)){
				geigerClickerDict[disc.joint] = new GeigerClicker { audioSource = audioSourceDict[disc.joint], geigerClickClips = this.geigerClickClips };
			}
			geigerClickerDict[disc.joint].intensity = Mathf.Lerp(0, 1, disc.distance / distanceToMaxIntensity);
		}

		void Start()
		{
			UseMultiClicks = useMultiClicks;
			if (geigerClickClips.Count < 1){
				Debug.LogError("no Geiger Click sounds set");
			}
		}

		void Update()
		{
			foreach (GeigerClicker geigerClicker in geigerClickerDict.Values){
				geigerClicker.DoClick(Time.deltaTime, maxClicksPerMinute, variance);
			}
		}
	}
}