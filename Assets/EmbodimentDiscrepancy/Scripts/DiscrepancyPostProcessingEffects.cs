using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace EmbodimentDiscrepancy
{
	public class DiscrepancyPostProcessingEffects : MonoBehaviour
	{
		[SerializeField]
		[Tooltip("set to /Packages/Post Processing/PostProcessing/PostProcessResources")]
		PostProcessResources postProcessResources;

		[SerializeField]
		[Tooltip("set to DiscrepancyPostProcessProfile")]
		PostProcessProfile postProcessProfileAsset;

		[SerializeField]
		[Tooltip("set to Prefabs/PostProcessVolumePrefab")]
		PostProcessVolume postProcessVolumePrefab;

		[SerializeField]
		[Tooltip("set to Camera (eye) in scene")]
		Camera mainCam;

		[SerializeField]
		[Tooltip("layer for postprocessing objects, must exist")]
		string postProcessLayerName = "PostProcessing";

		[SerializeField]
		[Tooltip("distance at which all colors will have been shifted through")]
		float distToFullColorShift = 2;
		
		[SerializeField]
		[Range(0, 350)]
		[Tooltip("lower this to shift through fewer colors")]
		int maxShiftDegrees = 300;

		[SerializeField]
		[Tooltip("time over which to smooth sudden jump from 0 after tolerance time is reached")]
		float smoothingTime = 1;


		PostProcessVolume postProcessVolume;

		ColorGrading colorGradingSetting;
		float curMaxDiscrepancyDistance = 0; //store the highest distance of all discrepancies, reset after frame
		float prevMaxDiscrepancyDistance = 0;
		bool isSmoothing = false;


		public void HandleColorShift(Discrepancy disc)
		{
			//effect will be based on largest discrepancy -> store the highest distance
			if (disc.distance > curMaxDiscrepancyDistance){
				curMaxDiscrepancyDistance = disc.distance;
			}
		}

		void Start()
		{
			mainCam.gameObject.layer = LayerMask.NameToLayer(postProcessLayerName);

			PostProcessLayer postProcessLayer = mainCam.gameObject.GetComponent<PostProcessLayer>();
			if (postProcessLayer == null){
				postProcessLayer = mainCam.gameObject.AddComponent<PostProcessLayer>();
				postProcessLayer.Init(postProcessResources);
			}
			postProcessLayer.volumeTrigger = mainCam.transform;
			postProcessLayer.volumeLayer |= 1 << LayerMask.NameToLayer(postProcessLayerName);

			postProcessVolume = Instantiate(postProcessVolumePrefab);
			postProcessVolume.transform.parent = this.transform;
			postProcessVolume.gameObject.layer = LayerMask.NameToLayer(postProcessLayerName);
			postProcessVolume.profile = postProcessProfileAsset;

			bool gotSetting = false;
			gotSetting = postProcessVolume.profile.TryGetSettings<ColorGrading>(out colorGradingSetting);
			if (!gotSetting){
				Debug.LogError("no colorgrading setting found");
			}
			SetColorShift(0);
		}

		IEnumerator ResetAfterFrame(){
			yield return new WaitForEndOfFrame();
			curMaxDiscrepancyDistance = 0;
		}


		void SetColorShift(float distance)
		{
			//hueshift goes from -180 to 180, 0 is neutral, 180 wraps to -180.
			float newShift = Mathf.Lerp(0, maxShiftDegrees, distance / distToFullColorShift);
			if (newShift > 180){
				newShift = -180 + (newShift - 180);
			}
			colorGradingSetting.hueShift.overrideState = true;
			colorGradingSetting.hueShift.value = newShift;
		}

		IEnumerator SmoothAdjust(float time)
		{
			isSmoothing = true;
			while (time > 0)
			{
				float smoothedDistance = Mathf.Lerp(curMaxDiscrepancyDistance, 0, time -= Time.deltaTime);
				SetColorShift(smoothedDistance);
				yield return null;
			}
			isSmoothing = false;
		}

		//handle everything after all Updates done, to make sure Handle*() functions called before
		void LateUpdate()
		{
			if (curMaxDiscrepancyDistance > 0)
			{	
				//smooth out initial distance jump after tolerance timer reached
				if (prevMaxDiscrepancyDistance == 0	 && !isSmoothing){
					StartCoroutine(SmoothAdjust(smoothingTime));
				}
				if (!isSmoothing){ //after smoothing set directly
					SetColorShift(curMaxDiscrepancyDistance);
				}
			}
			prevMaxDiscrepancyDistance = curMaxDiscrepancyDistance;
			
			StartCoroutine(ResetAfterFrame());
		}
	}
}