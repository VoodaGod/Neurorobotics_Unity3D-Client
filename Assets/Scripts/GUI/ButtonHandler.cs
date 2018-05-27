using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHandler : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnButtonSimulationStart()
    {
        SimulationControlService.Instance.SetState(SimulationControlService.STATE_STARTED);
    }

    public void OnButtonSimulationPause()
    {
        SimulationControlService.Instance.SetState(SimulationControlService.STATE_PAUSED);
    }
}
