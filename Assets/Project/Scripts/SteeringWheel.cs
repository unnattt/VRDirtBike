using System;
using System.Collections;
using UnityEngine;
using Yudiz.DirtBikeVR.Managers;

namespace Yudiz.DirtBikeVR.CoreGamePlay
{
    public class SteeringWheel : MonoBehaviour
    {
        #region PUBLIC_VARS        
        public static Action<float> OnSteer;        
        [HideInInspector]
        public bool wheelBeingHeld;
        #endregion

        #region PRIVATE_VARS
        [SerializeField] private CarController carController;

        private float maxRotation = -180;
        private float minRotation = 180;

        private float rotationValue;
        private Vector3 currentRotation;
        public float overallRotation = 0;
        //public float yolo;

        //float wheelAngle;
        //float wheelReleasedSpeed = 350f;
        //float maximumSteeringAngle = 90f;
        //float valueMultiplier = 1f;

        #endregion

        #region UNITY_CALLBACKS
        private void Start()
        {            
            Debug.Log("Inside - SteeringWheel");
        }

        //private void Update()
        //{
        //    //Debug.Log($"Rotation - {overallRotation}");
        //}

        //private void Update()
        //{
        //    //ClampSteeringWheelRotation();
        //    //ResetSteeringWheelTest();
        //}

        //private void Update()
        //{
        //    RotateSteeringWheelWithHands(transform.rotation.z);            
        //}

        //private void FixedUpdate()
        //{
        //    RotatingSteering();
        //}
        #endregion

        #region STATIC_FUNCTIONS
        public float Map(float value, float leftMin, float leftMax, float rightMin, float rightMax)
        {
            return rightMin + (value - leftMin) * (rightMax - rightMin) / (leftMax - leftMin);
        }        
      
        #endregion

        #region PUBLIC_FUNCTIONS

        //public void ResetSteeringWheel()
        //{
        //    StartCoroutine(nameof(ResetSteeringWheelCoroutine));
        //}

        //private void RotateSteeringWheelWithHands(float angle)
        //{
        //    StopAllCoroutines();
        //    Debug.Log($"Angle:: {angle}");
        //    currentRotation = transform.rotation.eulerAngles;
        //    //if (currentRotation.z > 180 && currentRotation.z < 361)
        //    //{
        //    //    currentRotation.z = Map(transform.rotation.eulerAngles.z, 360, 180, 0, -180);
        //    //}
        //    //else
        //    //{
        //    //    currentRotation.z = transform.rotation.eulerAngles.z;
        //    //}
        //    //currentRotation.z = Mathf.Clamp(currentRotation.z + angle,-180,180);
        //    currentRotation.z += angle;

        //    transform.rotation = Quaternion.Euler(currentRotation);
        //    Debug.Log($"transformRotEuler: {Quaternion.Euler(currentRotation)}");
        //    Debug.Log($"currentRot.z: {currentRotation.z}");


        //    //transform.Rotate(Vector3.forward + currentRotation);
        //    float rotation = currentRotation.z;
        //    //rotation = (rotation + 180) % 360 - 180;
        //    float steeringRotation = Map(rotation, minRotation, maxRotation, -1, 1);
        //    //float steeringRotation = Map2(rotation, );
        //    //wheelAngle = rotation;
        //    //Debug.Log($"<size=20><color=cyan>calculated steerValue :: {steeringRotation}</color></size>");
        //    OnSteer?.Invoke(steeringRotation);
        //}

        public void RotateSteeringWithHands(float signedAngle)
        {
            Debug.Log("New Signed Angle " + signedAngle);
            float newRawRotation = 0;
            //float rotationChange = 0;            
           
            newRawRotation = overallRotation + signedAngle;

            newRawRotation = Mathf.Clamp(newRawRotation, -30, 30);
            //rotationChange = newRawRotation - overallRotation;

            currentRotation = transform.localRotation.eulerAngles;
            currentRotation.y = newRawRotation;
            Debug.Log("current Rotation:" + currentRotation);
            Debug.Log("Current NewRAwRotation:" + newRawRotation);
            transform.localRotation = Quaternion.Euler(currentRotation);
            overallRotation = newRawRotation;
            float steeringRotation = Map(overallRotation, -30, 30, -1, 1);
            Debug.Log($"OverallRotation - {overallRotation}");
            Debug.Log($"SteeringRotation - {steeringRotation}");
            Debug.Log($"YRotation - {currentRotation.y}");
            //OnSteer?.Invoke(steeringRotation);
            carController.SteeringStarted(steeringRotation);
        }

        public void RotateSteeringBikeWithHands(float newAngle)
        {
            Debug.Log("New Angle Bike" + newAngle);
            newAngle = Mathf.Clamp(newAngle, -15,15);
            Vector3 currentRotation = transform.localRotation.eulerAngles;
            currentRotation.y = newAngle;
            transform.localRotation = Quaternion.Euler(currentRotation);
            float steeringRotation = Map(newAngle, -15, 15, -1, 1);
            carController.SteeringStarted(steeringRotation);
        }


