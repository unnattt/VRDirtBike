using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace Yudiz.DirtBikeVR.Managers
{
    public class InputController : MonoBehaviour
    {
        #region PUBLIC_VARS
        public static InputController instance;
        //public static Action<float> OnSteer;
        //public static Action OnRotateSteering;
        public static Action<float> OnAccelarate;
        //public static Action<float> OnReverseOrBrake;
        [SerializeField] private Canvas canvas;
        public static Action OnBreaking;

        public static Action OnCarStart;

        public static Action OnResetCarPos;

        public static Action<bool> ParticalPlayStop;

        bool isButtonOn;

        [HideInInspector]
        public float InputValue;        
        #endregion

        #region PRIVATE_VARS      
        [SerializeField] private InputActionReference leftHandActivate;
        [SerializeField] private InputActionReference rightHandActivate;
        [SerializeField] private InputActionReference ResetBikePos;
        [SerializeField] private InputActionReference TutorialImage;        
        #endregion

        #region UNITY_CALLBACKS
        private void Awake()
        {
            instance = this;            
        }
        private void OnEnable()
        {
            ResetBikePos.action.performed += ResetBikePosition;
            TutorialImage.action.performed += DisableImage;
        }

        private void DisableImage(InputAction.CallbackContext obj)
        {
            //throw new NotImplementedException();
            if(isButtonOn)
            {

            canvas.enabled = false;
                isButtonOn = false;
            }
            else
            {
                canvas.enabled = true;
                isButtonOn = true;
            }
        }

        private void ResetBikePosition(InputAction.CallbackContext obj)
        {
            OnResetCarPos?.Invoke();
        }

        private void Update()
        {
            //Debug.LogWarning(leftHandActivate.action.ReadValue<float>());
            //Debug.LogError(rightHandActivate.action.ReadValue<float>());

            if (rightHandActivate.action.ReadValue<float>() == leftHandActivate.action.ReadValue<float>())
            {
                OnAccelarate?.Invoke(0);
                OnCarStart?.Invoke();
                ParticalPlayStop?.Invoke(false);
            }
            else
            {
                if (leftHandActivate.action.ReadValue<float>() > 0)
                {
                    //OnCarStart?.Invoke();
                    OnAccelarate?.Invoke(1);
                    OnCarStart?.Invoke();
                    ParticalPlayStop?.Invoke(true);
                }

                if (rightHandActivate.action.ReadValue<float>() > 0)
                {
                    //OnCarStart?.Invoke();
                    OnAccelarate?.Invoke(-1);
                    OnCarStart?.Invoke();
                    //ParticalPlayStop?.Invoke(true, false);
                    ParticalPlayStop?.Invoke(false);
                }
            }
        }
        #endregion

        #region STATIC_FUNCTIONS        
        #endregion

        #region PUBLIC_FUNCTIONS       
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