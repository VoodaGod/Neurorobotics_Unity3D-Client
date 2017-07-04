using System;
using ROSBridgeLib;
using SimpleJSON;
using UnityEngine;

public class GzSceneTopicSubscriber : ROSBridgeSubscriber
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
        return "~/scene";
    }

    public new static string GetMessageType()
    {
        return "gazebo.msgs.Scene";
    }

    public new static ROSBridgeMsg ParseMessage(JSONNode msg)
    {
        Debug.Log("GzSceneTopicSubscriber.ParseMessage()");
        return new GzSceneMsg(msg);
    }

    public new static void CallBack(ROSBridgeMsg msg)
    {
        Debug.Log("GzSceneTopicSubscriber.CallBack()");
        //GzSceneMsg scene_msg = (GzSceneMsg)msg;
        
        GzBridgeManager.Instance.ReceiveMessage((GzSceneMsg)msg);
    }

    #endregion //PUBLIC_METHODS

    #region PRIVATE_METHODS

    #endregion //PRIVATE_METHODS
}
