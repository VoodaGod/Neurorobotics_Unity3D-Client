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

    private string avatar_name = null;

    private GameObject user_avatar = null;

    void Awake()
    {
        GzBridgeService.Instance.AddCallbackModelInfoMsg(this.OnModelInfoMsg);
        GzBridgeService.Instance.AddCallbackOnCloseConnection(this.DespawnAvatar);
    }

    // Use this for initialization
    void Start()
    {
        this.SpawnAvatarRayman();  // test
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SpawnAvatarRayman()
    {
        StartCoroutine(SpawnAvatar("user_avatar_rayman"));
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
        Vector3 spawn_pos = GazeboSceneManager.Unity2GzVec3(new Vector3(-6.5f, 0f, 2f));
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

    private IEnumerator TestAvatarRaymanMovement()
    {


        yield return null;
    }
}
