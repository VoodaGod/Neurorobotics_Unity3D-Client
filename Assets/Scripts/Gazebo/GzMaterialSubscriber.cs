using System;
using ROSBridgeLib;
using SimpleJSON;
using UnityEngine;

public class GzMaterialSubscriber : ROSBridgeSubscriber
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
        return "~/material";
    }

    public new static string GetMessageType()
    {
        return "gazebo.msgs.Material";
    }

    public new static ROSBridgeMsg ParseMessage(JSONNode msg)
    {
        return new GzMaterialMsg(msg);
    }

    public new static void CallBack(ROSBridgeMsg msg)
    {
        GzBridgeService.Instance.ReceiveMessage((GzMaterialMsg)msg);
    }

    #endregion //PUBLIC_METHODS

    #region PRIVATE_METHODS

    #endregion //PRIVATE_METHODS
}
