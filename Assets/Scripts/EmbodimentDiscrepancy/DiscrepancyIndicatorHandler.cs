using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EmbodimentDiscrepancy
{
	public class DiscrepancyIndicatorHandler : MonoBehaviour
	{
		public enum PointsAt
		{
			trackedPos,
			simulatedPos
		}

		[SerializeField]
		[Tooltip("set to Prefabs/EmbodimentDiscrepancy/DiscrepancyIndicatorPrefab")]
		DiscrepancyIndicator discrepancyIndicatorPrefab;

		[SerializeField]
		[Tooltip("set to Prefabs/EmbodimentDiscrepancy/DiscrepancyIndicatorCanvasPrefab")]
		Canvas discrepancyIndicatorCanvasPrefab;

		[SerializeField]
		[Tooltip("set to Camera (eye) (or the SteamVR_Cam)")]
		GameObject mainCamera;

		[SerializeField]
		PointsAt pointsAt = PointsAt.simulatedPos;

		Canvas discrepancyIndicatorCanvas;
		Dictionary<TrackedJoint, DiscrepancyIndicator> discrepancyIndicatorDict = new Dictionary<TrackedJoint, DiscrepancyIndicator>();


		public void HandleDiscrepancy(Discrepancy disc)
		{
			if (!discrepancyIndicatorDict.ContainsKey(disc.joint)){
				discrepancyIndicatorDict[disc.joint] = Instantiate(discrepancyIndicatorPrefab);
				discrepancyIndicatorDict[disc.joint].transform.SetParent(discrepancyIndicatorCanvas.transform, false);
			}

			if (pointsAt == PointsAt.simulatedPos){
				discrepancyIndicatorDict[disc.joint].PointAtTarget(disc.simulatedPos);
			}
			else if (pointsAt == PointsAt.trackedPos){
				discrepancyIndicatorDict[disc.joint].PointAtTarget(disc.trackedPos);
			}
		}

		// Use this for initialization
		void Start()
		{
			discrepancyIndicatorCanvas = Instantiate(discrepancyIndicatorCanvasPrefab);
			discrepancyIndicatorCanvas.transform.SetParent(mainCamera.transform, false);

		}

		// Update is called once per frame
		void Update()
		{

		}
	}
}