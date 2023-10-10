using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WheelsSystem;

public class CrashChecker : MonoBehaviour 
{
    private WheelsSystem.Driver driver;
    private WheelsSystem.Vehicle vehicle;
    private WheelsSystem.DriverPhysics driverPhysics;
    private bool isActive;
    private Renderer[] renders;
    private Collider checker;

    void Awake()
    {
        checker = GetComponent<Collider>();
        foreach (var item in transform.root.GetComponentsInChildren<Collider>())
        {
            Physics.IgnoreCollision(checker, item);
        }
        isActive = true;
        renders = transform.root.GetComponentsInChildren<Renderer>();
        driver = transform.root.GetComponentInChildren<WheelsSystem.Driver>();
        vehicle = transform.root.GetComponentInChildren<WheelsSystem.Vehicle>();
        driverPhysics = transform.root.GetComponentInChildren<WheelsSystem.DriverPhysics>();
    }

    public IEnumerator Resset()
    {
        if (!isActive)
        {
            while (Vector3.Dot(vehicle.transform.up, Vector3.up) < 0.2f)
            {
                yield return null;
            }
            StopCoroutine("WaitAndActivate");
            StartCoroutine("WaitAndActivate");
            driver.vehicleController.StopControl();
            driver.vehicleController.enabled = true;
            checker.enabled = true;
            GetComponent<Rigidbody>().isKinematic = false;
            driverPhysics.SetActive(false);
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (isActive)
        {
            isActive = false;
            if (!driver || !driver.vehicleController)
            {
                enabled = false;
                return;
            }
            driver.vehicleController.StopControl();
            driver.vehicleController.enabled = false;
            StopCoroutine("WaitAndResset");
            StartCoroutine("WaitAndResset");
            checker.enabled = false;
            GetComponent<Rigidbody>().isKinematic = true;
            driver.ActivatePhysics();
        }
    }
    void MakeTransparent(bool value)
    {
        foreach (var item in renders)
        {
            Color c = item.sharedMaterial.color;
            item.sharedMaterial.color = new Color(c.r, c.g, c.b, value? 1.0f: 0.0f);
        }
    }
    IEnumerator WaitAndActivate()
    {
        //MakeTransparent(true);
        yield return new WaitForSeconds(3.0f);
        //MakeTransparent(false);
        isActive = true;
    }
    IEnumerator WaitAndResset()
    {
        yield return new WaitForSeconds(3.0f);

        if (!isActive)
        {
            yield return StartCoroutine(Resset());
            Resset();
        }
    }
}