        //public void ResetSteeringWheel()
        //{
        //    StopAllCoroutines();
        //    StartCoroutine(ResetSteeringWheelCoroutine());
        //}
        //public void ResetSteeringWheel()

        //    float currentRotation = transform.rotation.eulerAngles.z;
        //    float rotationToReset = Mathf.LerpAngle(currentRotation, 0f, Time.deltaTime);
        //    transform.rotation = Quaternion.Euler(0f, 0f, rotationToReset);
        //    float steeringRotation = Map(rotationToReset, minRotation, maxRotation, -1, 1);
        //    OnSteer?.Invoke(steeringRotation);

        //    Debug.Log("Resetting Steering Wheel Complete");
        //}

        #endregion

        #region PRIVATE_FUNCTIONS

        private float Map2(float NewValue, float OldValue, float NewMax, float NewMin, float OldMin, float OldMax)
        {
            return (((OldValue - OldMin) * (NewMax - NewMin)) / (OldMax - OldMin)) + NewMin;
        }
        private void ClampSteeringWheelRotation()
        {
            Vector3 currentRotation = transform.rotation.eulerAngles;
            currentRotation.z = Mathf.Clamp(currentRotation.z, minRotation, maxRotation);
        }

        //private void RotateWheel()
        //{
        //    StartCoroutine(RotateCoroutine());
        //    //if (Mathf.Abs(rotationValue) < 0.01f) 
        //    //{
        //    //    currentRotation = 0.0f; 
        //    //}            
        //}

        //private void RotatingSteering()
        //{
        //    Vector3 currentRotation = transform.localRotation.eulerAngles;
        //    float rotation = currentRotation.z;
        //    float steeringRotation = Map(rotation, minRotation, maxRotation, -1, 1);
        //    OnSteer?.Invoke(steeringRotation);
        //}
        #endregion

        #region CO-ROUTINES

        //IEnumerator CheckForRotationChange(float signedAngle)
        //{
        //    while (true)
        //    {
        //        yield return null;
        //        float newRawRotation;
        //        //float rotationChange = 0;


        //        newRawRotation = overallRotation + signedAngle;

        //        newRawRotation = Mathf.Clamp(newRawRotation, -180, 180);
        //        //rotationChange = newRawRotation - overallRotation;

        //        currentRotation = transform.rotation.eulerAngles;
        //        currentRotation.z += newRawRotation;
        //        transform.rotation = Quaternion.Euler(currentRotation);
        //        overallRotation = newRawRotation;
        //        float steeringRotation = Map(currentRotation.z, -180, 180, -1, 1);
        //        OnSteer?.Invoke(steeringRotation);
        //    }
        //}

        IEnumerator ResetSteeringWheelCoroutine()
        {
            float timeElapsed = 0;
            float lerpDuration = 0.3f;
            currentRotation = transform.rotation.eulerAngles;
            while (timeElapsed < lerpDuration)
            {
                //Debug.Log("Resetting Steering Wheel");
                rotationValue = Mathf.Lerp(transform.rotation.eulerAngles.z, 0, timeElapsed / lerpDuration);
                currentRotation.z = currentRotation.z > 0 ? rotationValue : -rotationValue;
                transform.rotation = Quaternion.Euler(currentRotation);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
            currentRotation.z = 0;
            transform.rotation = Quaternion.Euler(currentRotation);

            Debug.Log("Resetting Steering Wheel Complete");
        }

        //private void ResetSteeringWheelTest()
        //{
        //    if (!wheelBeingHeld && wheelAngle != 0f)
        //    {
        //        float deltaAngle = wheelReleasedSpeed * Time.deltaTime;
        //        if (Mathf.Abs(deltaAngle) > Mathf.Abs(wheelAngle))
        //            wheelAngle = 0f;
        //        else if (wheelAngle > 0f)
        //            wheelAngle -= deltaAngle;
        //        else
        //            wheelAngle += deltaAngle;
        //    }

        //    Vector3 currentRotation = transform.localRotation.eulerAngles;
        //    //float rotation = currentRotation.z;
        //    currentRotation.z = wheelAngle * valueMultiplier / maximumSteeringAngle;
        //    transform.rotation = Quaternion.Euler(currentRotation);
        //    // Rotate the wheel image
        //    //wheelTR.localEulerAngles = new Vector3(0f, 0f, -wheelAngle);

        //    //m_value = wheelAngle * valueMultiplier / maximumSteeringAngle;
        //    //axis.value = m_value;
        //}

        //IEnumerator RotateCoroutine()
        //{
        //    while(true)
        //    {
        //        rotationValue = Map(carController.SteerValue, -1, 1, minRotation, maxRotation);
        //        Debug.LogWarning($"RotationValue:: {rotationValue}");
        //        Vector3 currentRotation = transform.localRotation.eulerAngles;
        //        currentRotation.z = rotationValue;
        //        transform.rotation = Quaternion.Euler(currentRotation);
        //        //transform.Rotate(Vector3.forward * rotationValue);
        //        yield return null;
        //    }
        //}
        #endregion

        #region EVENT_HANDLERS
        #endregion

        #region UI_CALLBACKS
        #endregion
    }
}