using System.Collections;
using System.Text;
using SimpleJSON;
using System.Collections.Generic;


namespace ROSBridgeLib
{
    namespace gazebo_msgs
    {
        public class JointStateMsg : ROSBridgeMsg
        {
            /*private List<string> _name;
            private List<double> _position;
            private List<double> _rate;
            private List<geometry_msgs.Vector3Msg> _axes;
            private List<geometry_msgs.Vector3Msg> _body1Forces;
            private List<geometry_msgs.Vector3Msg> _body2Forces;
            private List<geometry_msgs.Vector3Msg> _body1Torques;
            private List<geometry_msgs.Vector3Msg> _body2Torques;*/

            private string _robot_name;
            private string _name;
            private double _position;
            private double _rate;
            private geometry_msgs.Vector3Msg _axes;
            private geometry_msgs.Vector3Msg _body1Forces;
            private geometry_msgs.Vector3Msg _body2Forces;
            private geometry_msgs.Vector3Msg _body1Torques;
            private geometry_msgs.Vector3Msg _body2Torques;

            public JointStateMsg(JSONNode msg)
            {
                /*_name = new List<string>(msg["name"].ToList());
                _position = new List<double>(msg["position"].ToList());
                _rate = new List<double>(msg["rate"].ToList());
                _axes = new List<geometry_msgs.Vector3Msg>(msg["axes"].ToList());
                _body1Forces = new List<geometry_msgs.Vector3Msg>(msg["body1Forces"].ToList());
                _body2Forces = new List<geometry_msgs.Vector3Msg>(msg["body2Forces"].ToList());
                _body1Torques = new List<geometry_msgs.Vector3Msg>(msg["body1Torques"].ToList());
                _body2Torques = new List<geometry_msgs.Vector3Msg>(msg["body2Torques"].ToList());*/

                _robot_name = msg["robot_name"];
                _name = msg["name"];
                _position = msg["position"].AsFloat;
                _rate = msg["rate"].AsFloat;
                _axes = new geometry_msgs.Vector3Msg(msg["axes"]);
                _body1Forces = new geometry_msgs.Vector3Msg(msg["body1Forces"]);
                _body2Forces = new geometry_msgs.Vector3Msg(msg["body2Forces"]);
                _body1Torques = new geometry_msgs.Vector3Msg(msg["body1Torques"]);
                _body2Torques = new geometry_msgs.Vector3Msg(msg["body2Torques"]);
            }

            public JointStateMsg(string name, double position, double rate, 
                geometry_msgs.Vector3Msg axes, 
                geometry_msgs.Vector3Msg body1Forces, 
                geometry_msgs.Vector3Msg body2Forces, 
                geometry_msgs.Vector3Msg body1Torques, 
                geometry_msgs.Vector3Msg body2Torques)
            {
                _name = name;
                _position = position;
                _rate = rate;
                _axes = axes;
                _body1Forces = body1Forces;
                _body2Forces = body2Forces;
                _body1Torques = body1Torques;
                _body2Torques = body2Torques;
            }

            public static string GetMessageType()
            {
                return "gazebo_msgs/JointState";
            }

            public string GetName()
            {
                return _name;
            }

            public override string ToString()
            {

                return "JointStates [name=" + _name + ", " + "position=" + _position.ToString() + ", rate=" + _rate.ToString() +
                    ", axes=" + _axes.ToString() + ", body_1_forces=" + _body1Forces.ToString() + ", body_2_forces=" + _body2Forces.ToString() +
                    ", body_1_torques=" + _body1Torques.ToString() + ", body_2_torques=" + _body2Torques.ToString() + "]";
            }

            public override string ToYAMLString()
            {
                return "{\"name\" : \"" + _name + "\", \"position\" : " + _position.ToString() + ", \"rate\" : " + _rate.ToString() +
                    ", axes=" + _axes.ToYAMLString() + 
                    ", \"body_1_forces\" : " + _body1Forces.ToYAMLString() + ", \"body_2_forces\" : " + _body2Forces.ToYAMLString() + 
                    ", \"body_1_torques\" : " + _body1Torques.ToYAMLString() + ", \"body_2_torques\" : " + _body2Torques.ToYAMLString() + "}"; 
            }
        }
    }
}