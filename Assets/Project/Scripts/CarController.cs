using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using SimpleInputNamespace;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System.Threading.Tasks;
using System;
using Yudiz.DirtBikeVR.Managers;
//using Photon.Pun;
//using Master.UIKit;
//using Yudiz.CarVR.CoreGamePlay;
//using Photon.Rea;
namespace Yudiz.DirtBikeVR.CoreGamePlay
{
    public class CarController : MonoBehaviour
    {
        private float horizontalInput;
        private float steerValue;

        public float SteerValue { get { return steerValue; } }

        [SerializeField] private float verticalInput;
        [SerializeField] private float currentSteeringAngle;
        [SerializeField] private float currentBrakingForce;

        public float maxSpeed;
        [SerializeField] float currentSpeed;
        [SerializeField] float currentMagnitude;

        [SerializeField] float maxRpmForrealPlayer;
        [SerializeField] float currentRpmForRealPlayer;
        //public float turnSensitivity;

        //public TMP_InputField turnSensitivityInputField;
        //public TMP_InputField angularDragInputField;

        [SerializeField] private float motorForce, brakeForce, maxSteeringAngle;

        [SerializeField] private WheelCollider frontLeftWC, frontRightWC, backLeftWC, backRightWC;

        [SerializeField] private Transform frontLeftWheelTranform, frontRightWheelTranform, backLeftWheelTranform, backRightWheelTranform;

        [SerializeField] Rigidbody carRb;

        //[SerializeField] SteeringWheel steeringWheel;

        //[SerializeField] AnimationCurve steeringCurve;

        //[SerializeField] AxisInputUIArrows forwardBackwardArrows;
        [SerializeField] GameObject forBackArrowsGameObject;

        //getter setter for motorForce
        public float MotorForce { get { return motorForce; } set { motorForce = value; } }
        public float CurrentSpeed { get { return currentSpeed; } }


        [SerializeField] bool autoAccelerate = false;

        [SerializeField] float autoAccelerateState;
        bool steerWithGyro = false;

        //[Range(0,1)]
        //[SerializeField] float touchSteeringWheelSensitivity;

        //[SerializeField] CarPositionRecorder ourRecorder;

        float steeringSensitivity;
        //float stayTimer;

        bool takeInputs;


        //Animaiton curve: exponential type: based on absolute value of horizontal value: it will return the amount of drag needed to apply on car
        [SerializeField] AnimationCurve steerDragCurve;


        //Animation curve: exponential type: based on absolute value of horizontal value: it will return the STIFFNESS needed to apply on backWheels.
        [SerializeField] AnimationCurve steerStiffnessCurve; //it will be inversely proportional


        //experimental: Animation curve: time: horizontal input: value: steer sensitivity.
        //for lesser hori inputs: low sensitivity, for more hori values: more sensitivity, but not such like which makes bad user experience
        [SerializeField] AnimationCurve horiSteerSensitivityCurve;




        [SerializeField] bool amIBot;

        //getter for bool amIbot
        public bool AmIBot { get { return amIBot; } }

        //[SerializeField] ParentCollectionRaySensor allRaysParent;

        //[SerializeField] RaycastInputsForBot raycastInputsForBot;

        bool inputsSet = false;


        //[SerializeField] WayPointsCalculator wayPointCalculatorOnCar;

        private void OnEnable()
        {

            //InputController.OnSteer += SteeringStarted;
            SteeringWheel.OnSteer += SteeringStarted;      
            //BreakingStick.OnBraking += Braking;
            InputController.OnAccelarate += AccelarateOrBrake;
            InputController.OnBreaking += Braking;


            startTakingInputs();

        }



        public void AssignPositiontoCar(Vector3 pos)
        {
            transform.position = pos;
        }




        private void OnDisable()
        {           
            SteeringWheel.OnSteer -= SteeringStarted;
            //BreakingStick.OnBraking -= Braking;
            InputController.OnAccelarate -= AccelarateOrBrake;
            InputController.OnBreaking -= Braking;

        }


        public void SetInputForLocalPlay()
        {

        }

        //when braking manually: stiffness will be set to 1 for back tyre to make drift: and when this is false, stiffness will be taken from animation curve: hori input vs stiffness
        bool isBrakingManually = false;


        int currentTargetIndex = 0;


        void startTakingInputs()
        {
            // Debug.Log("key pre");
            takeInputs = true;
            // Debug.Log("key post");
            //Debug.Log($"takeInput Value: {takeInputs} from {gameObject.name}");
        }

        void StopTakingInputs()
        {
            takeInputs = false;
        }



        private void Start()
        {
#if UNITY_EDITOR == false
            InputSystem.EnableDevice(Accelerometer.current);
#endif


            carRb.centerOfMass = new Vector3(0, -0.5f, 0);
            steeringSensitivity = 0;//0.8f;

        }

