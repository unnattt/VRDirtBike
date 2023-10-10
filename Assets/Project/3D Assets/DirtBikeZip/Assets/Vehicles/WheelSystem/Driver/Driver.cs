using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WheelsSystem
{
    public abstract class Driver : MonoBehaviour
    {
        [SerializeField] private DriverPhysics driverPhysics;
        [System.NonSerialized] public VehicleUserController vehicleController;
        protected Steering steering;
        protected Vehicle vehicle;
        protected Transform driverIK;
        protected Transform driver;
        protected Animator animator;
        [SerializeField] private float deltaAngleOfRoll = 5.0f;
        [SerializeField] private float angleOfRollDirection = 1.0f;
        private float angleOfRoll;


        protected virtual void Awake()
        {
            animator = GetComponent<Animator>();
            vehicle = transform.root.GetComponent<Vehicle>();
            if (!vehicle)
            {
                enabled = false;
                return;
            }
            steering = vehicle.GetComponentInChildren<Steering>();
			if (!steering)
            {
				enabled = false;
				return;

			}

            Collider bodyCollider = GetComponentInParent<Vehicle>().bodyCollider;
            Collider crashCollider = GetComponentInChildren<CrashChecker>().GetComponent<Collider>();
            vehicleController = GetComponentInParent<VehicleUserController>();

            foreach (var item in driverPhysics.GetComponentsInChildren<Collider>())
            {
                Physics.IgnoreCollision(bodyCollider, item);
                Physics.IgnoreCollision(crashCollider, item);
            }
            driverPhysics.OnAwake();
            driverPhysics.SetActive(false);

            driverIK = vehicle.transform.Find("DriverIK");
            driver = vehicle.transform.Find("Driver");
        }

        protected virtual void FixedUpdate()
        {
            if (!vehicleController.enabled)
            {
                return;
            }
            if (Vector3.Dot(vehicle.transform.up, Vector3.up) < 0.2f || (vehicle.body.velocity.magnitude > 2.5f && Vector3.Dot(vehicle.body.velocity.normalized, vehicle.transform.forward) < 0.2f))
            {
                return;
            }

            angleOfRoll = Mathf.Lerp(angleOfRoll, Mathf.Clamp( -90.0f * angleOfRollDirection * Vector3.Dot(vehicle.transform.right, Vector3.up), -deltaAngleOfRoll, deltaAngleOfRoll), 3.0f * Time.fixedDeltaTime);
            transform.localRotation = Quaternion.Euler(0.0f, 0.0f, angleOfRoll);
        }

        public void ActivatePhysics()
        {
            driverPhysics.SetActive(true);
        }
    }
}
