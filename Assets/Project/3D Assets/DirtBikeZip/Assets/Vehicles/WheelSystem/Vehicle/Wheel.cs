using UnityEngine;
using System.Collections;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WheelsSystem
{
    [ExecuteInEditMode]
    [AddComponentMenu("Physics/Wheels System/Details/Wheel")]
    [DisallowMultipleComponent]
    public class Wheel : MonoBehaviour
    {
        [Serializable]
        public struct Parameters
        {
            public string name;
            public bool useMotor;
            public bool useBrake;
            public float mass;
            public float deltaThickness;
            public float deltaRadius;
            public PhysicMaterial physicMaterial;

            public Parameters(string name, bool useMotor, bool useBrake, float mass, float deltaThickness, float deltaRadius, PhysicMaterial physicMaterial)
            {
                this.name = name;
                this.useMotor = useMotor;
                this.useBrake = useBrake;
                this.mass = mass;
                this.deltaThickness = deltaThickness;
                this.deltaRadius = deltaRadius;
                this.physicMaterial = physicMaterial;
            }
            public static bool operator !=(Parameters p1, Parameters p2)
            {
                return p1.deltaThickness != p2.deltaThickness || p1.deltaRadius != p2.deltaRadius || p1.useMotor != p2.useMotor || p1.mass != p2.mass || p1.physicMaterial != p2.physicMaterial;
            }
            public static bool operator ==(Parameters p1, Parameters p2)
            {
                return p1.deltaThickness == p2.deltaThickness && p1.deltaRadius == p2.deltaRadius && p1.useMotor == p2.useMotor && p1.mass == p2.mass && p1.physicMaterial == p2.physicMaterial;
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
        public Rigidbody body;
        public Rigidbody turnBody;
        public MeshCollider bodyCollider;
        public HingeJoint joint;
        public HingeJoint turnJoint;
        public Rigidbody connectedBody;
        public Steering steering;
        public Absorber lineralDamper;
        public Parameters parameters;
        private Parameters oldParameters;


        private Steering.Parameters steeringOldParameters;
        private float steeringOldRotation;

        private Transform wheelPivot;
        public float radius;
        /// <summary>
        /// Max velocity in meters per second
        /// </summary>
        public float maxVelocityMPS;
        private float oldMaxVelocityMPS;
        /// <summary>
        /// Velocity in meters per second
        /// </summary>
        private float velocityMPS;
        private float oldVelocityMPS;
        public float horsepower;
        private float oldHorsepower;
        private float newtonMeterPerSecond;
        private float tractionForce; 
        private Vector3 localPosition;
        [SerializeField] private Transform absorber;
        private Transform root;
        private Vehicle vehicle;

        public void StartBrake()
        {
            if (!parameters.useMotor)
            {
                this.joint.useMotor = true;
            }
        }
        public void EndBrake()
        {
            if (!parameters.useMotor)
            {
                this.joint.useMotor = false;
            }
        }
        public void Brake(float velocityMPS)
        {
            if (parameters.useBrake)
            {
                this.velocityMPS = velocityMPS;
            }
        }

        public void SetVelocityInMPS(float velocityMPS)
        {
            if (parameters.useMotor)
            {
                this.velocityMPS = velocityMPS;
            }
        }
        void OnEnable ()
        {
            vehicle = transform.GetComponentInParent<Vehicle>();
            root = vehicle.transform;

            wheelPivot = transform.parent;
            #if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if (!body)
                {
                    GameObject wheel = new GameObject(name + "_Joint");
                    GameObject wheelTurn = null;
                    if (wheelPivot.name.Contains("WheelTurn"))
                    {
                        wheelTurn = new GameObject(name + "_Turn_Joint");
                        steering = root.GetComponentInChildren<Steering>();
                    }


                    Transform physics = root.Find("Physics");
                    if (!physics)
                    {
                        physics = new GameObject("Physics").transform;
                        physics.parent = root.transform;
                        physics.position = root.position;
                        physics.transform.localScale = Vector3.one;
                    }

                    wheel.transform.parent = physics;
                    wheel.transform.position = transform.position;
                    wheel.transform.rotation = root.rotation;

                    if (wheelTurn)
                    {
                        wheelTurn.transform.parent = physics;
                        wheelTurn.transform.position = wheelPivot.position;
						lineralDamper = GetComponentInParent<Absorber>();
                        if(!lineralDamper.name.Contains("LineralAbsorber"))
                        {
                            lineralDamper = null;
                        }
                        if(lineralDamper)
                        {
                            wheelTurn.transform.LookAt(wheelTurn.transform.position + steering.transform.up, -steering.transform.forward);
                        }
                        else
                        {
                            wheelTurn.transform.rotation = root.rotation;
                        }
                    }

                    joint = wheel.AddComponent<HingeJoint>();
                    bodyCollider = wheel.AddComponent<MeshCollider>();
                    body = wheel.GetComponent<Rigidbody>();

                    PhysicMaterial physicMaterial = (PhysicMaterial)AssetDatabase.LoadAssetAtPath("Assets/Vehicles/WheelSystem/PhysicsMaterials/Wheel.physicMaterial", typeof(PhysicMaterial));
              
                    parameters = new Parameters(name, true, true, 2.0f, 0.0f, 0.0f, physicMaterial);
                    joint.anchor = Vector3.zero;
                    joint.axis = Vector3.right;

                    if (wheelTurn)
                    {
                        turnJoint = wheelTurn.AddComponent<HingeJoint>();
                        turnBody = wheelTurn.GetComponent<Rigidbody>();
                        turnBody.mass = parameters.mass;
                        turnJoint.anchor = Vector3.zero;
                        turnJoint.axis = Vector3.up;

                        UpdateSteeringParameters();
                    }

                    if (!turnJoint)
                    {
                        Absorber aAbsorber = transform.GetComponentInParent<Absorber>();
                        if (aAbsorber)
                        {
                            connectedBody = aAbsorber.body;
                            absorber = aAbsorber.transform;
                        }
                        else
                        {
                            connectedBody = root.GetComponent<Rigidbody>();
                            absorber = root;

                        }
                    }
                    else
                    {
                        Absorber aAbsorber = transform.GetComponentInParent<Absorber>();
                        if (aAbsorber)
                        {
                            turnJoint.connectedBody = aAbsorber.body;
                            absorber = aAbsorber.transform;
                        }
                        else
                        {
                            turnJoint.connectedBody = root.GetComponent<Rigidbody>();
                            absorber = root;

                        }
                        connectedBody = turnBody;

                    }

                    joint.connectedBody = connectedBody;

                    //absorber = connectedBody.transform;

                    horsepower = 10.0f;
                    maxVelocityMPS = 55.0f;
                    newtonMeterPerSecond = 746.0f * horsepower;
                    tractionForce = newtonMeterPerSecond / maxVelocityMPS;
                    CalculateCollider();
                }
            }
            else
            {
            #endif
                if (!absorber)
                {
                    enabled = false;
                    return;
                }
                newtonMeterPerSecond = 746.0f * horsepower;
                tractionForce = newtonMeterPerSecond / maxVelocityMPS;
                radius = 0.5f * body.transform.lossyScale.y;
                body.maxAngularVelocity = maxVelocityMPS / radius;

                JointMotor motor = new JointMotor();
                motor.targetVelocity = (180.0f / Mathf.PI) * velocityMPS / radius;
                motor.force = tractionForce * radius;;
                joint.motor = motor;

                Absorber aAbsorber = transform.GetComponentInParent<Absorber>();

                if (aAbsorber)
                {
                    aAbsorber.OnUpdade += OnUpdade;
                } else if (vehicle)
                {
                    vehicle.OnUpdade += OnUpdade;
                }

                if (connectedBody)
                {
                    localPosition = VectorOperator.getLocalPosition(absorber, wheelPivot.position);
                    wheelPivot.parent = root;
                }
                foreach (var item in root.GetComponentsInChildren<Collider>())
                {
                    Physics.IgnoreCollision(bodyCollider, item);
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
                    parameters.name = name;
                }

                body.mass = parameters.mass;
                joint.useMotor = parameters.useMotor;
                joint.connectedBody = connectedBody;
                bodyCollider.sharedMaterial = parameters.physicMaterial;

                if (oldMaxVelocityMPS != maxVelocityMPS || oldHorsepower != horsepower)
                {
                    oldMaxVelocityMPS = maxVelocityMPS;
                    oldHorsepower = horsepower;
                    newtonMeterPerSecond = 746.0f * horsepower;
                    tractionForce = newtonMeterPerSecond / maxVelocityMPS;

                    joint.motor = new JointMotor(){ targetVelocity = (180.0f / Mathf.PI) * velocityMPS / radius, force = tractionForce * radius};
                }

                if (oldParameters != parameters)
                {
                    oldParameters = parameters;
                    CalculateCollider();
                }
                if (steering && steeringOldParameters != steering.parameters)
                {
                    steeringOldParameters = steering.parameters;
                    UpdateSteeringParameters();
                }
            }


        }

        #endif
        void OnUpdade ()
        {
            #if UNITY_EDITOR
            if (Application.isPlaying)
            {
            #endif
                if (connectedBody)
                {
                    wheelPivot.position = VectorOperator.getWordPosition(absorber, localPosition);
                    if (turnBody)
                    {
                        if (lineralDamper)
                        {
                            wheelPivot.transform.LookAt(wheelPivot.transform.position + steering.transform.up, -steering.transform.forward);
                        }
                        else
                        {
                            wheelPivot.LookAt(wheelPivot.position + Vector3.ProjectOnPlane(turnBody.transform.forward, root.up).normalized, root.up);
                        }
                    }

                    float angularVelocityX = Vector3.Dot(body.angularVelocity, wheelPivot.right);

                    if (Mathf.Abs(angularVelocityX) > 0.9f)
                    {
                        transform.RotateAround(wheelPivot.position, wheelPivot.right, (180.0f / Mathf.PI) * angularVelocityX * Time.deltaTime);
                    }
                }
                #if UNITY_EDITOR
            }
                #endif
        }
        void FixedUpdate ()
        {
            #if UNITY_EDITOR
            if (Application.isPlaying)
            {
            #endif
                if (oldVelocityMPS != velocityMPS && velocityMPS < maxVelocityMPS)
                {
                    oldVelocityMPS = velocityMPS;
                    joint.motor = new JointMotor(){ targetVelocity = (180.0f / Mathf.PI) * velocityMPS / radius, force = tractionForce * radius};
                }

                if (steering)
                {
                    if (steering && (steeringOldRotation != steering.rotation))
                    {
                        steeringOldRotation = steering.rotation;
                        RotateFromSteering();
                    }
                }
                #if UNITY_EDITOR
            }
                #endif
        }
        void UpdateSteeringParameters()
        {
            JointSpring spring = new JointSpring();
            spring.spring = steering.parameters.torque;
            spring.damper = steering.parameters.damper;
            turnJoint.spring = spring;

            JointLimits limits = new JointLimits();
            limits.min = -steering.parameters.limit;
            limits.max = steering.parameters.limit;
            turnJoint.limits = limits;

            turnJoint.useSpring = true;
            turnJoint.useLimits = true;
            turnJoint.enablePreprocessing = true;

            turnBody.mass = steering.parameters.mass;
        }
        void RotateFromSteering()
        {
            turnJoint.spring = new JointSpring(){ spring = steering.parameters.torque, damper = steering.parameters.damper, targetPosition = -steering.rotation};
        }
        public void StopControl ()
        {
            JointMotor motor = new JointMotor();
            motor.targetVelocity = 0.0f;
            motor.force = tractionForce;
            joint.motor = motor;

            joint.motor = new JointMotor(){ targetVelocity = 0.0f, force = tractionForce * radius};
        }
        #if UNITY_EDITOR
        public void CalculateCollider ()
        {
            BoxCollider box = gameObject.AddComponent<BoxCollider>();
            Vector3 size = box.size;
            DestroyImmediate(box); 

            bodyCollider.sharedMesh = (Mesh)AssetDatabase.LoadAssetAtPath("Assets/Vehicles/WheelSystem/Colliders/WheelCollider.fbx", typeof(Mesh));

            bodyCollider.convex = true;
            body.transform.localScale = new Vector3(parameters.deltaThickness + size.x, parameters.deltaRadius + size.y, parameters.deltaRadius + size.z);
            bodyCollider.sharedMaterial = parameters.physicMaterial;

            radius = 0.5f * body.transform.lossyScale.y;

            JointMotor motor = new JointMotor();
            motor.targetVelocity = (180.0f / Mathf.PI) * maxVelocityMPS / radius;
            motor.force = tractionForce * radius;
            joint.motor = motor;
            joint.enablePreprocessing = true;
        }
        #endif
    }
}
