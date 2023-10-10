using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WheelsSystem;

public class VehicleSwitch : MonoBehaviour 
{
    [SerializeField] private CameraControl cameraControl;

    public void SwitchVehicle(Vehicle vehicle)
    {
        cameraControl.target = vehicle.transform.Find("CameraTarget");
    }
}
