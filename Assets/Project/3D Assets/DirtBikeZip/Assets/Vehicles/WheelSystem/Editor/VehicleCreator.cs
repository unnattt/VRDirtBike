using UnityEngine;
using UnityEditor;
using System;
using WheelsSystem;

public abstract class VehicleCreator : EditorWindow
{
    Vector2 scrollPos;
    Transform vehicleBody;
    Transform centerOfMass;
    Transform bodyCollider;
    protected Vehicle vehicle;
    Vehicle.Parameters vehicleBodyParameters;
    Transform steering;
    Steering.Parameters steeringParameters;
    int wheelCount;
    Transform[] wheels;
    Wheel.Parameters[] wheelParameters;
    PhysicMaterial physicMaterial;
    protected static VehicleCreator window;

    protected static void ShowWindow ()
    {
        window.minSize = new Vector2(512.0f, 550.0f);
        window.Show();
    }
    protected virtual void OnEnable ()
    {
        vehicleBodyParameters = new Vehicle.Parameters(10.0f, 1.5f, 55.0f, 300.0f);
        steeringParameters = new Steering.Parameters(name, 10000.0f, 50.0f, 5.0f, 30.0f, 20.0f);
        wheelParameters = new Wheel.Parameters[0];
    }
    protected virtual void OnDisable ()
    {
        
    }
    void OnGUI()
    {
        Event e = Event.current;
        if(e.keyCode == KeyCode.Return) 
        {
            EditMasField();
        }
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width (512), GUILayout.Height (550));
        EditorGUILayout.LabelField("     ");
        vehicleBody = EditorGUILayout.ObjectField("Vehicle", vehicleBody, typeof(Transform), true) as Transform;

        if (vehicleBody)
        {
            centerOfMass = EditorGUILayout.ObjectField("Center of mass", centerOfMass, typeof(Transform), true) as Transform;
            bodyCollider = EditorGUILayout.ObjectField("Body collider", bodyCollider, typeof(Transform), true) as Transform;
            EditorGUILayout.LabelField("     " + vehicleBody.name + " parameters");
            EditorGUILayout.LabelField("     ");
            vehicleBodyParameters.maxVelocityMPS = EditorGUILayout.FloatField("     Max velocity MPS", vehicleBodyParameters.maxVelocityMPS);
            vehicleBodyParameters.acceleration = EditorGUILayout.FloatField("     Acceleration", vehicleBodyParameters.acceleration);
            vehicleBodyParameters.horsepower = EditorGUILayout.FloatField("     Horsepower", vehicleBodyParameters.horsepower);
            vehicleBodyParameters.mass = EditorGUILayout.FloatField("     Mass", vehicleBodyParameters.mass);
            EditorGUILayout.LabelField("     ");
        }

        steering = EditorGUILayout.ObjectField("Steering", steering, typeof(Transform), true) as Transform;
        if (steering)
        {
            EditorGUILayout.LabelField("     " + steering.name + " parameters");
            EditorGUILayout.LabelField("     ");
            steeringParameters.name = steering.name;
            steeringParameters.torque = EditorGUILayout.FloatField("     Torque", steeringParameters.torque);
            steeringParameters.velocity = EditorGUILayout.FloatField("     Velocity", steeringParameters.velocity);
            steeringParameters.damper = EditorGUILayout.FloatField("     Damper", steeringParameters.damper);
            steeringParameters.limit = EditorGUILayout.FloatField("     Limit", steeringParameters.limit);
            steeringParameters.mass = EditorGUILayout.FloatField("     Mass", steeringParameters.mass);
        }
        EditorGUILayout.LabelField("     ");
        SetAbsorbers();
        EditorGUILayout.LabelField("");
        wheelCount = EditorGUILayout.IntField("Wheels count", wheelCount);

        if (wheels != null)
        {
            if (!physicMaterial)
            {
                physicMaterial = (PhysicMaterial)AssetDatabase.LoadAssetAtPath("Assets/Vehicles/WheelSystem/PhysicsMaterials/Wheel.physicMaterial", typeof(PhysicMaterial));
            }
            for (int i = 0; i < wheels.Length; i++)
            {
                wheels[i] = EditorGUILayout.ObjectField("     Wheel", wheels[i], typeof(Transform), true) as Transform;

                if (wheels[i])
                {
                    Wheel.Parameters wheelParameter = wheelParameters[i];

                    if (wheelParameter.name + "" == "")
                    {
                        wheelParameter = new Wheel.Parameters(wheelParameters[i].name, true, true, 10.0f, 0.0f, 0.0f, physicMaterial);
                    }
                    wheelParameter.name = wheelParameters[i].name;
                    EditorGUILayout.LabelField("     parameters");

                    wheelParameter.useMotor = EditorGUILayout.Toggle("     Use motor", wheelParameter.useMotor);
                    wheelParameter.useBrake = EditorGUILayout.Toggle("     Use brake", wheelParameter.useBrake);
                    wheelParameter.mass = EditorGUILayout.FloatField("     Mass", wheelParameter.mass);
                    wheelParameter.deltaThickness = EditorGUILayout.FloatField("     Delta thickness", wheelParameter.deltaThickness);
                    wheelParameter.deltaRadius = EditorGUILayout.FloatField("     Delta radius", wheelParameter.deltaRadius);
                    wheelParameter.physicMaterial = EditorGUILayout.ObjectField("     Physic material", wheelParameter.physicMaterial, typeof(PhysicMaterial), true) as PhysicMaterial;

                    EditorGUILayout.LabelField("");
                    wheelParameters[i] = wheelParameter;
                }
            }
        }
        EditorGUILayout.EndScrollView();
        EditorGUILayout.LabelField("     ");
        EditorGUILayout.LabelField("     ");
        if (GUILayout.Button("Create"))
        {
            if (vehicleBody)
            {
                Vehicle.createInEditorWindow = true;
                vehicle = vehicleBody.gameObject.GetComponent<Vehicle>();
                if (vehicle)
                {
                    window.Close();
                    return;
                }
                vehicle = vehicleBody.gameObject.AddComponent<Vehicle>();
                vehicle.TryToSetBodyFromEditorWindow(centerOfMass, bodyCollider, vehicleBodyParameters, steering, steeringParameters);
                TryToSetOthersFromEditorWindow();
                vehicle.TryToSetWheelsFromEditorWindow(wheels, wheelParameters);
                Vehicle.createInEditorWindow = false;
            }
            window.Close();
        }
        EditorGUILayout.LabelField("     ");

    }

    protected abstract void SetAbsorbers();
    protected abstract void TryToSetOthersFromEditorWindow();
    protected virtual void EditMasField()
    {
        EditMasField(ref wheels, wheelCount);
        EditMasField(ref wheelParameters, wheelCount);
    }

    protected void EditMasField<T>(ref T[] source, int count)
    {
        T[] currentMas = source;
        source = new T[count];
        if (currentMas != null)
        {
            int length = Mathf.Min(currentMas.Length, source.Length);
            Array.Copy(currentMas, source, length);
        }
    }
}
