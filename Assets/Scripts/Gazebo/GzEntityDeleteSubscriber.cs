using UnityEngine;
using ROSBridgeLib;
using SimpleJSON;

public class GzEntityDeleteSubscriber : ROSBridgeSubscriber
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
        return "~/entity_delete";
        //return "~/model/modify";
    }

    public new static string GetMessageType()
    {
        return "gazebo.msgs.entity_delete";
        //return "gazebo.msgs.Model";
    }

    public new static ROSBridgeMsg ParseMessage(JSONNode msg)
    {
        Debug.Log("GzEntityDeleteSubscriber.ParseMessage");
        return new GzEntityDeleteMsg(msg);
    }

    public new static void CallBack(ROSBridgeMsg msg)
    {
        //GzBridgeService.Instance.ReceiveMessage((GzEntityDeleteMsg)msg);
    }

    #endregion //PUBLIC_METHODS

    #region PRIVATE_METHODS

    #endregion //PRIVATE_METHODS
}
