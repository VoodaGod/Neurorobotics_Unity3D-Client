﻿using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using ROSBridgeLib;

public class GzPoseInfoMsg : ROSBridgeMsg
{

    #region PUBLIC_MEMBER_VARIABLES

    public JSONNode MsgJSON
    {
        get
        {
            return this.msg_json_;
        }

        set
        {
            this.msg_json_ = value;
        }
    }

    #endregion //PUBLIC_MEMBER_VARIABLES

    #region PRIVATE_MEMBER_VARIABLES

    private JSONNode msg_json_;

    #endregion //PRIVATE_MEMBER_VARIABLES

    #region MONOBEHAVIOR_METHODS
    #endregion //MONOBEHAVIOR_METHODS

    #region PUBLIC_METHODS

    public GzPoseInfoMsg(JSONNode msg)
    {
        this.msg_json_ = msg;
    }

    public static string GetMessageType()
    {
        return "gazebo.msgs.Pose";
    }

    public override string ToString()
    {
        return "gazebo.msgs.Pose [name =";
    }

    public override string ToYAMLString()
    {
        return "{}";
    }

    #endregion //PUBLIC_METHODS

    #region PRIVATE_METHODS
    #endregion //PRIVATE_METHODS
}
