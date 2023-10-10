using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WheelsSystem
{
    public class QuadDriver : Driver
    {  
        [SerializeField] protected float forceAccelerateFactor = 10.0f;
        [SerializeField] protected float frontBrakeFactor = 8.0f;
        protected VehiclesInputType inputType;
        protected float stoppieFactor;
        protected float wheelieFactor;
        protected float driftFactor;

        protected virtual void Start ()
        {
            vehicleController.OnInput += VehicleController_OnInput;
        }
        void VehicleController_OnInput (VehiclesInputType inputType)
        {
            this.inputType = inputType;
        }
        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!vehicleController.enabled)
            {
                
                return;
            }
            if (Vector3.Dot(vehicle.transform.up, Vector3.up) < 0.2f || (vehicle.body.velocity.magnitude > 2.5f && Vector3.Dot(vehicle.body.velocity.normalized, vehicle.transform.forward) < 0.2f))
            {
                if ((vehicle.body.velocity.magnitude > 2.5f && Vector3.Dot(vehicle.body.velocity.normalized, vehicle.transform.forward) < 0.2f))
                {
                    vehicle.BackBrake(1.0f);
                    return;
                }
            }
            if (vehicle.body.velocity.magnitude >= 2.5f)
            {
                if (inputType == VehiclesInputType.ForceAccelerate)
                {
                    vehicle.body.AddRelativeTorque(-vehicle.body.mass * forceAccelerateFactor * Vector3.Dot(vehicle.transform.up, Vector3.up) * Vector3.right, ForceMode.Force);
                }
                else if (inputType == VehiclesInputType.FrontBrake)
                {
                    vehicle.body.AddRelativeTorque(vehicle.body.mass * frontBrakeFactor * Vector3.Dot(vehicle.transform.up, Vector3.up) * Vector3.right, ForceMode.Force);
                }   

                if (Vector3.Dot(vehicle.transform.forward, Vector3.up) > 0.7f)
                {
                    vehicle.body.AddRelativeTorque(3.0f * vehicle.body.mass * forceAccelerateFactor * Vector3.Dot(vehicle.transform.up, Vector3.up) * Vector3.right, ForceMode.Force);
                }
                else if (Vector3.Dot(vehicle.transform.forward, Vector3.up) < -0.7f)
                {
                    vehicle.body.AddRelativeTorque(-3.0f * vehicle.body.mass * forceAccelerateFactor * Vector3.Dot(vehicle.transform.up, Vector3.up) * Vector3.right, ForceMode.Force);
                }
            }
        }
    }
}
