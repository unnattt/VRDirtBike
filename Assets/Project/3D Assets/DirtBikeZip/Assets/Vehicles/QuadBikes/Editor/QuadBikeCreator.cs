using UnityEngine;
using UnityEditor;
using System;
using WheelsSystem;

public class QuadBikeCreator : VehicleCreator
{
    protected int angularAbsorberCount;
    protected Transform[] angularAbsorbers;
    protected bool[] useSprings;
    protected Spring[] angularAbsorbersSprings;
    protected AngularAbsorber.Parameters[] angularAbsorberParameters;

    [MenuItem("GameObject/3D Object/Vehicles/Quad bike")]
    static void Init()
    {
        window = (QuadBikeCreator)EditorWindow.GetWindow(typeof(QuadBikeCreator), true, "Quad bike creator", true);
        ShowWindow();
    }

    protected override void OnEnable ()
    {
        base.OnEnable();
        angularAbsorberParameters = new AngularAbsorber.Parameters[0];
        useSprings = new bool[0];
        angularAbsorbersSprings = new Spring[0];
    }

    protected override void OnDisable ()
    {
        base.OnDisable();
        foreach (var item in angularAbsorbersSprings)
        {
            DestroyImmediate(item);
        }
    }

    protected override void SetAbsorbers()
    {
        angularAbsorberCount = EditorGUILayout.IntField("Angular absorbers count", angularAbsorberCount);
        if (angularAbsorbers != null)
        {
            for (int i = 0; i < angularAbsorbers.Length; i++)
            {
                angularAbsorbers[i] = EditorGUILayout.ObjectField("  Angular absorbers", angularAbsorbers[i], typeof(Transform), true) as Transform;
                if (angularAbsorbers[i])
                {
                    AngularAbsorber.Parameters angularAbsorberParameter = angularAbsorberParameters[i];

                    useSprings[i] = EditorGUILayout.Toggle("     Use spring details", useSprings[i]);

                    if (useSprings[i])
                    {
                        Spring angularAbsorbersSpring = angularAbsorbersSprings[i];
                        if (angularAbsorbersSpring == null)
                        {
                            angularAbsorbersSpring = angularAbsorbers[i].gameObject.AddComponent<Spring>();
                        }

                        EditorGUILayout.LabelField("");
                        angularAbsorbersSpring.springDownDetail = EditorGUILayout.ObjectField("     Spring down detail", angularAbsorbersSpring.springDownDetail, typeof(Transform), true) as Transform;
                        angularAbsorbersSpring.springUpDetail = EditorGUILayout.ObjectField("     Spring up detail", angularAbsorbersSpring.springUpDetail, typeof(Transform), true) as Transform;
                        angularAbsorbersSpring.downPivot = EditorGUILayout.ObjectField("     Spring up pivot", angularAbsorbersSpring.downPivot, typeof(Transform), true) as Transform;
                        angularAbsorbersSpring.upPivot = EditorGUILayout.ObjectField("     Spring down pivot", angularAbsorbersSpring.upPivot, typeof(Transform), true) as Transform;
                        angularAbsorbersSpring.spring = EditorGUILayout.ObjectField("     Spring", angularAbsorbersSpring.spring, typeof(Transform), true) as Transform;
                        angularAbsorbersSprings[i] = angularAbsorbersSpring;
                    }
                    else if(angularAbsorbers[i].gameObject.GetComponent<Spring>())
                    {
                        DestroyImmediate(angularAbsorbers[i].gameObject.GetComponent<Spring>());
                    }

                    if (angularAbsorberParameter.name + "" == "")
                    {
                        angularAbsorberParameter = new AngularAbsorber.Parameters(angularAbsorbers[i].name, 1000.0f, 5.0f, 40.0f, 20.0f);
                    }
                    angularAbsorberParameter.name = angularAbsorbers[i].name;
                    EditorGUILayout.LabelField("     parameters");
                    angularAbsorberParameter.torque = EditorGUILayout.FloatField("     Torque", angularAbsorberParameter.torque);
                    angularAbsorberParameter.damper = EditorGUILayout.FloatField("     Damper", angularAbsorberParameter.damper);
                    angularAbsorberParameter.limit = EditorGUILayout.FloatField("     Limit", angularAbsorberParameter.limit);
                    angularAbsorberParameter.mass = EditorGUILayout.FloatField("     Mass", angularAbsorberParameter.mass);
                    EditorGUILayout.LabelField("");
                    angularAbsorberParameters[i] = angularAbsorberParameter;
                }
            }
        }
    }
    protected override void TryToSetOthersFromEditorWindow()
    {
        QuadBike quad = (QuadBike)vehicle;
        quad.TryToSetAngularAbsorbersFromEditorWindow(angularAbsorbers, angularAbsorbersSprings, angularAbsorberParameters);
    }
    protected override void EditMasField()
    {
        EditMasField(ref angularAbsorbers, angularAbsorberCount);
        EditMasField(ref angularAbsorberParameters, angularAbsorberCount);
        EditMasField(ref useSprings, angularAbsorberCount);
        EditMasField(ref angularAbsorbersSprings, angularAbsorberCount);
        base.EditMasField();
    }
}
