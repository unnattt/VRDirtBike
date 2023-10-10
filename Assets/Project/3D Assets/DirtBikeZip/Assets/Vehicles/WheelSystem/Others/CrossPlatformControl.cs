using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WheelsSystem;

public class CrossPlatformControl : MonoBehaviour {

    [SerializeField] private bool _isMobile;
    public static bool isMobile;
    [SerializeField] private GameObject pcInput;
    [SerializeField] private GameObject mobileInput;

    private Vehicle[] vehicles;
    [SerializeField] private VehicleSwitch vehicleSwitch;
    [SerializeField] private Dropdown vehiclesData;
    private CameraControl cameraControl;

    void Awake ()
    {
        cameraControl = CameraControl.FindObjectOfType<CameraControl>();
        vehicles = GameObject.FindObjectsOfType<Vehicle>();
        vehiclesData.options = new List<Dropdown.OptionData>();
        vehiclesData.RefreshShownValue();

        #if !UNITY_EDITOR
        _isMobile = Application.isMobilePlatform;
        #endif
        isMobile = _isMobile;

        pcInput.SetActive(!_isMobile);
        mobileInput.SetActive(_isMobile);
    }
    IEnumerator Start ()
    {
        yield return new WaitForEndOfFrame();
        bool findMoto = false;
        foreach (var item in vehicles)
        {
            vehiclesData.options.Add(new Dropdown.OptionData(item.name));
			if (item.name != "MotocrossBikeTemplate")
            {
                ActivateVehicle(false, item.gameObject);
            }
            else
            {
                findMoto = true;
            }
        }
        if (!findMoto)
        {
            foreach (var item in vehicles)
            {
				if (item.name == "SportQuadTemplate")
                {
                    ActivateVehicle(true, item.gameObject);
                    findMoto = true;
                }
            }
        }
    }
    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            cameraControl.isFollow = !cameraControl.isFollow;
        }
    }
    public void SwichVehicles()
    {
        vehicleSwitch.SwitchVehicle(vehicles[vehiclesData.value]);
        for (int i = 0; i < vehicles.Length; i++)
        {
            ActivateVehicle(i == vehiclesData.value, vehicles[i].gameObject);
        }
    }
    void ActivateVehicle(bool isOn, GameObject vehicle)
    {
        vehicle.SetActive(isOn);
    }
}
