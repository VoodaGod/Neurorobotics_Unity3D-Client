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

    public List<GameObject> published_links = null;

    public bool scripted_publishing = false;

    private string avatar_name = null;

    private GameObject user_avatar = null;
    private GameObject avatar_target = null;

    private bool spawning_avatar = false;

    private float publish_linear_velocity_frequency = 0.05f;
    private float publish_linear_velocity_tlast;
    private float publish_linear_velocity_velocity_const = 5f;
    private float publish_linear_velocity_velocity_factor = 30f;
    private float publish_linear_velocity_max_velocity = 20f;

    void Awake()
    {
        GzBridgeService.Instance.AddCallbackModelInfoMsg(this.OnModelInfoMsg);
        GzBridgeService.Instance.AddCallbackOnCloseConnection(this.DespawnAvatar);
    }
    
    void Start()
    {
        //StartCoroutine(SpawnAvatar("user_avatar_ybot"));
    }
    
    void Update()
    {
        /*if (this.user_avatar && this.avatar_visuals)
        {
            string topic = "/" + this.avatar_name + "/avatar_ybot/mixamorig_Head/cmd_pose";

            Vector3 gazebo_head_position_target = GazeboSceneManager.Unity2GzVec3(this.avatar_visuals.transform.position);
            PointMsg position_msg = new PointMsg(gazebo_head_position_target.x, gazebo_head_position_target.y, gazebo_head_position_target.z);

            Quaternion gazebo_head_rotation_target = GazeboSceneManager.Unity2GzQuaternion(this.avatar_visuals.transform.rotation);
            QuaternionMsg rotation_msg = new QuaternionMsg(gazebo_head_rotation_target.x, gazebo_head_rotation_target.y, gazebo_head_rotation_target.z, gazebo_head_rotation_target.w);
            PoseMsg pose = new PoseMsg(position_msg, rotation_msg);
            ROSBridgeService.Instance.websocket.Publish(topic, pose);
        }*/

        if (Input.GetKey("s"))
        {
            StartCoroutine(SpawnAvatar("user_avatar_ybot"));
        }

        if (this.scripted_publishing)
        {
            this.PublishScriptedSequence();
            return;
        }

        if (this.avatar_visuals)
        {
            //this.PublishModelRotationTarget();
        }

        if (this.published_links.Count > 0)
        {
            float t = Time.time;
            if (t - this.publish_linear_velocity_tlast > this.publish_linear_velocity_frequency)
            {
                //this.TestPublishLinearVelocity();
                //this.TestPublishAngularVelocity();
                this.publish_linear_velocity_tlast = t;
            }

            //this.TestPublishPose();
            //this.TestPublishWrench();
        }
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
        if (this.spawning_avatar)
        {
            yield break;
        }
        this.spawning_avatar = true;

        // wait until authentication token available
        yield return new WaitUntil(() => !string.IsNullOrEmpty(AuthenticationService.Instance.token));

        this.avatar_name = "user_avatar_" + AuthenticationService.Instance.token.Replace("-", "_");
        Debug.Log("SpawnAvatar - auth token: " + AuthenticationService.Instance.token);
        Debug.Log("SpawnAvatar - avatar_name: " + this.avatar_name);
        Vector3 spawn_pos = GazeboSceneManager.Unity2GzVec3(new Vector3(avatar_visuals.transform.position.x, avatar_visuals.transform.position.y + 0.5f, avatar_visuals.transform.position.z));
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

        Debug.Log("Found avatar model: " + this.user_avatar);
        this.avatar_target = Object.Instantiate(this.user_avatar);
        this.avatar_target.transform.SetParent(this.transform);  // make sure this is not different from the parent of user_avatar (the "Gazebo Scene" object)
        this.avatar_target.name = "avatar_local_visuals";
    }

    private void TestPublishPose()
    {
        if (this.user_avatar == null)
        {
            return;
        }

        foreach(GameObject link in this.published_links)
        {
            if (link != null)
            {
                string topic = this.avatar_name + "/avatar_ybot/" + link.name + "/cmd_pose";

                Vector3 position_target = GazeboSceneManager.Unity2GzVec3(link.transform.position);
                PointMsg position_msg = new PointMsg(position_target.x, position_target.y, position_target.z);

                Quaternion rotation_target = GazeboSceneManager.Unity2GzQuaternion(link.transform.rotation);
                QuaternionMsg rotation_msg = new QuaternionMsg(rotation_target.x, rotation_target.y, rotation_target.z, rotation_target.w);
                PoseMsg pose_msg = new PoseMsg(position_msg, rotation_msg);
                ROSBridgeService.Instance.websocket.Publish(topic, pose_msg);
            }
        }
    }

    private void TestPublishLinearVelocity()
    {
        if (this.user_avatar == null)
        {
            return;
        }

        foreach (GameObject target_link in this.published_links)
        {
            if (target_link != null)
            {
                string topic = "/" + this.avatar_name + "/avatar_ybot/" + target_link.name + "/linear_vel";

                string server_link_name = this.avatar_name + "::avatar_ybot::" + target_link.name;
                GameObject server_link = GameObject.Find(server_link_name);
                if (!server_link)
                {
                    Debug.Log("could not find child link " + server_link_name + " of server avatar model");
                    return;
                }

                Vector3 position_diff = target_link.transform.position - server_link.transform.position;
                Vector3 velocity = GazeboSceneManager.Unity2GzVec3(position_diff) * this.publish_linear_velocity_velocity_factor;
                Vector3Msg velocity_msg = new Vector3Msg(velocity.x, velocity.y, velocity.z);
                ROSBridgeService.Instance.websocket.Publish(topic, velocity_msg);
            }
        }
    }

    private void TestPublishAngularVelocity()
    {
        if (this.user_avatar == null)
        {
            return;
        }

        foreach (GameObject target_link in this.published_links)
        {
            if (target_link != null)
            {
                string topic = "/" + this.avatar_name + "/avatar_ybot/" + target_link.name + "/angular_vel";

                string server_link_name = this.avatar_name + "::avatar_ybot::" + target_link.name;
                GameObject server_link = GameObject.Find(server_link_name);
                if (!server_link)
                {
                    Debug.Log("could not find child link " + server_link_name + " of server avatar model");
                    return;
                }

                //Quaternion rotation_diff = target_link.transform.rotation * Quaternion.Inverse(server_link.transform.rotation);
                Quaternion rotation_diff = Quaternion.FromToRotation(server_link.transform.forward, target_link.transform.up);

                Vector3 angular_velocity = rotation_diff.eulerAngles * 0.5f;
                Debug.Log("rotation difference (" + target_link.name + ") : " + angular_velocity);
                //rotation_diff = GazeboSceneManager.Unity2GzQuaternion(rotation_diff);
                //Vector3 rotation = rotation_diff.eulerAngles;

                Vector3Msg angular_velocity_msg = new Vector3Msg(-angular_velocity.x, -angular_velocity.y, -angular_velocity.z);
                ROSBridgeService.Instance.websocket.Publish(topic, angular_velocity_msg);
            }
        }
    }

    /*private void TestPublishWrench()
    {
        if (this.user_avatar == null)
        {
            return;
        }

        foreach (GameObject link in this.published_links)
        {
            if (link != null)
            {
                string topic = "/gazebo/default/" + this.avatar_name + "/avatar_ybot/" + link.name + "/wrench";

                string server_link_name = this.avatar_name + "::avatar_ybot::" + link.name;
                GameObject server_link = GameObject.Find(server_link_name);
                if (!server_link)
                {
                    Debug.Log("could not find child link " + server_link_name + " of server avatar model");
                    return;
                }

                Vector3 position_diff = server_link.transform.position - link.transform.position;

                Vector3 force = GazeboSceneManager.Unity2GzVec3(position_diff) * 1000f;
                Vector3Msg force_msg = new Vector3Msg(force.x, force.y, force.z);

                Vector3 torque = new Vector3();  //GazeboSceneManager.Unity2GzQuaternion(link.transform.rotation);
                Vector3Msg torque_msg = new Vector3Msg(torque.x, torque.y, torque.z);
                GzWrenchMsg wrench_msg = new GzWrenchMsg(force_msg, torque_msg);
                GzBridgeService.Instance.gzbridge.Publish(topic, wrench_msg);
            }
        }
    }*/

    /*private void CommandPublishWrench()
    {
        if (this.user_avatar == null) return;

        GameObject visual_link = this.published_links[0];
        if (!visual_link) return;

        string topic = "~/" + this.avatar_name + "/avatar_ybot/" + visual_link.name + "/wrench";
        
        Vector3Msg force_msg = new Vector3Msg(1000, 0, 0);

        Vector3 torque = new Vector3();  //GazeboSceneManager.Unity2GzQuaternion(link.transform.rotation);
        Vector3Msg torque_msg = new Vector3Msg(torque.x, torque.y, torque.z);
        GzWrenchMsg wrench_msg = new GzWrenchMsg(force_msg, torque_msg);
        Debug.Log("CommandPublishWrench() - " + wrench_msg.ToYAMLString());
        GzBridgeService.Instance.gzbridge.Publish(topic, wrench_msg);
    }*/

    private void PublishScriptedSequence()
    {
        string topic = "/user_avatar_ybot_0/avatar_ybot/mixamorig_Spine2/cmd_pose";

        Vector3 position_target = GazeboSceneManager.Unity2GzVec3(new Vector3(Mathf.Sin(Time.time), 0.5f, Mathf.Cos(Time.time)));
        PointMsg position_msg = new PointMsg(position_target.x, position_target.y, position_target.z);

        Quaternion rotation_target = GazeboSceneManager.Unity2GzQuaternion(Quaternion.Euler(0, (Time.time) % 360, 0));
        QuaternionMsg rotation_msg = new QuaternionMsg(rotation_target.x, rotation_target.y, rotation_target.z, rotation_target.w);
        PoseMsg pose = new PoseMsg(position_msg, rotation_msg);
        ROSBridgeService.Instance.websocket.Publish(topic, pose);
    }

    private void PublishModelRotationTarget()
    {
        if (this.user_avatar == null)
        {
            return;
        }

        string topic = this.avatar_name + "/cmd_rot";
        Quaternion rotation_target = GazeboSceneManager.Unity2GzQuaternion(this.avatar_visuals.transform.rotation);
        QuaternionMsg rotation_msg = new QuaternionMsg(rotation_target.x, rotation_target.y, rotation_target.z, rotation_target.w);
        ROSBridgeService.Instance.websocket.Publish(topic, rotation_msg);
    }
}
