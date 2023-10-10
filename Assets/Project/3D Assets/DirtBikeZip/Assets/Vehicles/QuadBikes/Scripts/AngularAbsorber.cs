using UnityEngine;
using System.Collections;
using System;

namespace WheelsSystem
{
    [ExecuteInEditMode]
    [AddComponentMenu("Physics/Wheels System/Details/Angular absorber")]
    [DisallowMultipleComponent]
    public class AngularAbsorber : Absorber
    {
        [Serializable]
        public struct Parameters
        {
            public string name;
            public float torque;
            public float damper;
            public float limit; 
            public float mass;

            public Parameters(string name, float torque, float damper, float limit, float mass)
            {
                this.name = name;
                this.torque = torque;
                this.damper = damper;
                this.limit = limit;
                this.mass = mass;
            }
            public static bool operator !=(Parameters p1, Parameters p2)
            {
                return p1.torque != p2.torque || p1.damper != p2.damper || p1.limit != p2.limit || p1.mass != p2.mass;
            }
            public static bool operator ==(Parameters p1, Parameters p2)
            {
                return p1.torque == p2.torque && p1.damper == p2.damper && p1.limit == p2.limit && p1.mass == p2.mass;
            }
            public override bool Equals(object obj)
            {
                return base.Equals(obj);
            }
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
            public override string ToString()
            {
                return string.Format("[Parameters]");
            }
        }

        public HingeJoint joint;
        public Spring springDetail;
        private float upDirection;
        public Parameters parameters;
        private Parameters oldParameters;
        private Transform root;

        void OnEnable ()
        {
            root = transform.GetComponentInParent<Vehicle>().transform;
            #if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if (!body)
                {
                    GameObject absorber = new GameObject(name + "_Joint");

                    Transform physics = root.Find("Physics");
                    if (!physics)
                    {
                        physics = new GameObject("Physics").transform;
                        physics.parent = root;
                        physics.position = root.position;
                        physics.transform.localScale = Vector3.one;
                    }
                    absorber.transform.parent = physics;
                    absorber.transform.position = transform.position;
                    absorber.transform.rotation = transform.rotation;
                    joint = absorber.AddComponent<HingeJoint>();
                    body = absorber.GetComponent<Rigidbody>();
                    joint.anchor = Vector3.zero;
                    joint.axis = Vector3.right;
                    connectedBody = root.GetComponent<Rigidbody>();

                    joint.connectedBody = connectedBody;
                    parameters = new Parameters(name, 3000.0f, 500.0f, 40.0f, 20.0f);

                    joint.spring = new JointSpring(){spring = parameters.torque, damper = parameters.damper};
                    joint.limits = new JointLimits(){min = -parameters.limit, max = parameters.limit};

                    joint.useSpring = true;
                    joint.useLimits = true;
                    joint.enablePreprocessing = true;

                    springDetail = absorber.AddComponent<Spring>();
                    if (!Vehicle.createInEditorWindow)
                    {
                        springDetail.TryToSet(transform);
                    }
                }
            }
            else
            {
            #endif
                springDetail = body.GetComponent<Spring>();
                upDirection = Vector3.Dot( transform.up , root.up) >= 0.0f? 1.0f:-1.0f;
                if (springDetail && connectedBody)
                {
                    Steering steering = transform.GetComponentInParent<Steering>();
                    if (steering)
                    {
                        springDetail.SetParent(steering.transform, transform);
                    }
                    else
                    {
                        springDetail.SetParent(root, transform);
                    }
                }
            #if UNITY_EDITOR
            }
            #endif
        }

        #if UNITY_EDITOR
        void OnDestroy ()
        {
            if (!Application.isPlaying)
            {
                if (body)
                {
                    DestroyImmediate(body.gameObject);
                }
            }
        }
        #endif

        #if UNITY_EDITOR
        void Update ()
        {
            if (!Application.isPlaying)
            {
                if (parameters.name != name)
                {
                    parameters = new Parameters(name, parameters.torque, parameters.damper, parameters.limit, parameters.mass);
                }
                body.mass = parameters.mass;
                joint.connectedBody = connectedBody;

                if (oldParameters != parameters)
                {
                    oldParameters = parameters;
                    joint.spring = new JointSpring(){spring = parameters.torque, damper = parameters.damper};
                    joint.limits = new JointLimits(){min = -parameters.limit, max = parameters.limit};
                }
            }
        }
        #endif
        void LateUpdate ()
        {
            #if UNITY_EDITOR
            if (Application.isPlaying)
            {
            #endif
                if (connectedBody)
                {
                    transform.LookAt(transform.position + body.transform.forward, upDirection * root.up);
                    CallUpdate();
                }
            #if UNITY_EDITOR
            }
            #endif
        }
    }
}
