using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscrepancyLineHandler : MonoBehaviour
{

	[SerializeField]
	[Tooltip("set to Prefabs/Embodiment/DiscrepancyLine prefab")]
	DiscrepancyLine DiscrepancyLinePrefab;

	Dictionary<DiscrepancyTracker.TrackedJoints, DiscrepancyLine> lineDict = new Dictionary<DiscrepancyTracker.TrackedJoints, DiscrepancyLine>();
	List<DiscrepancyLine> linesToDraw = new List<DiscrepancyLine>();

	[SerializeField]
	[Range(0.1f, 2)]
	float thicknessMultiplier = 1;

	public void DrawLine(DiscrepancyTracker.TrackedJoints joint, Transform source, Transform target, float distance)
	{
		if (!lineDict.ContainsKey(joint)){
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
		foreach (KeyValuePair<DiscrepancyTracker.TrackedJoints, DiscrepancyLine> entry in lineDict)
		{
			DiscrepancyTracker.TrackedJoints joint = entry.Key;
			DiscrepancyLine line = entry.Value;
			if (linesToDraw.Contains(line)){
				line.DrawLine();
			}
			else {
				line.StopDrawLine();
			}
		}
		linesToDraw.Clear();
	}
}