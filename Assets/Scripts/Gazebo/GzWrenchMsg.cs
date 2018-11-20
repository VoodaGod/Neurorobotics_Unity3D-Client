using System.Collections.Generic;
using UnityEngine;
using ROSBridgeLib;
using ROSBridgeLib.geometry_msgs;
using SimpleJSON;

public class GzWrenchMsg : ROSBridgeMsg
{
    #region PRIVATE_MEMBER_VARIABLES

    private Vector3Msg _force;
    private Vector3Msg _torque;

    #endregion //PRIVATE_MEMBER_VARIABLES


    #region PUBLIC_METHODS

    public GzWrenchMsg(JSONNode msg)
    {
        _force = new Vector3Msg(msg["force"]);
        _torque = new Vector3Msg(msg["torque"]);
    }

    public GzWrenchMsg(Vector3Msg force, Vector3Msg torque)
    {
        _force = force;
        _torque = torque;
    }

    public static string GetMessageType()
    {
        return "gazebo.msgs.Wrench";
    }

    public Vector3Msg GetForce()
    {
        return _force;
    }

    public Vector3Msg GetTorque()
    {
        return _torque;
    }

    public override string ToString()
    {
        return "Wrench [force=" + _force.ToString() + ",  torque=" + _torque.ToString() + "]";
    }

    public override string ToYAMLString()
    {
        return "{\"force\" : " + _force.ToYAMLString() + ", \"torque\" : " + _torque.ToYAMLString() + "}";
    }

    #endregion //PUBLIC_METHODS
}
