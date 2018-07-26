using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyoArmCalibration : MonoBehaviour {

    public GameObject myo1, myo2;
    public GameObject joint_shoulder, joint_elbow, joint_wrist;
    public Transform hmd;

    //public float neck_length = 0.2f, shoulder_to_shoulder_width = 0.4f, shoulder_forward = 0.0f;
    //public GameObject tracker_shoulder;
    //public float shoulder2tracker_offset = -0.05f;
    //public float arm_length = 0.6f;
    public float accelerometer_sum_tap_threshold = ACCELEROMETER_SUM_TAP_THRESHOLD;

    public Vector3 bodyHeadOffset = new Vector3(0, -0.65f, 0);

    private GameObject myo_upper_arm_ = null, myo_lower_arm_ = null;
    private bool calibrate = true;

    private List<Vector4> accelerometer_history_myo1 = new List<Vector4>();
    private List<Vector4> accelerometer_history_myo2 = new List<Vector4>();
    private const float ACCELEROMETER_HISTORY_TIME_WINDOW = 0.1f;
    private const float ACCELEROMETER_SUM_TAP_THRESHOLD = 40f;

    //private Vector3 shoulder_offset_ = new Vector3(0, 0, -10);
    private Vector3 shoulder_pos_ = new Vector3(0, 0, -10);

    public ThalmicMyo thalmicmyo_upper_arm
    {
        get { return myo_upper_arm_.GetComponent<ThalmicMyo>(); }
    }

    public GameObject myo_lower_arm
    {
        get { return myo_lower_arm_; }
    }
    
    void Start () {
        // unlock Myos
        if (myo1) myo1.GetComponent<ThalmicMyo>().Unlock(Thalmic.Myo.UnlockType.Hold);
        if (myo2) myo2.GetComponent<ThalmicMyo>().Unlock(Thalmic.Myo.UnlockType.Hold);

        // set everything transparent
        //this.SetArmAlpha(0.5f);
        //StartCoroutine(SetStretchArmActive(false, 0f));

        //this.transform.localPosition = new Vector3(0, 0, this.shoulder2tracker_offset);
    }
	
	void Update ()
    {
        //this.SetArmToShoulderTrackerPosition();
        //TODO: extract from IKControls and here to separate script positioning whole avatar
        this.transform.position = hmd.position + bodyHeadOffset;
    

        //joint_shoulder.transform.position = hmd.transform.position + shoulder_offset_;
        if (this.calibrate)
        {
            //this.SetArmRotationHorizontally();

            // check for taps
            if (myo1 && myo2)
            {
                this.CalibrateMyoPositionsOnArmViaPat();
            }
        }

        // re-enable calibration
        if (Input.GetKeyDown("f1"))
        {
            //this.SetArmAlpha(0.5f);
            Debug.Log("f1");

            joint_shoulder.GetComponent<MyoArmJointOrientation>().UpdateReference(null);
            joint_elbow.GetComponent<MyoArmJointOrientation>().UpdateReference(null);

            joint_shoulder.transform.rotation = new Quaternion();
            joint_elbow.transform.rotation = new Quaternion();

            this.calibrate = true;
        }

        // calibrate shoulder offset
        /*if (Input.GetKeyDown("f2"))
        {
            Debug.Log("f2 shoulder set");
            //shoulder_offset_ = tracker_shoulder.transform.position - hmd.transform.position;
            //shoulder_offset_.y += shoulder2tracker_y_offset;
            joint_shoulder.transform.position = new Vector3(tracker_shoulder.transform.position.x, 
                tracker_shoulder.transform.position.y + shoulder2tracker_offset, 
                tracker_shoulder.transform.position.z);

            Vector3 elbow_local_pos = joint_elbow.transform.localPosition;
            elbow_local_pos.z = this.GetArmSegmentLength();
            Vector3 wrist_local_pos = joint_wrist.transform.localPosition;
            wrist_local_pos.z = this.GetArmSegmentLength();
        }*/

        // calibrate shoulder offset
        /*if (Input.GetKeyDown("s"))
        {
            Debug.Log("setting stretchy arm active");
            StartCoroutine(SetStretchArmActive(true, 0.5f));
        }*/
    }

    void OnDrawGizmos()
    {

    }

    /*public float GetArmSegmentLength()
    {
        return arm_length / 2.0f;
    }*/

    /*IEnumerator SetStretchArmActive (bool active, float wait_time)
    {
        yield return new WaitForSeconds(wait_time);

        MyoStretchArm stretch_hand = this.GetComponent<MyoStretchArm>();
        if (!stretch_hand)
        {
            stretch_hand = this.GetComponentInChildren<MyoStretchArm>();
        }

        if (stretch_hand)
        {
            stretch_hand.enabled = active;
        }
    }*/

    /*private void SetArmAlpha (float alpha)
    {
        Renderer[] renderers = this.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; i = i + 1)
        {
            Color color = renderers[i].material.color;
            color.a = alpha;
            renderers[i].material.color = color;
        }
    }*/

    /*private void SetArmRotationHorizontally()
    {
        Quaternion local_delta = Quaternion.FromToRotation(Vector3.forward, Quaternion.Inverse(this.tracker_shoulder.transform.rotation) * Vector3.forward);
        this.joint_shoulder.transform.localRotation = local_delta;
    }*/

    private void CalibrateMyoPositionsOnArmViaPat()
    {
        this.UpdateMyoAccelerometerHistories();

        bool placement_calibrated = false;
        if (this.DetectMyoTap(this.accelerometer_history_myo1))
        {
            //myo1.GetComponent<ThalmicMyo>().Vibrate(Thalmic.Myo.VibrationType.Medium);
            this.myo_lower_arm_ = this.myo1;
            this.myo_upper_arm_ = this.myo2;
            placement_calibrated = true;
            Debug.Log("lower arm: myo1, upper arm: myo2");
        }
        else if (DetectMyoTap(this.accelerometer_history_myo2))
        {
            // myo2.GetComponent<ThalmicMyo>().Vibrate(Thalmic.Myo.VibrationType.Medium);
            this.myo_lower_arm_ = this.myo2;
            this.myo_upper_arm_ = this.myo1;
            placement_calibrated = true;
            Debug.Log("lower arm: myo2, upper arm: myo1");
        }

        if (placement_calibrated)
        {
            //Debug.Log("Myo Arm placement calibrated");

            if (joint_shoulder && joint_elbow)
            {
                joint_shoulder.GetComponent<MyoArmJointOrientation>().UpdateReference(myo_upper_arm_);
                joint_elbow.GetComponent<MyoArmJointOrientation>().UpdateReference(myo_lower_arm_);
                Debug.Log("Myo references and reference poses updated");

                this.calibrate = false;
                //StartCoroutine(SetStretchArmActive(true, 0.5f));

                //this.SetArmAlpha(1.0f);
            }
        }
    }

    private bool DetectMyoTap(List<Vector4> history)
    {
        float accelerometer_sum = 0;
        foreach (Vector4 data_point in history)
        {
            accelerometer_sum += Mathf.Abs(history[0][1]) + Mathf.Abs(history[0][2]) + Mathf.Abs(history[0][3]);
        }

        if (accelerometer_sum > this.accelerometer_sum_tap_threshold)
        {
            return true;
        }

        return false;
    }

    private void UpdateMyoAccelerometerHistories()
    {
        float current_time = Time.time;
        ThalmicMyo thalmic_myo_1 = myo1.GetComponent<ThalmicMyo>();
        ThalmicMyo thalmic_myo_2 = myo2.GetComponent<ThalmicMyo>();
        accelerometer_history_myo1.Add(new Vector4(current_time, thalmic_myo_1.accelerometer.x, thalmic_myo_1.accelerometer.y, thalmic_myo_1.accelerometer.z));
        accelerometer_history_myo2.Add(new Vector4(current_time, thalmic_myo_2.accelerometer.x, thalmic_myo_2.accelerometer.y, thalmic_myo_2.accelerometer.z));

        // clean out history
        while (accelerometer_history_myo1[0][0] < current_time - ACCELEROMETER_HISTORY_TIME_WINDOW)
        {
            accelerometer_history_myo1.Remove(accelerometer_history_myo1[0]);
        }
        while (accelerometer_history_myo2[0][0] < current_time - ACCELEROMETER_HISTORY_TIME_WINDOW)
        {
            accelerometer_history_myo2.Remove(accelerometer_history_myo2[0]);
        }
    }

    /*private void CalibrateMyoPositionViaDoubleTap()
    {
        bool placement_calibrated = false;
        if (myo1.GetComponent<ThalmicMyo>().pose == Thalmic.Myo.Pose.DoubleTap)
        {
            myo_lower_arm_ = myo1;
            myo_upper_arm_ = myo2;
            placement_calibrated = true;
        }
        else if (myo2.GetComponent<ThalmicMyo>().pose == Thalmic.Myo.Pose.DoubleTap)
        {
            myo_lower_arm_ = myo2;
            myo_upper_arm_ = myo1;
            placement_calibrated = true;
        }

        if (placement_calibrated)
        {
            //Debug.Log("Myo Arm placement calibrated");

            if (joint_shoulder && joint_elbow)
            {
                joint_shoulder.GetComponent<MyoArmJointOrientation>().UpdateReference(myo_upper_arm_);
                joint_elbow.GetComponent<MyoArmJointOrientation>().UpdateReference(myo_lower_arm_);
                //Debug.Log("Myo references and reference poses updated");

                this.calibrate = false;
                StartCoroutine(SetStretchArmActive(true, 0.5f));

                this.SetArmAlpha(1.0f);
            }
        }
    }*/

    /*private void SetArmToShoulderTrackerPosition()
    {
        Vector3 delta_vec = Quaternion.Inverse(this.tracker_shoulder.transform.rotation) * new Vector3(0, 0, this.shoulder2tracker_offset);
        this.transform.position = this.tracker_shoulder.transform.position + delta_vec;
    }*/
}
