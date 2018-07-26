using System.Collections.Generic;
using UnityEngine;

public class MyoArmJointOrientation : MonoBehaviour
{
    private GameObject myo_ = null;
    
    private Quaternion _antiYaw = Quaternion.identity;
    
    private float _referenceRoll = 0.0f;

    private Queue<Quaternion> rotations = new Queue<Quaternion>();

    private void Start()
    {
    }

    // Update is called once per frame.
    void Update()
    {
        if (!this.myo_) return;

        this.UpdateOrientationSlerp();
    }

    public void UpdateReference(GameObject myo)
    {
        this.myo_ = myo;
        
        if (this.myo_)
        {
            _antiYaw = Quaternion.FromToRotation(
                new Vector3(myo.transform.forward.x, 0, myo.transform.forward.z),
                new Vector3(0, 0, 1)
            );

            Vector3 referenceZeroRoll = computeZeroRollVector(myo.transform.forward);
            _referenceRoll = rollFromZero(referenceZeroRoll, myo.transform.forward, myo.transform.up);
        }
    }
    
    float rollFromZero(Vector3 zeroRoll, Vector3 forward, Vector3 up)
    {
        float cosine = Vector3.Dot(up, zeroRoll);
        
        Vector3 cp = Vector3.Cross(up, zeroRoll);
        float directionCosine = Vector3.Dot(forward, cp);
        float sign = directionCosine < 0.0f ? 1.0f : -1.0f;
        
        return sign * Mathf.Rad2Deg * Mathf.Acos(cosine);
    }
    
    Vector3 computeZeroRollVector(Vector3 forward)
    {
        Vector3 antigravity = Vector3.up;
        Vector3 m = Vector3.Cross(this.myo_.transform.forward, antigravity);
        Vector3 roll = Vector3.Cross(m, this.myo_.transform.forward);

        return roll.normalized;
    }
    
    float normalizeAngle(float angle)
    {
        if (angle > 180.0f)
        {
            return angle - 360.0f;
        }
        if (angle < -180.0f)
        {
            return angle + 360.0f;
        }
        return angle;
    }

    private void UpdateOrientation()
    {
        ThalmicMyo thalmic_myo = this.myo_.GetComponent<ThalmicMyo>();

        Vector3 zeroRoll = computeZeroRollVector(this.myo_.transform.forward);
        float roll = rollFromZero(zeroRoll, this.myo_.transform.forward, this.myo_.transform.up);

        float relativeRoll = normalizeAngle(roll - _referenceRoll);

        Quaternion antiRoll = Quaternion.AngleAxis(relativeRoll, this.myo_.transform.forward);

        transform.rotation = _antiYaw * antiRoll * Quaternion.LookRotation(this.myo_.transform.forward);
        transform.rotation = new Quaternion(transform.rotation.z, transform.rotation.y, -transform.rotation.x, transform.rotation.w);  // black magic, figure out later

        // the rotation needs to be updated to compensate.
        if (thalmic_myo.xDirection == Thalmic.Myo.XDirection.TowardWrist)
        {
            transform.rotation = new Quaternion(transform.localRotation.x,
                                                -transform.localRotation.y,
                                                transform.localRotation.z,
                                                -transform.localRotation.w);
        }
    }

    private void UpdateOrientationSmoothed()
    {
        ThalmicMyo thalmic_myo = this.myo_.GetComponent<ThalmicMyo>();

        Vector3 zeroRoll = computeZeroRollVector(this.myo_.transform.forward);
        float roll = rollFromZero(zeroRoll, this.myo_.transform.forward, this.myo_.transform.up);

        float relativeRoll = normalizeAngle(roll - _referenceRoll);

        Quaternion antiRoll = Quaternion.AngleAxis(relativeRoll, this.myo_.transform.forward);

        rotations.Enqueue(_antiYaw * antiRoll * Quaternion.LookRotation(this.myo_.transform.forward));
        while (rotations.Count > 8)
        {
            rotations.Dequeue();
        }
        transform.rotation = QuaternionUtility.GetAverage(rotations.ToArray());
        transform.rotation = new Quaternion(transform.rotation.z, transform.rotation.y, -transform.rotation.x, transform.rotation.w);  // black magic, figure out later

        // the rotation needs to be updated to compensate.
        if (thalmic_myo.xDirection == Thalmic.Myo.XDirection.TowardWrist)
        {
            transform.rotation = new Quaternion(transform.localRotation.x,
                                                -transform.localRotation.y,
                                                transform.localRotation.z,
                                                -transform.localRotation.w);
        }
    }

    private void UpdateOrientationSlerp()
    {
        ThalmicMyo thalmic_myo = this.myo_.GetComponent<ThalmicMyo>();

        Vector3 zeroRoll = computeZeroRollVector(this.myo_.transform.forward);
        float roll = rollFromZero(zeroRoll, this.myo_.transform.forward, this.myo_.transform.up);

        float relativeRoll = normalizeAngle(roll - _referenceRoll);

        Quaternion antiRoll = Quaternion.AngleAxis(relativeRoll, this.myo_.transform.forward);

        rotations.Enqueue(_antiYaw * antiRoll * Quaternion.LookRotation(this.myo_.transform.forward));
        while (rotations.Count > 2)
        {
            rotations.Dequeue();
        }
        Quaternion[] rotations_array = rotations.ToArray();
        transform.rotation = Quaternion.Slerp(rotations_array[0], rotations_array[1], 0.5f);
        transform.rotation = new Quaternion(transform.rotation.z, transform.rotation.y, -transform.rotation.x, transform.rotation.w);  // black magic, figure out later

        // the rotation needs to be updated to compensate.
        if (thalmic_myo.xDirection == Thalmic.Myo.XDirection.TowardWrist)
        {
            transform.rotation = new Quaternion(transform.localRotation.x,
                                                -transform.localRotation.y,
                                                transform.localRotation.z,
                                                -transform.localRotation.w);
        }
    }
}