        private void FixedUpdate()
        {
            //ClampCarSpeed();
            //Debug.LogWarning($"SteeringValue:: {steerValue}");
            //        Debug.Log("takeINput bool state: "+takeInputs);           
            if (takeInputs == false) return;



            GetInput();


            ReverseInAutoSteer();

            if (isBrakingManually)
            {
                carRb.drag = 1f;
            }
            else
            {
                RunTimeDragBasedOnHoriInput();
            }
            // UpdateSpeedToSpeedometer();


            //AccelarateOrBrake(InputController.instance.InputValue);
            HandleMotor();

            //if(raycastInputsForBot.updateAngulareVelocity==false)
            HandleSteering();

            UpdateWheels();

            UpdateBackTyreStiffnessBasedOnHoriInput();
        }



        public void AICycleFixedUpdate()
        {


        }




        public void RunTimeDragBasedOnHoriInput()
        {



            //if no vertical input is given then drag should be 0.5 and at that time dont get drag based on horizontal input

            if (verticalInput == 0)
            {
                carRb.drag = 0.5f;
            }
            else
            {
                carRb.drag = steerDragCurve.Evaluate(Mathf.Abs(horizontalInput));

            }



        }


        void UpdateBackTyreStiffnessBasedOnHoriInput()
        {
            if (isBrakingManually) return;

            WheelFrictionCurve myWfc;
            myWfc = backRightWC.sidewaysFriction;


            myWfc.stiffness = steerStiffnessCurve.Evaluate(Math.Abs(horizontalInput));

            //Debug.Log("steer: "+ Math.Abs(horizontalInput)+" stiffness: "+myWfc.stiffness);

            backRightWC.sidewaysFriction = myWfc;
            backLeftWC.sidewaysFriction = myWfc;
        }




        public void ResetCarMotors()
        {
            frontLeftWC.motorTorque = 0;
            frontRightWC.motorTorque = 0;
            carRb.velocity = Vector3.zero;
            carRb.drag = 0;
            currentBrakingForce = 0;
            //Brake(true);
        }

        public void ReverseInAutoSteer()
        {
            if (autoAccelerate)
            {
                //autoAccelerateState = (forwardBackwardArrows.Value.y == -1) ? -1 : 1;
                autoAccelerateState = (verticalTouchForButtonsWithoutUIArrowScript == -1) ? -1 : 1;
            }
        }


        [SerializeField] float steerInput;

        float verticalTouchForButtonsWithoutUIArrowScript;

        public void AccelarateOrBrake(float a)
        {
            Debug.LogWarning($"Value of a : {a}");
            if (a == -1)
            {
                {
                    Debug.Log("Reversing");
                    //here call reverse method
                    Brake(false);
                    verticalTouchForButtonsWithoutUIArrowScript = a;

                }
            }

            if (a == 1)
            {
                Debug.Log("Forward");
                Brake(false);
                Debug.Log("From AB else case : " + a);
                verticalTouchForButtonsWithoutUIArrowScript = a;
            }

            if (a == 0)
            {
                Debug.Log("Zero");
                verticalTouchForButtonsWithoutUIArrowScript = a;
            }

        }

        private void Braking()
        {
            Brake(true);
        }

        private void SteeringStarted(float value)
        {
            StartCoroutine(LerpSteerValue(value));
        }

        IEnumerator LerpSteerValue(float value)
        {
            float timeElapsed = 0;
            float lerpDuration = 0.3f;
            //float startValue = 0;
            //float endValue = 10;
            Debug.Log("started");
            while (timeElapsed < lerpDuration)
            {
                //if ()
                //{
                Debug.Log("OnGoing");
                steerValue = Mathf.Lerp(steerValue, value, timeElapsed / lerpDuration);
                timeElapsed += Time.deltaTime;
                yield return null;

            }
            //steerValue = value;
            Debug.Log("Ended");
        }

        //IEnumerator SteeringCoroutine(float value)
        //{
        //    while (true)
        //    {
        //        //Debug.LogError($"{value}");
        //        if (value == 1)
        //        {
        //            steerValue = Mathf.Lerp(steerValue, value, Time.deltaTime);
        //            //  Debug.LogWarning($"1 :: {steerValue}");
        //        }
        //        else if (value == -1)
        //        {
        //            steerValue = Mathf.Lerp(steerValue, value, Time.deltaTime);
        //            // Debug.LogWarning($"-1 :: {steerValue}");
        //        }
        //        else
        //        {
        //            steerValue = Mathf.Lerp(steerValue, value, Time.deltaTime);
        //            // Debug.LogWarning($"0 :: {steerValue}");
        //        }
        //        yield return null;
        //    }
        //}

