using System.Collections;
using System.Text;
using SimpleJSON;

namespace ROSBridgeLib
{
    namespace gazebo_msgs
    {
        public class ModelStateMsg : ROSBridgeMsg
        {
            private string _model_name;
            private geometry_msgs.PoseMsg _pose;
            private geometry_msgs.Vector3Msg _scale;
            private geometry_msgs.TwistMsg _twist;
            private string _reference_frame;

            public ModelStateMsg(JSONNode msg)
            {
                _model_name = msg["model_name"];
                _pose = new geometry_msgs.PoseMsg(msg["pose"]);
                _twist = new geometry_msgs.TwistMsg(msg["twist"]);
                _reference_frame = msg["reference_frame"];
            }

            public ModelStateMsg(string model_name, geometry_msgs.PoseMsg pose, geometry_msgs.Vector3Msg scale, geometry_msgs.TwistMsg twist, string reference_frame = "world")
            {
                _model_name = model_name;
                _pose = pose;
                _scale = scale;
                _twist = twist;
                _reference_frame = reference_frame;
            }

            public static string GetMessageType()
            {
                return "gazebo_msgs/ModelState";
            }

            public string GetModelName()
            {
                return _model_name;
            }

            public geometry_msgs.PoseMsg GetPose()
            {
                return _pose;
            }

            public geometry_msgs.Vector3Msg GetScale()
            {
                return _scale;
            }

            public geometry_msgs.TwistMsg GetTwist()
            {
                return _twist;
            }

            public string GetReferenceFrame()
            {
                return _reference_frame;
            }

            public override string ToString()
            {
                return "ModelState [model_name=" + _model_name.ToString() + ", " + "pose=" + _pose.ToString() + ", scale=" + _scale.ToString() + 
                    ",  twist=" + _twist.ToString() + ",  reference_frame=" + _reference_frame.ToString() + "]";
            }

            public override string ToYAMLString()
            {
                return "{\"model_name\" : \"" + _model_name + "\", \"pose\" : " + _pose.ToYAMLString() + ", \"scale\" : " + _scale.ToYAMLString() + 
                    ", \"twist\" : " + _twist.ToYAMLString() + ", \"reference_frame\" : \"" + _reference_frame + "\"}";
            }
        }
    }
}