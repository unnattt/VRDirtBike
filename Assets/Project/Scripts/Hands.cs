using System;
using UnityEngine;
//using Yudiz.CarVR.Managers;
using UnityEngine.XR.Interaction.Toolkit;

namespace Yudiz.DirtBikeVR.CoreGamePlay
{
    public class Hands : MonoBehaviour
    {
        #region PUBLIC_VARS
        //public static event Action OnInput;
        #endregion

        #region PRIVATE_VARS        
        [SerializeField] private SteeringWheel steeringWheel;

        private IXRSelectInteractor controller1;
        private IXRSelectInteractor controller2;

        private Vector3 bothHandIntitialPos;
        private Vector3 oneHandInitialPos;
        //private Vector3 initialHandPos;
        #endregion

        #region UNITY_CALLBACKS
        private void Start()
        {
            Debug.Log("Inside - HandsScript");
            
            //if (steeringWheel == null) { Debug.Log("Steering Wheel Empty"); }

        }
        private void Update()
        {
            CalculateHandAngle();
            //CalculateHandAngle2();
        }
        #endregion

        #region STATIC_FUNCTIONS
        #endregion

        #region PUBLIC_FUNCTIONS
        private void BothHandInitialPosition()
        {
            Debug.Log("Calculating BothHand Position");
            bothHandIntitialPos = controller1.transform.position - controller2.transform.position;
        }

        private void OneHandInitialPosition()
        {
            Debug.Log("Calculating OneHand Position");
            oneHandInitialPos = transform.position - controller1.transform.position;
        }

        public void OnEntered(SelectEnterEventArgs eventArgs)
        {
            Debug.Log("Inside - OnEntered");
            if (controller1 == null)
            {
                Debug.Log("Controller1 is Touching");
                controller1 = eventArgs.interactorObject;
                //steeringWheel.wheelBeingHeld = true;
                OneHandInitialPosition();
                //OnInput?.Invoke();
            }
            else
            {
                Debug.Log("Controller2 is Touching");
                controller2 = eventArgs.interactorObject;
                //steeringWheel.wheelBeingHeld = true;
                BothHandInitialPosition();
                //OnInput?.Invoke();
            }
        }

        public void OnExit(SelectExitEventArgs eventArgs)
        {
            Debug.Log("Inside - OnExit");
            if (controller1 == eventArgs.interactorObject)
            {                
                Debug.Log("Controller1 has Exited");
                //steeringWheel.wheelBeingHeld = false;
                controller1 = null;
            }
            else
            {
                Debug.Log("Controller2 has Exited");
                //steeringWheel.wheelBeingHeld = false;
                controller2 = null;
            }
        }
        #endregion

        #region PRIVATE_FUNCTIONS        
        private void CalculateHandAngle()
        {
            if (controller1 != null && controller2 != null)
            {
                Debug.Log("BothHands on Steering");
                steeringWheel.wheelBeingHeld = true;
                Vector3 newDifference = controller1.transform.position - controller2.transform.position;
                float angle = Vector3.SignedAngle(bothHandIntitialPos, newDifference, transform.up);
                //Debug.LogWarning($"Angle:: {angle}");
                //steeringWheel.RotateSteeringWheelWithHands(angle);
                Debug.Log("Angle(In Both Hands) : " + angle);
                steeringWheel.RotateSteeringWithHands(angle);
                bothHandIntitialPos = newDifference;
            }
            else if (controller1 != null && controller2 == null)
            {
                Debug.Log("One hand on Steering");
                steeringWheel.wheelBeingHeld = true;
                Vector3 newDifference = transform.position - controller1.transform.position;
                float angle = Vector3.SignedAngle(oneHandInitialPos, newDifference, transform.up);
                //steeringWheel.RotateSteeringWheelWithHands(angle);
                Debug.Log("Angle(In One Hands) : " + angle);
                steeringWheel.RotateSteeringWithHands(angle);
                oneHandInitialPos = newDifference;
            }
            else if (controller1 == null && controller2 == null)
            {
                //steeringWheel.ResetSteeringWheel();
                steeringWheel.wheelBeingHeld = false;
                //Vector3 newDifference = initialHandPos;
                //float angle = Vector3.SignedAngle(oneHandInitialPos, newDifference, transform.up);
                ////steeringWheel.RotateSteeringWheelWithHands(angle);
                //Debug.Log("Angle(In One Hands) : " + angle);
                //steeringWheel.RotateSteeringWithHands(angle);
                //oneHandInitialPos = newDifference;
            }
            else if (controller1 == null && controller2 != null)
            {
                steeringWheel.wheelBeingHeld = true;
                Vector3 newDifference = transform.position - controller2.transform.position;
                float angle = Vector3.SignedAngle(oneHandInitialPos, newDifference, transform.up);
                //steeringWheel.RotateSteeringWheelWithHands(angle);
                Debug.Log("Angle(In One Hands) : " + angle);
                steeringWheel.RotateSteeringWithHands(angle);
                oneHandInitialPos = newDifference;
            }
        }


        #endregion

        #region CO-ROUTINES
        #endregion

        #region EVENT_HANDLERS
        #endregion

        #region UI_CALLBACKS
        #endregion
    }
}