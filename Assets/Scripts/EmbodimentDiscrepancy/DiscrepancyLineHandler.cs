using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EmbodimentDiscrepancy
{
	public class DiscrepancyLineHandler : MonoBehaviour
	{

		[SerializeField]
		[Tooltip("set to Prefabs/Embodiment/DiscrepancyLine prefab")]
		DiscrepancyLine DiscrepancyLinePrefab;

		Dictionary<TrackedJoint, DiscrepancyLine> lineDict = new Dictionary<TrackedJoint, DiscrepancyLine>();
		List<DiscrepancyLine> linesToDraw = new List<DiscrepancyLine>();

		[SerializeField]
		[Range(0.1f, 2)]
		float thicknessMultiplier = 1;


		public void DrawLine(Discrepancy disc){
			DrawLine(joint: disc.joint, source: disc.simulatedPos, target: disc.trackedPos, distance: disc.distance);
		}
		public void DrawLine(TrackedJoint joint, Transform source, Transform target, float distance)
		{
			if (!lineDict.ContainsKey(joint))
			{
				lineDict[joint] = Instantiate(DiscrepancyLinePrefab);
				lineDict[joint].transform.SetParent(this.transform);
			}
			linesToDraw.Add(lineDict[joint]);
			lineDict[joint].source = source;
			lineDict[joint].target = target;
			lineDict[joint].thickness = distance;
		}

		void Update()
		{
			foreach (KeyValuePair<TrackedJoint, DiscrepancyLine> entry in lineDict)
			{
				TrackedJoint joint = entry.Key;
				DiscrepancyLine line = entry.Value;
				if (linesToDraw.Contains(line))
				{
					line.DrawLine();
				}
				else
				{
					line.StopDrawLine();
				}
			}
			linesToDraw.Clear();
		}
	}
}