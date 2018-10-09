using ROSBridgeLib.geometry_msgs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class UserAvatarService : Singleton<UserAvatarService>
{
    public GameObject avatar
    {
        get { return this.user_avatar; }
    }

    public GameObject avatar_visuals = null;

    private string avatar_name = null;

    private GameObject user_avatar = null;

    void Awake()
    {
        GzBridgeService.Instance.AddCallbackModelInfoMsg(this.OnModelInfoMsg);
        GzBridgeService.Instance.AddCallbackOnCloseConnection(this.DespawnAvatar);
    }
    
    void Start()
    {
        StartCoroutine(SpawnAvatar("user_avatar_ybot"));
    }
    
    void Update()
    {
        if (!this.user_avatar)
        {
            return;
        }

        string topic = "/" + this.avatar_name + "/user_avatar_ybot/mixamorig_Head/cmd_pose";

        Vector3 gazebo_head_position_target = GazeboSceneManager.Unity2GzVec3(this.avatar_visuals.transform.position);
        PointMsg position_msg = new PointMsg(gazebo_head_position_target.x, gazebo_head_position_target.y, gazebo_head_position_target.z);

        Quaternion gazebo_head_rotation_target = GazeboSceneManager.Unity2GzQuaternion(this.avatar_visuals.transform.rotation);
        QuaternionMsg rotation_msg = new QuaternionMsg(gazebo_head_rotation_target.x, gazebo_head_rotation_target.y, gazebo_head_rotation_target.z, gazebo_head_rotation_target.w);
        PoseMsg pose = new PoseMsg(position_msg, rotation_msg);
        //ROSBridgeService.Instance.websocket.Publish(String topic, ROSBridgeMsg msg);
    }

    public void DespawnAvatar()
    {
        if (this.user_avatar == null)
        {
            return;
        }

        Debug.Log("Despawning: " + this.avatar_name);
        GzEntityDeleteMsg msg = new GzEntityDeleteMsg(this.avatar_name);

        GzBridgeService.Instance.gzbridge.Publish(GzEntityDeletePublisher.GetMessageTopic(), msg);
    }

    public void OnModelInfoMsg(GzModelInfoMsg model_info_msg)
    {
        JSONNode json_model_info = model_info_msg.MsgJSON;
        string model_name = json_model_info["name"];
        if (model_name.Contains(this.avatar_name))
        {
            StartCoroutine(this.WaitForAvatarCreation());
        }
    }

    private IEnumerator SpawnAvatar(string avatar_model_name)
    {
        // wait until authentication token available
        yield return new WaitUntil(() => !string.IsNullOrEmpty(AuthenticationService.Instance.token));

        this.avatar_name = "user_avatar_" + AuthenticationService.Instance.token;
        Vector3 spawn_pos = GazeboSceneManager.Unity2GzVec3(new Vector3(avatar_visuals.transform.position.x, avatar_visuals.transform.position.y + 1.5f, avatar_visuals.transform.position.z));
        Quaternion spawn_rot = new Quaternion();

        GzFactoryMsg msg = new GzFactoryMsg(this.avatar_name, avatar_model_name, new PointMsg(spawn_pos.x, spawn_pos.y, spawn_pos.z), new QuaternionMsg(spawn_rot.x, spawn_rot.y, spawn_rot.z, spawn_rot.w));

        GzBridgeService.Instance.gzbridge.Publish(GzFactoryPublisher.GetMessageTopic(), msg);
    }

    private IEnumerator WaitForAvatarCreation()
    {
        yield return new WaitUntil(() => {
            this.user_avatar = GameObject.Find(this.avatar_name);
            return this.user_avatar != null;
            }
        );
    }
}
