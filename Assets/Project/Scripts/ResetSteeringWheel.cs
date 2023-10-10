using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yudiz.DirtBikeVR.CoreGamePlay
{
    public class ResetSteeringWheel : MonoBehaviour
    {
        #region PUBLIC_VARS
        #endregion

        #region PRIVATE_VARS
        [SerializeField] private CarController carController;
        [SerializeField] private SteeringWheel steeringWheel;
        [SerializeField] private AnimationCurve steeringRotationCurve;
        //[SerializeField] private float carSpeed;
        private float steeringRotateStep = 10;
        private float rotationSpeed;
        #endregion

        #region UNITY_CALLBACKS
        private void FixedUpdate()
        {
            SteeringRotation();
            ResetSteering();
        }
        #endregion

        #region PUBLIC_FUNCTIONS
        #endregion

        #region PRIVATE_FUNCTIONS
        private void SteeringRotation()
        {
            //Debug.Log("OverAll Rotation : " + steeringWheel.overallRotation);
            //Debug.LogWarning("Current Speed : " + carController.CarSpeedRigidBody());
            float currentSpeed = steeringWheel.Map(carController.CarSpeedRigidBody(), 0, carController.maxSpeed, 0, 1);
            float curveValue = steeringRotationCurve.Evaluate(currentSpeed);
            //Debug.LogWarning("Curve Value : " + curveValue);
            rotationSpeed =  steeringRotateStep * curveValue;
            //Debug.LogWarning("Rotation Speed: " + rotationSpeed);
        }

        private void ResetSteering()
        {
            if(!steeringWheel.wheelBeingHeld && steeringWheel.overallRotation > 5)
            {
                Debug.Log("Reseting Steering");
                steeringWheel.RotateSteeringWithHands(rotationSpeed * -1);
            }
            else if(!steeringWheel.wheelBeingHeld && steeringWheel.overallRotation < -5)
            {
                Debug.Log("Reseting Steering");
                steeringWheel.RotateSteeringWithHands(rotationSpeed);
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
