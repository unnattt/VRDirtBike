using UnityEngine;
using UnityEngine.XR;
using Yudiz.DirtBikeVR.Managers;
using UnityEngine.XR.Interaction.Toolkit;
using System;

namespace Yudiz.DirtBikeVR.CoreGamePlay
{
    public class Hands : MonoBehaviour
    {
        #region PUBLIC_VARS
        //public Transform leftHand;
        //public Transform rightHand;
        private Vector3 bothHandIntitialPosManual;
        #endregion

        #region PRIVATE_VARS        
        [SerializeField] private Transform steeringReference;
        [SerializeField] private SteeringWheel steeringWheel;

        [SerializeField] private Transform leftHandController;
        [SerializeField] private Transform rightHandController;

        [SerializeField] private XRSimpleInteractable leftGrabPoint;
        [SerializeField] private XRSimpleInteractable rightGrabPoint;

        private IXRSelectInteractor controller1;
        private IXRSelectInteractor controller2;

        private Vector3 bothHandIntitialPos;
        private Vector3 oneHandInitialPos;

        private Vector3 initialController1Position;
        private Vector3 initialController2Position;
        private Quaternion initialSteeringWheelRotation;

        public Transform middleHoldPoint;


        private void BothHandInitialPosition2()
        {
            Debug.Log("Calculating BothHand Position");
            initialController1Position = controller1.transform.position; // right hand
            initialController2Position = controller2.transform.position; // Left hand
            initialSteeringWheelRotation = steeringWheel.transform.rotation;
        }
        #endregion

        #region UNITY_CALLBACKS
        private void OnEnable()
        {
            leftGrabPoint.selectEntered.AddListener(OnLeftGrabbed);
            leftGrabPoint.selectExited.AddListener(OnLeftReleased);

            rightGrabPoint.selectEntered.AddListener(OnRightGrabbed);
            rightGrabPoint.selectExited.AddListener(OnRightReleased);
        }

        private void OnLeftGrabbed(SelectEnterEventArgs arg0)
        {
            Debug.Log("On Left Grabbed");
            leftHandController = arg0.interactorObject.transform;
        }

        private void OnLeftReleased(SelectExitEventArgs arg0)
        {
            Debug.Log("On Left Released");
            leftHandController = null;
        }

        private void OnRightGrabbed(SelectEnterEventArgs arg0)
        {
            rightHandController = arg0.interactorObject.transform;
        }

        private void OnRightReleased(SelectExitEventArgs arg0)
        {
            rightHandController = null;
        }

        public float rotateSpeed = 1;
        private void FixedUpdate()
        {
            CalculateHandAngle();
        }
        #endregion

        #region STATIC_FUNCTIONS
        #endregion

        #region PUBLIC_FUNCTIONS
        #endregion

        #region PRIVATE_FUNCTIONS        
        private void CalculateHandAngle()
        {
            if (leftHandController != null && rightHandController != null)
            {


                //Vector3 newDifference = controller1.transform.position - controller2.transform.position;
                Vector3 newDifference = rightHandController.transform.position - leftHandController.transform.position;
                newDifference.y = 0;
                Vector3 steeringRight = steeringReference.right;
                steeringRight.y = 0;
                float angle = Vector3.SignedAngle(steeringRight, newDifference, steeringReference.up);
                steeringWheel.RotateSteeringBikeWithHands(angle);
            }
            else if ((leftHandController != null && rightHandController == null) || (leftHandController == null && rightHandController != null))
            {

                Vector3 newDifference;
                if (leftHandController != null)
                {
                    newDifference = (middleHoldPoint.position - leftHandController.transform.position).normalized;
                }
                else
                {
                    newDifference = (rightHandController.transform.position - middleHoldPoint.position).normalized;
                }
                Vector3 steeringRight = steeringReference.right;
                steeringRight.y = 0;

                newDifference.y = 0;
                float angle = Vector3.SignedAngle(steeringRight, newDifference, steeringReference.up);
                steeringWheel.RotateSteeringBikeWithHands(angle);
            }
        }


        #endregion

    }
}