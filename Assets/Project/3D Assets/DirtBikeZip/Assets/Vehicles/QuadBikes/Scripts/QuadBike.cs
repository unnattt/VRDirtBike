using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WheelsSystem
{
    [AddComponentMenu("Physics/Wheels System/Vehicles/Quad bike")]
    public class QuadBike : Vehicle 
    {
        [SerializeField] private List<AngularAbsorber.Parameters> angularAbsorbersParameters;

        [SerializeField] private List<AngularAbsorber> angularAbsorbers;

        protected override void SetAbsorbers()
        {
            SetAngularAbsorbers();
        }

        protected override void UpdateAbsorbers()
        {
            UpdateAngularAbsorbers();
        }

        void SetAngularAbsorbers()
        {
            angularAbsorbersParameters = new List<AngularAbsorber.Parameters>(0);
            angularAbsorbers = new List<AngularAbsorber>(0);
            foreach (var item in transform.GetComponentsInChildren<Transform>())
            {
                if (!item.name.Contains("Joint") && item.name.Contains("AngularAbsorber"))
                {
                    AngularAbsorber aAbsorber = item.gameObject.GetComponent<AngularAbsorber>();
                    if (!aAbsorber)
                    {
                        aAbsorber = item.gameObject.AddComponent<AngularAbsorber>();
                    }
                    angularAbsorbers.Add(aAbsorber);
                    angularAbsorbersParameters.Add(aAbsorber.parameters);
                }
            }
        }

        void UpdateAngularAbsorbers()
        {
            if (angularAbsorbers == null)
            {
                return;
            }
            int i = 0;
            foreach (AngularAbsorber angularAbsorber in angularAbsorbers)
            {
                AngularAbsorber.Parameters aAbsorberParameters = angularAbsorbersParameters[i];
                angularAbsorber.parameters = aAbsorberParameters;
                if (aAbsorberParameters.name != angularAbsorber.name)
                {
                    aAbsorberParameters = new AngularAbsorber.Parameters(angularAbsorber.name, aAbsorberParameters.torque, aAbsorberParameters.damper, aAbsorberParameters.limit, aAbsorberParameters.mass);
                    angularAbsorbersParameters[i] = aAbsorberParameters;
                }
                i++;
            }
        }

        public void TryToSetAngularAbsorbersFromEditorWindow(Transform[] angularAbsorbers, Spring[] angularAbsorbersSprings, AngularAbsorber.Parameters[] angularAbsorberParameters)
        {
            int i = 0;
            if (angularAbsorbers != null)
            {
                this.angularAbsorbersParameters = new List<AngularAbsorber.Parameters>(0);
                this.angularAbsorbers = new List<AngularAbsorber>(0);
                foreach (var item in angularAbsorbers)
                {
                    AngularAbsorber aAbsorber = item.gameObject.GetComponent<AngularAbsorber>();
                    if (!aAbsorber)
                    {
                        aAbsorber = item.gameObject.AddComponent<AngularAbsorber>();
                    }
                    if (angularAbsorbersSprings != null && i < angularAbsorbersSprings.Length)
                    {
                        Spring spring = angularAbsorbersSprings[i];
                        if (spring)
                        {
                            aAbsorber.springDetail.TryToSetFromEditorWindow(spring.springDownDetail, spring.springUpDetail, spring.downPivot, spring.upPivot, spring.spring);
                        }
                    }

                    this.angularAbsorbers.Add(aAbsorber);
                    this.angularAbsorbersParameters.Add(angularAbsorberParameters[i]);
                    i++;
                }
            }
        }
    }
}
