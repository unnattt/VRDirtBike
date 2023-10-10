using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WheelsSystem;

public class VehicleResset : MonoBehaviour
{
    private CrashChecker[] crashCheckers;
    private Vector3 checkPosition;
    private Quaternion checkRotation;

    void Awake()
    {
        crashCheckers = CrashChecker.FindObjectsOfType<CrashChecker>();
    }

    void OnEnable()
    {
        StartCoroutine(StartCheck());
    }
    IEnumerator StartCheck ()
    {
        while (true)
        {
            yield return new WaitForSeconds(3.0f);
            if (Vector3.Dot(transform.up, Vector3.up) < 0.2f)
            {
                yield return StartCoroutine(Check());
            }
            else if (Vector3.Dot(transform.up, Vector3.up) > 0.5f)
            {
                checkPosition = transform.position;
                checkRotation = transform.rotation;
            }
        }
	}
	
    IEnumerator Check()
    {
        yield return new WaitForSeconds(2.0f);
        if (Vector3.Dot(transform.up, Vector3.up) < 0.2f)
        {
            Resset();
        }
    }
    void Resset()
    {
        TriggerColliders(false);
        transform.position = checkPosition + 2.0f * Vector3.up;
        transform.rotation = checkRotation;

        foreach (var item in crashCheckers)
        {
            StartCoroutine(item.Resset());
        }
        TriggerColliders(true);
    }
    void TriggerColliders(bool isOn)
    {
        foreach (var item in GetComponentsInChildren<Collider>())
        {
            item.enabled = isOn;
        }
    }
}
