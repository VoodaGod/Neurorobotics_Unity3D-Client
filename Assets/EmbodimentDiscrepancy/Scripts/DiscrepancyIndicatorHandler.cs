using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EmbodimentDiscrepancy
{
	public class DiscrepancyIndicatorHandler : MonoBehaviour
	{
		enum PointsAt
		{
			trackedPos,
			simulatedPos
		}

		[SerializeField]
		[Tooltip("where the indicators should point to")]
		PointsAt pointsAt = PointsAt.simulatedPos;

		[SerializeField]
		[Tooltip("set to Prefabs/DiscrepancyIndicatorPrefab")]
		DiscrepancyIndicator discrepancyIndicatorPrefab;

		[SerializeField]
		[Tooltip("set to Prefabs/DiscrepancyIndicatorCanvasPrefab")]
		Canvas discrepancyIndicatorCanvasPrefab;

		[SerializeField]
		[Tooltip("set to main Camera (probably Camera (eye)), indicator canvas will be added as child to this")]
		GameObject mainCamera;

		Canvas discrepancyIndicatorCanvas;
		Dictionary<TrackedJoint, DiscrepancyIndicator> discrepancyIndicatorDict = new Dictionary<TrackedJoint, DiscrepancyIndicator>();


		public void HandleDiscrepancy(Discrepancy disc)
		{
			if (!discrepancyIndicatorDict.ContainsKey(disc.joint)){
				DiscrepancyIndicator discrepancyIndicator = Instantiate(discrepancyIndicatorPrefab);
				discrepancyIndicator.transform.SetParent(discrepancyIndicatorCanvas.transform, false);
				discrepancyIndicator.cam = mainCamera.transform;
				discrepancyIndicatorDict[disc.joint] = discrepancyIndicator;
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