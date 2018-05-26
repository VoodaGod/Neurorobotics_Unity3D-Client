using System;
using ROSBridgeLib;
using SimpleJSON;
using UnityEngine;

public class GzModelInfoSubscriber : ROSBridgeSubscriber
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
        return "~/model/info";
    }

    public new static string GetMessageType()
    {
        return "gazebo.msgs.Model";
    }

    public new static ROSBridgeMsg ParseMessage(JSONNode msg)
    {
        return new GzModelInfoMsg(msg);
    }

    public new static void CallBack(ROSBridgeMsg msg)
    {
        GzBridgeService.Instance.ReceiveMessage((GzModelInfoMsg)msg);
    }

    #endregion //PUBLIC_METHODS

    #region PRIVATE_METHODS

    #endregion //PRIVATE_METHODS
}
