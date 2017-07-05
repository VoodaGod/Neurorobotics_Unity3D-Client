using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class GazeboSceneManager : MonoBehaviour {

    public enum GZ_LIGHT_TYPE
    {
        POINT = 1,
        SPOT = 2,
        DIRECTIONAL = 3,
        UNKNOWN = 4
    }

    private string scene_name_ = null;

	// Use this for initialization
	void Start () {
        Camera.main.transform.position = new Vector3(4, 4, 2);
        Camera.main.transform.LookAt(new Vector3(), new Vector3(0, 0, 1));
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public bool BuildScene(JSONNode sceneMsg)
    {
        // clear scene before building new one
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in transform)
            children.Add(child.gameObject);
        children.ForEach(child => Destroy(child));

        // name
        if (sceneMsg["name"] != null)
            scene_name_ = sceneMsg["name"];

        // ambient color
        JSONClass ambient = sceneMsg["ambient"].AsObject;
        if (ambient != null)
        {
            Color color_ambient = new Color(ambient["r"].AsFloat, ambient["g"].AsFloat, ambient["b"].AsFloat, ambient["a"].AsFloat);
            RenderSettings.ambientLight = color_ambient;
        }

        // background color
        JSONClass background = sceneMsg["background"].AsObject;
        if (ambient != null)
        {
            Color color_background = new Color(background["r"].AsFloat, background["g"].AsFloat, background["b"].AsFloat, background["a"].AsFloat);
            Camera.main.backgroundColor = color_background;
        }

        // lights
        JSONArray lights = sceneMsg["light"].AsArray;
        foreach(JSONNode light in lights)
        {
            this.CreateLightFromJSON(light);
        }

        // models
        JSONArray models = sceneMsg["model"].AsArray;
        foreach (JSONNode model in models)
        {
            this.CreateModelFromJSON(model);
        }

        // joints
        JSONArray joints = sceneMsg["joint"].AsArray;
        foreach (JSONNode joint in joints)
        {
            this.CreateJointFromJSON(joint);
        }

        return true;
    }

    private void CreateLightFromJSON(JSONNode json_light)
    {
        string light_name = json_light["name"];
        GameObject light_gameobject = new GameObject(light_name);
        light_gameobject.transform.parent = this.gameObject.transform;
        Light light_component = light_gameobject.AddComponent<Light>();

        // pose
        JSONNode json_pose = json_light["pose"];
        if (json_pose != null)
        {
            // position
            JSONNode json_position = json_pose["position"];
            Vector3 position = new Vector3(json_position["x"].AsFloat, json_position["y"].AsFloat, json_position["z"].AsFloat);
            light_gameobject.transform.position = position;
            // rotation
            JSONNode json_rotation = json_pose["orientation"];
            Quaternion rotation = new Quaternion(json_rotation["x"].AsFloat, json_rotation["y"].AsFloat, json_rotation["z"].AsFloat, json_rotation["w"].AsFloat);
            light_gameobject.transform.rotation = rotation;
        }

        // color
        JSONNode json_diffuse = json_pose["diffuse"];
        if (json_diffuse != null)
        {
            Color color_diffuse = new Color(json_diffuse["r"].AsFloat, json_diffuse["g"].AsFloat, json_diffuse["b"].AsFloat, json_diffuse["a"].AsFloat);
            light_component.color = color_diffuse;
        }
        //TODO: what to do with specular part of gazebo light?
        
        // light type
        JSONNode light_type = json_light["type"];
        if (light_type != null)
        {
            if (light_type.AsInt == (int)GZ_LIGHT_TYPE.POINT)
            {
                light_component.type = LightType.Point;
                light_component.range = json_light["range"].AsFloat;
            }
            else if (light_type.AsInt == (int)GZ_LIGHT_TYPE.SPOT)
            {
                light_component.type = LightType.Spot;
                light_component.range = json_light["range"].AsFloat;
                light_component.spotAngle = Mathf.Rad2Deg * json_light["spot_outer_angle"].AsFloat;

                JSONNode json_direction = json_light["direction"];
                Vector3 direction = new Vector3(json_direction["x"].AsFloat, json_direction["y"].AsFloat, json_direction["z"].AsFloat);
                light_gameobject.transform.LookAt(light_gameobject.transform.position + direction);
            }
            else if (light_type.AsInt == (int)GZ_LIGHT_TYPE.DIRECTIONAL)
            {
                light_component.type = LightType.Directional;

                JSONNode json_direction = json_light["direction"];
                Vector3 direction = new Vector3(json_direction["x"].AsFloat, json_direction["y"].AsFloat, json_direction["z"].AsFloat);
                light_gameobject.transform.LookAt(light_gameobject.transform.position + direction);
            }
        }

        // intensity (from gazebo attenuation factors)
        // equation taken from https://docs.blender.org/manual/en/dev/render/blender_render/lighting/lights/light_attenuation.html
        if (json_light["attenuation_linear"] != null && json_light["attenuation_quadratic"] != null)
        {
            float E = 1.0f;
            float D = 1.0f;
            float r = 1.0f;
            float L = json_light["attenuation_linear"].AsFloat;
            float Q = json_light["attenuation_quadratic"].AsFloat;

            float intensity = E * (D / (D + L * r)) * (Mathf.Pow(D, 2.0f) / (Mathf.Pow(D, 2.0f) + Q * Mathf.Pow(r, 2.0f)));
            light_component.intensity = intensity;
        }

        // shadows
        bool cast_shadows = json_light["cast_shadows"].AsBool;
        if (cast_shadows)
        {
            light_component.shadows = LightShadows.Soft;
        }
        else
        {
            light_component.shadows = LightShadows.None;
        }
    }

    private void CreateModelFromJSON(JSONNode model_node)
    {

    }

    private void CreateJointFromJSON(JSONNode joint_node)
    {

    }
}
