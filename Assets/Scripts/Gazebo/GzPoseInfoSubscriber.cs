using System;
using ROSBridgeLib;
using SimpleJSON;
using UnityEngine;

public class GzPoseInfoSubscriber : ROSBridgeSubscriber
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
        return "~/pose/info";
    }

    public new static string GetMessageType()
    {
        return "gazebo.msgs.Pose";
    }

    public new static ROSBridgeMsg ParseMessage(JSONNode msg)
    {
        //Debug.Log("GzSceneTopicSubscriber.ParseMessage()");
        return new GzPoseInfoMsg(msg);
    }

    public new static void CallBack(ROSBridgeMsg msg)
    {
        //Debug.Log("GzSceneTopicSubscriber.CallBack()");
        GzBridgeManager.Instance.ReceiveMessage((GzPoseInfoMsg)msg);
    }

    #endregion //PUBLIC_METHODS

    #region PRIVATE_METHODS

    #endregion //PRIVATE_METHODS
}
