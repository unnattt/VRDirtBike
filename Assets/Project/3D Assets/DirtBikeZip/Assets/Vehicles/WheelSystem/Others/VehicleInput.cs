using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace WheelsSystem
{
    public enum  VehiclesInputType
    {
        Gas = 1,
        BackGas,
        FrontBrake,
        BackBrake,
        LeftSteering,
        RightSteering,
        ForceAccelerate
    }

    public class VehicleInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public static float gas;
        public static float frontBrake;
        public static float backBrake;
        public static float steering;
        public static float forceAccelerate;
        [SerializeField]
        private VehiclesInputType inputType;
        public static event System.Action<bool> StartBreak;
        public static event System.Action<bool> EndBreak;

        void Update()
        {
            if (!CrossPlatformControl.isMobile)
            {
                
                if (Input.GetKeyDown(KeyCode.F))
                {
                    if (StartBreak != null)
                    {
                        StartBreak(true);
                    }
                }
                else if (Input.GetKeyDown(KeyCode.B))
                {
                    if (StartBreak != null)
                    {
                        StartBreak(false);
                    }
                }
                else if (Input.GetKeyUp(KeyCode.F))
                {
                    if (EndBreak != null)
                    {
                        EndBreak(true);
                    }
                }
                else if (Input.GetKeyUp(KeyCode.B))
                {
                    if (EndBreak != null)
                    {
                        EndBreak(false);
                    }
                }
                gas = Input.GetAxis("Vertical");
                steering = Input.GetAxis("Horizontal");
                frontBrake = Input.GetKey(KeyCode.F) ? 1.0f : 0.0f;
                backBrake = Input.GetKey(KeyCode.B) ? 1.0f : 0.0f;
                forceAccelerate = Input.GetKey(KeyCode.E) ? 1.0f : 0.0f;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            switch (inputType)
            {
                case VehiclesInputType.FrontBrake:
                    frontBrake = 1.0f;
                    if (StartBreak != null)
                    {
                        StartBreak(true);
                    }
                    break;
                case VehiclesInputType.BackBrake:
                    backBrake = 1.0f;
                    if (StartBreak != null)
                    {
                        StartBreak(false);
                    }
                    break;
                case VehiclesInputType.Gas:
                    gas = 1.0f;
                    break;
                case VehiclesInputType.BackGas:
                    gas = -1.0f;
                    break;
                case VehiclesInputType.LeftSteering:
                    steering = -1.0f;
                    break;
                case VehiclesInputType.RightSteering:
                    steering = 1.0f;
                    break;
                case VehiclesInputType.ForceAccelerate:
                    forceAccelerate = 1.0f;
                    break;
                default:
                    break;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            switch (inputType)
            {
                case VehiclesInputType.FrontBrake:
                    frontBrake = 0.0f;
                    if (EndBreak != null)
                    {
                        EndBreak(true);
                    }
                    break;
                case VehiclesInputType.BackBrake:
                    backBrake = 0.0f;
                    if (EndBreak != null)
                    {
                        EndBreak(false);
                    }
                    break;
                case VehiclesInputType.Gas:
                    gas = 0.0f;
                    break;
                case VehiclesInputType.BackGas:
                    gas = 0.0f;
                    break;
                case VehiclesInputType.LeftSteering:
                    steering = 0.0f;
                    break;
                case VehiclesInputType.RightSteering:
                    steering = 0.0f;
                    break;
                case VehiclesInputType.ForceAccelerate:
                    forceAccelerate = 0.0f;
                    break;
                default:
                    break;
            }
        }
    }
}
