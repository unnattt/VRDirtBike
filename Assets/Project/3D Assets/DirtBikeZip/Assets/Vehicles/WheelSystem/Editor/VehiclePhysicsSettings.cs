using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class VehiclePhysicsSettings : MonoBehaviour 
{
    static VehiclePhysicsSettings ()
    {
        Time.fixedDeltaTime = 0.01f;
        Physics.defaultSolverIterations = 7;
	}
}
