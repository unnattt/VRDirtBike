using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Yudiz.CarVR.CoreGamePlay
{
    public class BreakingStick : MonoBehaviour
    {
        #region PUBLIC_VARS
        public static Action OnBraking;
        #endregion

        #region PRIVATE_VARS
        private IXRSelectInteractor controller1;
        #endregion

        #region UNITY_CALLBACKS
        private void Update()
        {
            if (controller1 != null)
            {
                OnBraking?.Invoke();
            }            
        }
        #endregion

        #region PUBLIC_FUNCTIONS
        public void OnEntered(SelectEnterEventArgs eventArgs)
        {
            //OnBraking?.Invoke();
            Debug.Log("Inside - OnEntered");
            if (controller1 == null)
            {
                Debug.Log("Controller1 is Touching");
                controller1 = eventArgs.interactorObject;
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
        }
        #endregion

        #region PRIVATE_FUNCTIONS
        #endregion

        #region CO-ROUTINES
        #endregion

        #region EVENT_HANDLERS
        #endregion

        #region UI_CALLBACKS
        #endregion
    }
}
