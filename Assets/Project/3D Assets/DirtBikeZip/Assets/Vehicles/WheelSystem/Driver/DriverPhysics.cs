using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WheelsSystem
{
    public class DriverPhysics : MonoBehaviour
    {
        [SerializeField] private WheelsSystem.Driver driver;
        [SerializeField] private DriverPhysics driverPhysics;
        [SerializeField] private Rigidbody rootBody;
        private SkinnedMeshRenderer[] ragdollRender;
        private SkinnedMeshRenderer[] driverRender;
        private Transform[] driverTransforms;
        private Transform[] ragdollTransforms;

        [SerializeField] private CrashChecker crashChecker;
        private Collider[] colliders;
        private Rigidbody[] bodyes;

        public void OnAwake()
        {
            ragdollRender = driverPhysics.GetComponentsInChildren<SkinnedMeshRenderer>();
            driverRender = driver.GetComponentsInChildren<SkinnedMeshRenderer>();
            colliders = GetComponentsInChildren<Collider>();
            bodyes = GetComponentsInChildren<Rigidbody>();
            driverTransforms = driver.GetComponentsInChildren<Transform>();
            ragdollTransforms = driverPhysics.GetComponentsInChildren<Transform>();
        }

        public void SetActive(bool value)
        {   
            foreach (var item in ragdollRender)
            {
                item.enabled = value;
            }
            foreach (var item in driverRender)
            {
                item.enabled = !value;
            }
            foreach (var item in colliders)
            {
                item.enabled = value;
            }
            foreach (var item in bodyes)
            {
                item.isKinematic = !value;
            }
            //crashChecker.gameObject.SetActive(!value);

            driverPhysics.transform.localPosition = driver.transform.localPosition;
            driverPhysics.transform.localRotation = driver.transform.localRotation;

            for (int i = 0; i < ragdollTransforms.Length; i++)
            {
                ragdollTransforms[i].localPosition = driverTransforms[i].localPosition;
                ragdollTransforms[i].localRotation = driverTransforms[i].localRotation;
            }
            if (value)
            {
                rootBody.velocity = driver.vehicleController.vehicle.body.velocity +
                    Vector3.Cross(driver.vehicleController.vehicle.body.angularVelocity, (rootBody.position - driver.vehicleController.vehicle.centerOfMass.position).normalized);
            }
          
        }

       
    }
}