        private void GetInput()
        {
            horizontalInput = steerValue;
            verticalInput = autoAccelerate ? autoAccelerateState : verticalTouchForButtonsWithoutUIArrowScript;
        }

        public void Brake(bool brake)
        {
            Debug.Log("BREAKING");
            autoAccelerateState = brake ? 0 : 1;

            //isBrakingManually = brake;

            if (brake)
            {
                currentBrakingForce = brakeForce;
                autoAccelerateState = 0;


            }
            else
            {
                currentBrakingForce = 0;
                autoAccelerateState = 1;
            }

            MakeCarDrift(brake);

            ApplyBraking();
        }

        public void ApplyDrag()
        {
            if (verticalInput == 0)
            {
                carRb.drag = 1f;
            }

            else
            {
                carRb.drag = 0f;
                carRb.angularDrag = 0f;
            }
            //carRb.drag = enable ? 0.3f : 0;        
        }


        void HandleMotor()
        {

            currentSpeed = 2 * Mathf.PI * frontLeftWC.radius * 60 * frontLeftWC.rpm / 1000;
            currentMagnitude = carRb.velocity.magnitude;

            if (carRb.velocity.magnitude < maxSpeed)
            {
                frontLeftWC.motorTorque = verticalInput * motorForce;
                frontRightWC.motorTorque = verticalInput * motorForce;

                backLeftWC.motorTorque = verticalInput * motorForce;
                backRightWC.motorTorque = verticalInput * motorForce;
            }
            else
            {
                frontLeftWC.motorTorque = 0;
                frontRightWC.motorTorque = 0;

                backLeftWC.motorTorque = 0;
                backRightWC.motorTorque = 0;
            }


        }

        void ApplyBraking()
        {
            frontLeftWC.brakeTorque = currentBrakingForce;
            frontRightWC.brakeTorque = currentBrakingForce;
        }



       public void HandleSteering()
        {
            currentSteeringAngle = maxSteeringAngle * horizontalInput; // * steeringSensitivity;//0.85f; //steeringSensitivity;
            frontLeftWC.steerAngle = currentSteeringAngle;
            frontRightWC.steerAngle = currentSteeringAngle;
        }


        void MakeCarDrift(bool doDrift)
        {
            //this method will be called only while break button is pressed, here we will also apply turn at backwheels which will make all the difference
            //between car just stopping and car drifting...
            if (doDrift)
            {
                //for making drift with very less slipperiness
                //backRightWC.steerAngle = -currentSteeringAngle/4;
                //backLeftWC.steerAngle = -currentSteeringAngle/4;

                //for making drift with more slipperiness:
                backRightWC.steerAngle = -currentSteeringAngle;
                backLeftWC.steerAngle = -currentSteeringAngle;


                carRb.centerOfMass = new Vector3(0, 0.5f, 0);


                WheelFrictionCurve myWfc;
                myWfc = backRightWC.sidewaysFriction;
                // myWfc.stiffness = 1f;
                //myWfc.stiffness = 1.5f;
                myWfc.stiffness = 1.3f;

                backRightWC.sidewaysFriction = myWfc;
                backLeftWC.sidewaysFriction = myWfc;


            }
            else
            {
                //reset the angles
                backRightWC.steerAngle = 0;
                backLeftWC.steerAngle = 0;
                carRb.centerOfMass = new Vector3(0, 0.5f, 0.1f);

                WheelFrictionCurve myWfc;
                myWfc = backRightWC.sidewaysFriction;
                myWfc.stiffness = 2f;

                backRightWC.sidewaysFriction = myWfc;
                backLeftWC.sidewaysFriction = myWfc;
            }

            //.Debug.Log("driftdone " + doDrift);
        }



        float dragAmountToHave = 0;





        #region WheelCollider and transform parts
        void UpdateWheels()
        {
            UpdateSingleWheel(frontLeftWC, frontLeftWheelTranform);
            UpdateSingleWheel(frontRightWC, frontRightWheelTranform);
            UpdateSingleWheel(backLeftWC, backLeftWheelTranform);
            UpdateSingleWheel(backRightWC, backRightWheelTranform);
        }

        void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
        {
            Vector3 pos;
            Quaternion rot;

            wheelCollider.GetWorldPose(out pos, out rot);
            wheelTransform.rotation = rot;
            wheelTransform.position = pos;
        }
        #endregion



        public float CarSpeedRigidBody()
        {
            return carRb.velocity.magnitude;
        }

        #region code related to change in InputScheme/controls

    }
    #endregion
}

public enum BrakeType
{
    HardBrake = 1,
    DriftBrakeLow = 2,
    DriftBrakeHigh = 3,


    DefaultState = 4
}



