using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamVRControllerInput : MonoBehaviour {

    private SteamVR_TrackedObject trackedObject;
    private SteamVR_Controller.Device device;

    public TrackedObjectAssigner trackedObjectAssigner;
    
	void Start () {
        trackedObject = GetComponent<SteamVR_TrackedObject>();

    }
	
	void Update () {
        device = SteamVR_Controller.Input((int)trackedObject.index);

        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
        {
            Debug.Log("Grip");
            //UserAvatarService.Instance.SpawnYBot();
            trackedObjectAssigner.AssignTrackersToRoles();
        }

    }
}
