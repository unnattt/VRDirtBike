using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace WheelsSystem
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(VehicleResset))]
    [RequireComponent(typeof(VehicleUserController))]
    [RequireComponent(typeof(VehicleResset))]
    [RequireComponent(typeof(Rigidbody))]
    public abstract class Vehicle : MonoBehaviour
    {
        [SerializeField]
        private bool initialized = false;
        public static bool createInEditorWindow = false;

        [Serializable]
        public struct Parameters
        {
            /// <summary>
            /// The max velocity meter per seconds.
            /// </summary>
            public float maxVelocityMPS;
           
            /// <summary>
            /// The acceleration of vehicle.
            /// </summary>
            public float acceleration;
            /// <summary>
            /// The horsepower of vehicle.
            /// </summary>
            public float horsepower;
            /// <summary>
            /// The mass of vehicle.
            /// </summary>
            public float mass;

            public Parameters(float maxVelocityMPS, float acceleration, float horsepower, float mass)
            {
                this.maxVelocityMPS = maxVelocityMPS;
                this.acceleration = acceleration;
                this.horsepower = horsepower;
                this.mass = mass;
            }

            public static bool operator !=(Parameters p1, Parameters p2)
            {
                return p1.maxVelocityMPS != p2.maxVelocityMPS || p1.acceleration != p2.acceleration || p1.horsepower != p2.horsepower || p1.mass != p2.mass;
            }

            public static bool operator ==(Parameters p1, Parameters p2)
            {
                return p1.maxVelocityMPS == p2.maxVelocityMPS && p1.acceleration == p2.acceleration && p1.horsepower == p2.horsepower && p1.mass == p2.mass;
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
        public MeshCollider bodyCollider;
        public Transform centerOfMass;
        public Parameters parameters;
        private Parameters oldParameters;
        public event Action OnUpdade;
        private VehicleUserController vehicleController;

        [SerializeField] private Steering.Parameters steeringParameter;
        [SerializeField] private List<Wheel.Parameters> wheelsParameters;

        public Steering steering;

        [SerializeField] private List<Wheel> wheels;
        private float velocityMPS;
        private float brakeVelocityMPS;

        public void StopControl()
        {
            velocityMPS = 0.0f;
            foreach (var item in gameObject.GetComponentsInChildren<Wheel>())
            {
                item.StopControl();
            }
            GetComponent<VehicleUserController>().enabled = false;
        }

        public void RotateSteering(float unitSpeed)
        {
            steering.Rotate(unitSpeed);
        }

        public void NormalizeSteering()
        {
            steering.Normalize();
        }

        public void Accelerate(float unitSpeed)
        {
            unitSpeed = Mathf.Clamp(unitSpeed, -1.0f, 1.0f);
            if (velocityMPS * unitSpeed < 0.0f)
            {
                velocityMPS += 10.0f * unitSpeed * parameters.acceleration * Time.fixedDeltaTime;
            }
            else
            {
                velocityMPS += unitSpeed * parameters.acceleration * Time.fixedDeltaTime;
            }
            velocityMPS = Mathf.Clamp(velocityMPS, -parameters.maxVelocityMPS, parameters.maxVelocityMPS);
            foreach (Wheel wheel in wheels)
            {
                wheel.SetVelocityInMPS(velocityMPS);
            }
        }

        public void ForceAccelerate(float force)
        {
            velocityMPS += force * parameters.acceleration * Time.fixedDeltaTime;
            velocityMPS = Mathf.Clamp(velocityMPS, -parameters.maxVelocityMPS, parameters.maxVelocityMPS);
            foreach (Wheel wheel in wheels)
            {
                wheel.SetVelocityInMPS(velocityMPS);
            }
        }

        public void StartBrake(bool isFront)
        {
            brakeVelocityMPS = velocityMPS;
            foreach (Wheel wheel in wheels)
            {
                if ((isFront && wheel.steering) || (!isFront && !wheel.steering))
                {
                    wheel.StartBrake();
                }
            }
        }
        public void EndBrake(bool isFront)
        {
            foreach (Wheel wheel in wheels)
            {
                if ((isFront && wheel.steering) || (!isFront && !wheel.steering))
                {
                    wheel.EndBrake();
                }
            }
        }
        public void FrontBrake(float value01)
        {
            velocityMPS = Mathf.Lerp(velocityMPS, Mathf.Clamp01(1.0f - value01) * velocityMPS, 5.0f * Time.fixedDeltaTime);
            foreach (Wheel wheel in wheels)
            {
                if (wheel.steering)
                {
                    wheel.Brake(velocityMPS);
                }
                else
                {
                    brakeVelocityMPS = Mathf.Lerp(brakeVelocityMPS, 0.0f, 0.5f * Time.fixedDeltaTime);
                    wheel.SetVelocityInMPS(brakeVelocityMPS);
                }
            }
        }
        public void BackBrake(float value01)
        {
            velocityMPS = Mathf.Lerp(velocityMPS, Mathf.Clamp01(1.0f - value01) * velocityMPS, 5.0f * Time.fixedDeltaTime);
            foreach (Wheel wheel in wheels)
            {
                if (!wheel.steering)
                {
                    wheel.Brake(velocityMPS);
                }
                else
                {
                    brakeVelocityMPS = Mathf.Lerp(brakeVelocityMPS, 0.0f, 0.5f * Time.fixedDeltaTime);
                    wheel.SetVelocityInMPS(velocityMPS);
                }
            }
        }

        public void SlowDown()
        {
            velocityMPS = Mathf.Lerp(velocityMPS, 0.0f, 0.5f * Time.fixedDeltaTime);
            foreach (Wheel wheel in wheels)
            {
                wheel.SetVelocityInMPS(velocityMPS);
            }
        }
        public void ForceStopControl()
        {
            velocityMPS = 0.0f;
            foreach (var item in gameObject.GetComponentsInChildren<Wheel>())
            {
                item.StopControl();
            }
        }

        void OnEnable()
        {
            #if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if (!initialized)
                {
                    Debug.Log("initialized");
                    initialized = true;
                    body = gameObject.GetComponent<Rigidbody>();
                    gameObject.GetComponent<VehicleUserController>().SetVehicle(this);
                    if (!createInEditorWindow)
                    {
                        TryToSet();
                    }
                }
            }
            else
            {
            #endif
                foreach (var item in GetComponentsInChildren<Rigidbody>())
                {
                    item.maxAngularVelocity = 50.0f;
                }
                velocityMPS = 0.0f;
                body.centerOfMass = centerOfMass.localPosition;
                vehicleController = GetComponent<VehicleUserController>();
            #if UNITY_EDITOR
            }
            #endif
            OnTrigger(true);
        }
        #if UNITY_EDITOR
        public void TryToSetBodyFromEditorWindow(Transform centerOfMass, Transform bodyCollider, Parameters bodyParameters, Transform steering, Steering.Parameters steeringParameter)
        {
                if (centerOfMass)
                {
                    this.centerOfMass = centerOfMass;
                }
                if (bodyCollider)
                {
                    this.bodyCollider = bodyCollider.gameObject.AddComponent<MeshCollider>();
                    this.bodyCollider.convex = true;
                    this.bodyCollider.GetComponent<MeshRenderer>().enabled = false;
                }
                this.bodyCollider.sharedMaterial = (PhysicMaterial)AssetDatabase.LoadAssetAtPath("Assets/WheelSystem/PhysicsMaterials/Body.physicMaterial", typeof(PhysicMaterial));
                this.parameters = bodyParameters;
                if (steering)
                {
                    this.steering = steering.gameObject.AddComponent<Steering>();
                    this.steeringParameter = steeringParameter;
                }
        }

        public void TryToSetWheelsFromEditorWindow(Transform[] wheels, Wheel.Parameters[] wheelParameters)
        {
            int i = 0;
            if (wheels != null)
            {
                this.wheelsParameters = new List<Wheel.Parameters>(0);
                this.wheels = new List<Wheel>(0);
                foreach (var item in wheels)
                {
                    Wheel wheel = item.gameObject.GetComponent<Wheel>();
                    if (!wheel)
                    {
                        wheel = item.gameObject.AddComponent<Wheel>();
                    }
                    this.wheels.Add(wheel);
                    this.wheelsParameters.Add(wheelParameters[i]);
                    i++;
                }
            }
        }
        #endif
        #if UNITY_EDITOR
        private void TryToSet()
        {
            centerOfMass = transform.Find("CenterOfMass");
            parameters = new Parameters(10.0f, 1.5f, 55.0f, 300.0f);
            if (!bodyCollider)
            {
                Transform bc = transform.Find("BodyCollider");
                if (bc)
                {
                    bodyCollider = bc.gameObject.GetComponent<MeshCollider>();
                    if (!bodyCollider)
                    {
                        bodyCollider = bc.gameObject.AddComponent<MeshCollider>();
                    }
                    bodyCollider.sharedMaterial = (PhysicMaterial)AssetDatabase.LoadAssetAtPath("Assets/WheelSystem/PhysicsMaterials/Body.physicMaterial", typeof(PhysicMaterial));

                    bodyCollider.convex = true;
                    if (bodyCollider.GetComponent<MeshRenderer>())
                    {
                        bodyCollider.GetComponent<MeshRenderer>().enabled = false;
                    }
                }
            }

            foreach (var item in transform.GetComponentsInChildren<Transform>())
            {
                if (!item.name.Contains("Joint") && item.name.Contains("Steering"))
                {
                    steering = item.gameObject.GetComponent<Steering>();
                    if (!steering)
                    {
                        steering = item.gameObject.AddComponent<Steering>();
                    }
                    steeringParameter = steering.parameters;
                    break;
                }
            }
            SetAbsorbers();



            wheelsParameters = new List<Wheel.Parameters>(0);
            wheels = new List<Wheel>(0);
            foreach (var item in transform.GetComponentsInChildren<Transform>())
            {
                if (!item.name.Contains("Joint") && item.name.Contains("Wheel") && !item.name.Contains("WheelPivot") && !item.name.Contains("WheelTurn"))
                {
                    Wheel wheel = item.gameObject.GetComponent<Wheel>();
                    if (!wheel)
                    {
                        wheel = item.gameObject.AddComponent<Wheel>();
                    }
                    wheels.Add(wheel);
                    wheelsParameters.Add(wheel.parameters);
                }
            }
        }
        #endif
        #if UNITY_EDITOR
        void OnDestroy()
        {
            if (!Application.isPlaying)
            {
                foreach (var item in transform.GetComponentsInChildren<Steering>())
                {
                    DestroyImmediate(item);
                }
				foreach (var item in transform.GetComponentsInChildren<Absorber>())
                {
                    DestroyImmediate(item);
                }
                foreach (var item in transform.GetComponentsInChildren<Wheel>())
                {
                    DestroyImmediate(item);
                }

                DestroyImmediate(bodyCollider);
                Transform physics = transform.Find("Physics");
                if (physics)
                {
                    DestroyImmediate(physics.gameObject);
                }
            }
        }
        #endif
        void OnDisable()
        {
            OnTrigger(false);
        }

        void OnTrigger(bool isOn)
        {
            foreach (var item in transform.GetComponentsInChildren<Steering>())
            {
                item.enabled = isOn;
            }
			foreach (var item in transform.GetComponentsInChildren<Absorber>())
            {
                item.enabled = isOn;
            }
            foreach (var item in transform.GetComponentsInChildren<Wheel>())
            {
                item.enabled = isOn;
            }
        }
        #if UNITY_EDITOR
        void Update()
        {
            if (!Application.isPlaying)
            {
                body.mass = parameters.mass;
                SetSteering();
                UpdateAbsorbers();
                SetWheels();
            }
        }
        #endif

        void LateUpdate ()
        {
            #if UNITY_EDITOR
            if (Application.isPlaying)
            {
            #endif
                if (OnUpdade != null)
                {
                    OnUpdade();
                }
            #if UNITY_EDITOR
            }
            #endif
        }


        void FixedUpdate()
        {
            #if UNITY_EDITOR
            if (Application.isPlaying)
            {
            #endif
                if (!vehicleController.enabled)
                {
                    return;
                }
                if (body.velocity.magnitude > 2.5f)
                {
                    Vector3 rightVelocity = Vector3.Project(body.velocity, transform.right);
                    float dir = Vector3.Dot(rightVelocity, transform.right) > 0.0f ? 1.0f : -1.0f;
                    body.AddRelativeForce(-dir * body.mass * rightVelocity.magnitude * Vector3.right, ForceMode.Force);
                }
                float angularVelocityZ = Mathf.Clamp( Vector3.Dot(body.angularVelocity, transform.forward), -50.0f, 50.0f);
                float angularVelocityY = Mathf.Clamp( Vector3.Dot(body.angularVelocity, transform.up), -50.0f, 50.0f);

                body.AddRelativeTorque(-body.mass * angularVelocityZ * Vector3.forward, ForceMode.Force);
                body.AddRelativeTorque(-body.mass * angularVelocityY * (1.0f - Mathf.Abs(vehicleController.turn)) * Vector3.up, ForceMode.Force);

            #if UNITY_EDITOR
            }
            #endif
        }

        void SetSteering()
        {
            if (!steering)
            {
                return;
            }
            if (steering.parameters != steeringParameter)
            {
                steering.parameters = steeringParameter;
            }
            if (steeringParameter.name != steering.name)
            {
                steeringParameter.name = steering.name;
            }
        }

        protected abstract void SetAbsorbers();
        protected abstract void UpdateAbsorbers();
  
        void SetWheels()
        {
            if (wheels == null)
            {
                return;
            }
            int i = 0;
            int motorCount = 0;
            foreach (Wheel wheel in wheels)
            {
                Wheel.Parameters wheelParameters = wheelsParameters[i];
                wheel.parameters = wheelParameters;
                if (wheelParameters.name != wheel.name)
                {
                    wheelParameters.name = wheel.name;
                    wheelsParameters[i] = wheelParameters;
                }
                if (wheel.parameters.useMotor)
                {
                    motorCount++;
                }
                i++;
            }
            if (oldParameters != parameters)
            {
                oldParameters = parameters;
                foreach (Wheel wheel in wheels)
                {
                    wheel.horsepower = parameters.horsepower / motorCount;
                    wheel.maxVelocityMPS = parameters.maxVelocityMPS;
                }
            }
        }
    }
}
