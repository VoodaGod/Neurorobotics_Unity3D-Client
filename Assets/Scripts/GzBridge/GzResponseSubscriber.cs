using System;
using ROSBridgeLib;
using SimpleJSON;
using UnityEngine;

public class GzResponseSubscriber : ROSBridgeSubscriber
{
    #region PUBLIC_MEMBER_VARIABLES
    #endregion //PUBLIC_MEMBER_VARIABLES

    #region PRIVATE_MEMBER_VARIABLES
    #endregion //PRIVATE_MEMBER_VARIABLES

    #region MONOBEHAVIOR_METHODS
    #endregion //MONOBEHAVIOR_METHODS

    #region PUBLIC_METHODS

    public new static string GetMessageTopic()
    {
        return "~/response";
    }

    public new static string GetMessageType()
    {
        return "gazebo/msgs/response";
    }

    public new static ROSBridgeMsg ParseMessage(JSONNode msg)
    {
        Debug.Log("ParseMessage:\n" + msg);
        return new RoboyPoseMsg(msg);
    }

    public new static void CallBack(ROSBridgeMsg msg)
    {
        Debug.Log("CallBack:\n" + msg);
        RoboyPoseMsg pose = (RoboyPoseMsg)msg;
        //RoboyManager.Instance.ReceiveMessage(pose);
    }

    #endregion //PUBLIC_METHODS

    #region PRIVATE_METHODS
    #endregion //PRIVATE_METHODS
}
