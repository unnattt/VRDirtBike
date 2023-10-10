using System;
using UnityEngine;

namespace Yudiz.DirtBikeVR.CoreGamePlay
{
    public class SteeringWheel : MonoBehaviour
    {
        #region PUBLIC_VARS        
        public static Action<float> OnSteer;
        [HideInInspector]
        public bool wheelBeingHeld;
        [HideInInspector]
        public float overallRotation = 0;        
        #endregion

        #region PRIVATE_VARS

        private Vector3 currentRotation;

        #endregion

        #region UNITY_CALLBACKS
        private void Start()
        {
            Debug.Log("Inside - SteeringWheel");
        }
        #endregion

        #region STATIC_FUNCTIONS
        public float Map(float value, float leftMin, float leftMax, float rightMin, float rightMax)
        {
            return rightMin + (value - leftMin) * (rightMax - rightMin) / (leftMax - leftMin);
        }
        #endregion

        #region PUBLIC_FUNCTIONS

        public void RotateSteeringWithHands(float signedAngle)
        {
            float newRawRotation = 0;
            //float rotationChange = 0;            

            newRawRotation = overallRotation + signedAngle;

            newRawRotation = Mathf.Clamp(newRawRotation, -30, 30);
            //rotationChange = newRawRotation - overallRotation;

            currentRotation = transform.localRotation.eulerAngles;
            currentRotation.y = newRawRotation;
            transform.localRotation = Quaternion.Euler(currentRotation);
            overallRotation = newRawRotation;
            float steeringRotation = Map(overallRotation, -30, 30, -1, 1);
            Debug.Log($"OverallRotation - {overallRotation}");
            Debug.Log($"YRotation - {currentRotation.y}");
            OnSteer?.Invoke(steeringRotation);
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