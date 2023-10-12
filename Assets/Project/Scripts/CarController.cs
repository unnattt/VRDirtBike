using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using SimpleInputNamespace;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System.Threading.Tasks;
using System;
//using Photon.Pun;
//using Master.UIKit;
using Yudiz.DirtBikeVR.Managers;
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
        [SerializeField] float currentmagnitude;

        //[SerializeField] float maxRpmForrealPlayer;
        //[SerializeField] float currentRpmForRealPlayer;


        //public float turnSensitivity;

        //public TMP_InputField turnSensitivityInputField;
        //public TMP_InputField angularDragInputField;

        [SerializeField] private float motorForce, brakeForce, maxSteeringAngle;

        [SerializeField] private WheelCollider frontLeftWC, frontRightWC, backLeftWC, backRightWC;

        [SerializeField] private Transform frontLeftWheelTranform, frontRightWheelTranform, backLeftWheelTranform, backRightWheelTranform;

        [SerializeField] Rigidbody carRb;
        //[SerializeField] private SteeringWheel steeringWheel;

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



        //[SerializeField] TextMeshProUGUI currentSteerValFromAngleToNormalisedTxt;

        //[SerializeField] TextMeshProUGUI currentStiffnessTxt;
        //[SerializeField] TextMeshProUGUI currentDragTxt;

        //[SerializeField] TextMeshProUGUI currentBrakeForceTxt;


        //PowerUpSpeedBoost ourPowerUp;

        //[SerializeField] ParticlesManagerOnCar ourParticles;

        [SerializeField] bool amIBot;

        //getter for bool amIbot
        public bool AmIBot { get { return amIBot; } }

        //[SerializeField] ParentCollectionRaySensor allRaysParent;

        //[SerializeField] RaycastInputsForBot raycastInputsForBot;

        bool inputsSet = false;

        [Header("Storing CarsPosition and Rotation")]
        private Vector3 carsInitialPosition;
        private Quaternion carsInitialRotation;



        //[Header("Speedo Meter")]
        //[SerializeField] private GameObject needle;
        //[SerializeField] private float needleMinRotation = -20f;
        //[SerializeField] private float needleMaxRotation = 200f;




        //[SerializeField] WayPointsCalculator wayPointCalculatorOnCar;

        private void OnEnable()
        {

            //InputController.OnSteer += SteeringStarted;
            SteeringWheel.OnSteer += SteeringStarted;
            //BreakingStick.OnBraking += Braking;
            InputController.OnAccelarate += AccelarateOrBrake;
            //InputController.OnBreaking += Braking;
            InputController.OnResetCarPos += ResetCarPosition;

            //if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
            //{
            //    if (myCarPhotonView.IsMine == false)
            //    {
            //        //turning off this script from running, as there is no need to run from multiple machines
            //        enabled = false;
            //    }
            //}

            //() => await EnableInputsAfterDelay();  //() => takeInputs = true;
            //if (AmIBot == false)
            //{
            //    RacingRampageEvents.OnClickInvitePlay += startTakingInputs;
            //    RacingRampageEvents.OnGameOver += StopTakingInputs;
            //}


            //if(amIBot==false)
            //{
            //    AssignSteering();
            //}

            //if (PhotonNetwork.IsConnected == false)
            //{
            //    //if(amIBot)
            startTakingInputs();

            //if (inputsSet == false && amIBot == false)
            //{
            //    //Debug.Log("here diconnected");
            //    RacingRampageEvents.UseSpeedUpBooster += SpeedUp;
            //    UIController.instance.getScreen(ScreenType.Gameplay).GetComponent<ScreenGamePlayUI>().BrakeUpDown += Brake;//(a) => Brake(a);
            //    UIController.instance.getScreen(ScreenType.Gameplay).GetComponent<ScreenGamePlayUI>().ForwardReverse += changeVerticalWithButton;
            //    inputsSet = true;

            //    KeyThingsHolder.instance.GetSpeedo.AssignMaxRPM = maxRpmForrealPlayer;
            //}

            //}
            //else
            //{




            //    //Debug.Log("connect from");
            //    if (amIBot == false)
            //    {
            //        //startTakingInputs();
            //        RacingRampageEvents.UseSpeedUpBooster += SpeedUp;
            //        gameObject.name = $"car{UnityEngine.Random.Range(1, 9)}";

            //        if (CheckIfThisIsMinePhotonView())
            //        {
            //            if (inputsSet == false)
            //            {
            //                UIController.instance.getScreen(ScreenType.Gameplay).GetComponent<ScreenGamePlayUI>().BrakeUpDown += Brake;//(a) => Brake(a);
            //                UIController.instance.getScreen(ScreenType.Gameplay).GetComponent<ScreenGamePlayUI>().ForwardReverse += changeVerticalWithButton;
            //                inputsSet = true;
            //                KeyThingsHolder.instance.GetSpeedo.AssignMaxRPM = maxRpmForrealPlayer;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        //this is bot
            //        //connect and if iam master client then my bots should take inputs
            //        if (PhotonNetwork.IsMasterClient)
            //        {
            //            RacingRampageEvents.OnClickInvitePlay += startTakingInputs;
            //        }
            //    }
            //}
        }



        public void AssignPositiontoCar(Vector3 pos)
        {
            transform.position = pos;
        }


        //public bool CheckIfIamRealMe()
        //{
        //    if (amIBot)
        //    {
        //        return false;
        //    }

        //    if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        //    {
        //        return myCarPhotonView.IsMine;
        //    }

        //    return true;
        //}


        private void OnDisable()
        {
            //RacingRampageEvents.OnClickInvitePlay -= startTakingInputs;

            //RacingRampageEvents.OnGameOver -= StopTakingInputs;

            //InputController.OnSteer -= SteeringStarted;
            SteeringWheel.OnSteer -= SteeringStarted;
            //BreakingStick.OnBraking -= Braking;
            InputController.OnAccelarate -= AccelarateOrBrake;
            //InputController.OnBreaking -= Braking;
            InputController.OnResetCarPos -= ResetCarPosition;


            // RacingRampageEvents.OnClickPlay -= () => takeInputs = true;

            //if (PhotonNetwork.IsConnected == false)
            //{
            //    if (inputsSet == false && amIBot == false)
            //    {
            //        RacingRampageEvents.UseSpeedUpBooster -= SpeedUp;
            //        ScreenGamePlayUI s = UIController.instance.getScreen(ScreenType.Gameplay).GetComponent<ScreenGamePlayUI>();

            //        if (s != null)
            //        {
            //            s.BrakeUpDown -= Brake;//(a) => Brake(a);
            //            s.ForwardReverse -= changeVerticalWithButton;
            //            inputsSet = true;
            //        }
            //    }
            //}
            //else
            //{
            //    if (amIBot == false)
            //    {

            //        RacingRampageEvents.UseSpeedUpBooster -= SpeedUp;
            //        if (CheckIfThisIsMinePhotonView())
            //        {
            //            if (UIController.instance.getScreen(ScreenType.Gameplay) != null)
            //            {
            //                ScreenGamePlayUI s = UIController.instance.getScreen(ScreenType.Gameplay).GetComponent<ScreenGamePlayUI>();
            //                if (s != null)
            //                {
            //                    if (inputsSet == false)
            //                    {
            //                        s.BrakeUpDown -= Brake;//(a) => Brake(a);
            //                        s.ForwardReverse -= changeVerticalWithButton;
            //                        inputsSet = true;
            //                    }
            //                }
            //            }

            //        }
            //    }
            //}
        }




        public void SetInputForLocalPlay()
        {

        }

        //when braking manually: stiffness will be set to 1 for back tyre to make drift: and when this is false, stiffness will be taken from animation curve: hori input vs stiffness
        bool isBrakingManually = false;

        //async Task EnableInputsAfterDelay()
        //{
        //    await Task.Delay(3000);
        //   takeInputs = true;
        //}

        //public void UpdateCarStatsToDisplay()
        //{
        //    currentSteerValFromAngleToNormalisedTxt.text = Mathf.Clamp((currentSteeringAngle / maxSteeringAngle), -1, 1).ToString("n2");
        //    currentDragTxt.text = carRb.drag.ToString("n2");
        //    currentStiffnessTxt.text = backLeftWC.sidewaysFriction.stiffness.ToString("n2");
        //    currentBrakeForceTxt.text = currentBrakingForce.ToString("n2");
        //}
        //PhotonView myCarPhotonView;
        int currentTargetIndex = 0;

        //public bool CheckIfThisIsMinePhotonView()
        //{
        //    if (amIBot)
        //    {
        //        return false;
        //    }
        //    // Debug.Log($"returning for photonView: on object: {gameObject.name}: {myCarPhotonView.IsMine}");
        //    return myCarPhotonView.IsMine;
        //}

        //public override void OnPhotonInstantiate(PhotonMessageInfo info)
        //{
        //    //Assign this gameObject to player called instantiate the prefab
        //    info.sender.TagObject = this.gameObject;
        //}

        //private void Awake()
        //{
        //    myCarPhotonView = PhotonView.Get(this);

        //    if (amIBot == false)
        //    {
        //        if (PhotonNetwork.IsConnected && myCarPhotonView.IsMine == false)
        //        {
        //            takeInputs = false;
        //        }
        //        else
        //        {
        //            steeringWheel = FindObjectOfType<SteeringWheel>().GetComponent<SteeringWheel>();
        //            if (steeringWheel == null)
        //            {
        //                Debug.Log("steerWheel null");
        //            }
        //            forBackArrowsGameObject = FindObjectOfType<AxisInputUIArrows>().gameObject;
        //            forwardBackwardArrows = forBackArrowsGameObject.GetComponent<AxisInputUIArrows>();

        //            //if (amIBot == false)
        //            //CameraFollow c = FindObjectOfType<CameraFollow>();//.GetComponent<CameraFollow>().target = transform;
        //            //.GetComponent<CameraFollow>().target = transform;

        //            KeyThingsHolder.instance.PlayerCarCameraFollow.AssignCarAsTargetToThisCamera(this);

        //        }
        //    }


        //}

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

        //public void AssignSteering()
        //{
        //    steeringWheel = FindObjectOfType<SteeringWheel>();
        //}

        private void Start()
        {
            //Storing Cars Position and Rotation
            carsInitialPosition = transform.position;
            carsInitialRotation = transform.rotation;


#if UNITY_EDITOR == false
            InputSystem.EnableDevice(Accelerometer.current);
#endif

            //AudioManager.instance.PlaySound(AudioTrack.CarEngine, true);

            //forwardBackwardArrows = FindObjectOfType<AxisInputUIArrows>();



            // takeInputs = false;
            //takeInputs = false;
            //ourRecorder = GetComponent<CarPositionRecorder>();

            //steeringSensitivity = 0;//0.8f;

            //ourPowerUp = GetComponent<PowerUpSpeedBoost>();

            //ourParticles = GetComponent<ParticlesManagerOnCar>();

            //if (amIBot)
            //{
            //    raycastInputsForBot = GetComponent<RaycastInputsForBot>();
            //}

            //WheelFrictionCurve myWfc;
            //myWfc = backRightWC.sidewaysFriction;
            //myWfc.stiffness = 1.2f;

            //backRightWC.sidewaysFriction = myWfc;
            //backLeftWC.sidewaysFriction = myWfc;

            //stayTimer = 0.3f;
            //stayTimerCounter = stayTimer;

            //autoAccelerateState = 1;
            //carRb = GetComponent<Rigidbody>();
            //wheelColliders = GetComponentsInChildren<WheelCollider>();

            //steeringCurve = new AnimationCurve(new Keyframe(0, 1f), new Keyframe(10, 0.5f), new Keyframe(20f, 0.2f));
            //Time.timeScale = 1f;
        }

        private void FixedUpdate()
        {
            //ControlSpeedoMeter();

            //ClampCarSpeed();
            //Debug.LogWarning($"SteeringValue:: {steerValue}");
            //        Debug.Log("takeINput bool state: "+takeInputs);            
            //if (amIBot)
            //{
            //    if (carRb.velocity.sqrMagnitude > 300)
            //    {
            //        carRb.drag = raycastInputsForBot.GetDistanceBasedDrag(transform.position);
            //    }
            //    else
            //    {
            //        carRb.drag = 0;
            //    }

            //    //Debug.Log($"<size=15>{gameObject.name}my Squared Velo<color=cyan>{carRb.velocity.sqrMagnitude}</color></size>");

            //    if (raycastInputsForBot.updateAngulareVelocity == false)
            //    {
            //        steerInput = allRaysParent.GetSteerValue();
            //    }
            //    else
            //    {
            //        steerInput = 0;
            //    }




            //    // Debug.Log("botSteerValue:  "+ steerInput);

            //    //verticalInput =  Mathf.Clamp01( raycastInputsForBot.GetPercentOfDistancecoveredFromLastPosToNextPos(transform.position) * 5); //Vector3.Distance(transform.position, raycastInputsForBot.GetCurrentTargetPosition());
            //    verticalInput = raycastInputsForBot.IdealVertical;
            //    //Debug.Log("I got ver input: "+verticalInput);
            //    if (verticalInput > 0.4 && carRb.velocity.magnitude < 0.2f)
            //    {
            //        verticalInput = 10;
            //    }

            //    if (Math.Abs(steerInput) == 1f)
            //    {

            //        //carRb.drag =   //1f;
            //        //carRb.drag = currentSpeed/maxSpeed * 100;
            //        // carRb.drag = currentSpeed / maxSpeed * 10;
            //        // currentBrakingForce = brakeForce;
            //        //verticalInput = 0.2f;
            //        horizontalInput = steerInput;
            //        RunTimeDragBasedOnHoriInput();

            //    }
            //    else
            //    {
            //        //carRb.drag = 0f;
            //        //verticalInput = 1;
            //        // currentBrakingForce = 0f;
            //    }
            //    horizontalInput = Mathf.Clamp(steerInput, -1, 1);
            //    //ApplyBraking();
            //}
            //else


            //if (PhotonNetwork.IsConnected)
            //{

            //    if (CheckIfThisIsMinePhotonView() == false) return;
            //}



            if (takeInputs == false) return;



            GetInput();


            //UpdateSteerSensitivityBasedOnCarSpeed();

            //ApplyDrag();
            //ReverseInAutoSteer();
            //FlippedCarCheck();


            //UpdateCarStatsToDisplay();

            //    if (isBrakingManually)
            //    {
            //        carRb.drag = 1f;
            //    }
            //    else
            //    {
            //        RunTimeDragBasedOnHoriInput();
            //    }
            //    // UpdateSpeedToSpeedometer();
            //}

            //AccelarateOrBrake(InputController.instance.InputValue);
            HandleMotor();

            //if(raycastInputsForBot.updateAngulareVelocity==false)
            HandleSteering();

            UpdateWheels();

            UpdateBackTyreStiffnessBasedOnHoriInput();
        }


        //private void Update()
        //{
        //    if (amIBot == false)
        //    {
        //        if (PhotonNetwork.IsConnected)
        //        {

        //            if (CheckIfThisIsMinePhotonView() == false) return;


        //        }

        //        UpdateSpeedToSpeedometer();

        //    }

        //}

        public void AICycleFixedUpdate()
        {
            //all methods of AI bot will be called here, the methods which should be in FixedUpdate...
            //this will be called if: amIBot is true for this car...


        }




        //public async void SpeedUp()
        //{
        //    //Debug.Log( string.Format( "From {0}: used speedUp",gameObject.name.ToString()));
        //    if (ourParticles != null)
        //        ourParticles.PlayNitroFx();
        //    if (ourPowerUp != null)
        //    {
        //        ourPowerUp.BoostSpeed(5);
        //        await Task.Delay(5000);
        //    }
        //    ourParticles.PauseNitroFx();

        //}
        //public void SpeedDown()
        //{
        //    if (ourPowerUp != null)
        //    {
        //        ourPowerUp.DropSpeed(5);
        //    }
        //}
        //Vector3 positionAtLastCollision;
        //private void OnCollisionEnter(Collision collision)
        //{
        //    positionAtLastCollision = transform.position;

        //    //if(transform.rotation.)
        //    //ReFlipCar();
        //}



        //float stayTimerCounter=0f;
        //bool shouldCheckInTriggerStay = false;


        //private void OnTriggerEnter(Collider other)
        //{

        //    if (other.CompareTag("checkpoint"))
        //    {
        //        wayPointCalculatorOnCar.RegisterCrossedWaypoint(other.transform);
        //    }




        //    if (wayPointCalculatorOnCar.AllVisited())
        //    {

        //        //here call that rpc to increment rank for all
        //        GetComponent<RankManagerOnCar>().FireRPCToIncrementRankForAll();



        //        if (amIBot)
        //        {
        //            return;
        //        }

        //        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        //        {
        //            if (myCarPhotonView.IsMine == false)
        //            {
        //                Debug.Log("this is was not mine view, so returning from onTrigger from other car ");
        //                return;
        //            }
        //        }


        //        Debug.Log($"my rank: {GetComponent<RankManagerOnCar>().GetMyRank}");

        //        Debug.Log($"myname is: {RacingRampagePhotonGameplay.instance.MyCarCached.name} and I have Completed my lap. So iam invoking game over now, and amIMasterClient={PhotonNetwork.IsMasterClient}");

        //        RacingRampageEvents.OnGameOver();
        //    }



        //}

        //private void OnTriggerEnter(Collider other)
        //{

        //    SpeedBreakerBehaviour s = other.GetComponent<SpeedBreakerBehaviour>();

        //    if(s!=null)
        //    {
        //        //stayTimer = s.GetTime();
        //        stayTimerCounter = s.GetTime(); //stayTimer;
        //        dragAmountToHave = s.GetDragAmount();
        //        shouldCheckInTriggerStay = true;
        //        //Debug.Log("time I got: "+stayTimer);
        //    }

        //    //if (other.tag == "SpeedBreaker")
        //    //{
        //    //    stayTimerCounter = stayTimer;
        //    //    shouldCheckInTriggerStay = true;
        //    //}
        //}



        public void RunTimeDragBasedOnHoriInput()
        {

            //if (isBrakingManually) return;

            //-1 to 1 hori input
            //carRb.drag = Mathf.Abs(horizontalInput);


            //if no vertical input is given then drag should be 0.5 and at that time dont get drag based on horizontal input

            if (verticalInput == 0)
            {
                carRb.drag = 0.5f;
            }
            else
            {
                carRb.drag = steerDragCurve.Evaluate(Mathf.Abs(horizontalInput));

            }

            //Debug.Log("hori: "+ Math.Abs( horizontalInput)+"drag: "+carRb.drag);
            //0 to 1.5 or 2

        }


        void UpdateBackTyreStiffnessBasedOnHoriInput()
        {
            if (isBrakingManually) return;

            WheelFrictionCurve myWfc;
            myWfc = backRightWC.sidewaysFriction;
            // myWfc.stiffness = 1f;
            //myWfc.stiffness = 1.5f;
            //myWfc.stiffness = 2f;
            myWfc.stiffness = steerStiffnessCurve.Evaluate(Math.Abs(horizontalInput));

            //Debug.Log("steer: "+ Math.Abs(horizontalInput)+" stiffness: "+myWfc.stiffness);

            backRightWC.sidewaysFriction = myWfc;
            backLeftWC.sidewaysFriction = myWfc;
        }

        //public void TriggerEffect(bool slowSpeed)
        //{
        //    //if slowsSpeed true: then more drag
        //    Brake(slowSpeed);
        //    //if(slowSpeed)
        //    //{
        //    //    carRb.drag = 5f;
        //    //    Brake(true);
        //    //}
        //    //else
        //    //{
        //    //    carRb.drag = 0f;
        //    //    Brake(false);
        //    //}


        //    //if slow speed false: then less drag
        //}

        //void CheckTriggersInUpdate()
        //{
        //   // Debug.Log("stayTimerCounter: "+stayTimerCounter);
        //    if(shouldCheckInTriggerStay == false)
        //    {
        //        return;
        //    }

        //    if(stayTimerCounter<0)
        //    {      
        //        shouldCheckInTriggerStay = false;
        //        //stayTimerCounter = stayTimer;
        //        Brake(BrakeType.DefaultState);
        //    }
        //    else
        //    {
        //        stayTimerCounter -= Time.deltaTime;
        //        Brake(BrakeType.DriftBrakeLow);
        //    }



        //}




        //void ResetCarPosRotFromRecorder()
        //{
        //    ourRecorder.SetCarPositionToSecondLastSavedTransform();
        //}


        //float timerGap=0;
        //void FlippedCarCheck()
        //{
        //    //Debug.Log("flip car check: ");
        //    //timerGap -= Time.deltaTime;
        //    //if (timerGap > 0) return;
        //    if (Vector3.Dot(transform.up, Vector3.down) > 0)
        //    {
        //        //positionAtLastCollision = transform.position;
        //        //means upside down, just reflip it to make default
        //        //ReFlipCar();
        //        ResetCarMotors();

        //        ourRecorder.ChangeBoolStateOfCarFlip(false);

        //        ResetCarPosRotFromRecorder();
        //    }
        //    else
        //    {
        //        if (Vector3.Dot(transform.up, Vector3.down) < 0)
        //            ourRecorder.ChangeBoolStateOfCarFlip(true);
        //        else
        //            ourRecorder.ChangeBoolStateOfCarFlip(false);
        //    }
        //}


        public void ResetCarMotors()
        {
            frontLeftWC.motorTorque = 0;
            frontRightWC.motorTorque = 0;
            carRb.velocity = Vector3.zero;
            carRb.drag = 0;
            currentBrakingForce = 0;
            //Brake(true);
        }

        //why check only in collision, just check in every fixed update, if the car is upside down or not
        //if yes then reflip it, o/w do nothing...




        //public void ReverseInAutoSteer()
        //{
        //    if (autoAccelerate)
        //    {
        //        //autoAccelerateState = (forwardBackwardArrows.Value.y == -1) ? -1 : 1;
        //        autoAccelerateState = (verticalTouchForButtonsWithoutUIArrowScript == -1) ? -1 : 1;
        //    }
        //}

        //public void updateAngularDrag(TMP_InputField input)
        //{
        //    carRb.angularDrag = int.Parse(input.text);
        //}

        //[SerializeField] float steerInput;

        public float verticalTouchForButtonsWithoutUIArrowScript;

        public void AccelarateOrBrake(float a)
        {
            //Debug.LogWarning($"Value of a : {a}");
            if (a == -1)
            {
                //if (carRb.velocity.magnitude > 3)
                //{
                //    Debug.Log("Stopping");
                //    //here drift, means call brake method
                //    verticalTouchForButtonsWithoutUIArrowScript = 0;
                //    Brake(true);
                //}
                //else
                {
                    //Debug.Log("Reversing");
                    //here call reverse method
                    //Brake(false);
                    verticalTouchForButtonsWithoutUIArrowScript = a;

                }
            }

            if (a == 1)
            {
                //Debug.Log("Forward");
                //Brake(false);
                //Debug.Log("From AB else case : " + a);
                verticalTouchForButtonsWithoutUIArrowScript = a;
            }

            if (a == 0)
            {
                //Debug.Log("Zero");
                verticalTouchForButtonsWithoutUIArrowScript = a;
            }

            //Debug.Log($"vert called from: {gameObject.name} value given: {a}");
            //verticalTouchForButtonsWithoutUIArrowScript = a;

            //if reverse button is tapped i.e a=-1

            //then if car's velocity is NOT zero then, make drift, if zero then reverse      

            //Debug.Log("direction passed: "+a);
        }

        private void Braking()
        {
            Brake(true);
        }

        public void SteeringStarted(float value)
        {
            //Debug.Log($"Steering Angle :: {value}");

            //StopAllCoroutines();
            //StartCoroutine(SteeringCoroutine(value));
            //Debug.Log($"<size=20><color=cyan>Value :: {value}</color></size>");

            //steerValue = Mathf.Lerp(steerValue, value, Time.deltaTime);
            //StopAllCoroutines();
            StartCoroutine(LerpSteerValue(value));

            //Debug.Log($"<size=20><color=cyan>calculated steerValue :: {steerValue}</color></size>");
        }

        IEnumerator LerpSteerValue(float value)
        {
            float timeElapsed = 0;
            float lerpDuration = 0.3f;
            //float startValue = 0;
            //float endValue = 10;
            while (timeElapsed < lerpDuration)
            {
                //if ()
                //{
                //steerValue = Mathf.Lerp(steerValue, value, timeElapsed / lerpDuration);
                steerValue = value;
                timeElapsed += Time.deltaTime;
                yield return null;
                //}
                //else
                //{

                //    break;
                //}
                //startValue = endValue;
            }
            steerValue = value;

        }

        IEnumerator SteeringCoroutine(float value)
        {
            while (true)
            {
                //Debug.LogError($"{value}");
                if (value == 1)
                {
                    steerValue = Mathf.Lerp(steerValue, value, Time.deltaTime);
                    //  Debug.LogWarning($"1 :: {steerValue}");
                }
                else if (value == -1)
                {
                    steerValue = Mathf.Lerp(steerValue, value, Time.deltaTime);
                    // Debug.LogWarning($"-1 :: {steerValue}");
                }
                else
                {
                    steerValue = Mathf.Lerp(steerValue, value, Time.deltaTime);
                    // Debug.LogWarning($"0 :: {steerValue}");
                }
                yield return null;
            }
        }

        private void GetInput()
        {
            //if (steerWithGyro)
            //    GetInputFromAccelerometerForSteering();
            //else
            {
                //horizontalInput = steeringWheel.Value;// * touchSteeringWheelSensitivity;
                horizontalInput = steerValue;
            }
            //Debug.Log("hor: " + horizontalInput + "     x: " + steerInput );

            //if (autoAccelerate == false)
            //verticalInput = (autoAccelerate) ? autoAccelerateState : forwardBackwardArrows.Value.y;
            verticalInput = (autoAccelerate) ? autoAccelerateState : verticalTouchForButtonsWithoutUIArrowScript;
            //verticalInput = Input.GetAxis("Vertical");
            //        Debug.Log($"now vertINput: {verticalInput} and passed value: {verticalTouchForButtonsWithoutUIArrowScript}");
            //        Debug.Log(verticalInput);
            //isBraking = Input.GetKey(KeyCode.Space);
        }


        //        private void GetInputFromAccelerometerForSteering()
        //        {
        //#if UNITY_EDITOR
        //        return;
        //#endif
        //            steerInput = Accelerometer.current.acceleration.ReadValue().x;
        //            //turnSensitivity = 1;//float.Parse(turnSensitivityInputField.text);
        //            // horizontalInput = Mathf.Clamp(steerInput * turnSensitivity, -1, 1);
        //            horizontalInput = Mathf.Clamp(steerInput, -1, 1);
        //            //steerInput = 
        //        }


        //this one is used by a brake button in gameplay screen
        public void Brake(bool brake)
        {
            Debug.Log("BREAKING");
            autoAccelerateState = brake ? 0 : 1;

            //isBrakingManually = brake;

            if (brake)
            {
                currentBrakingForce = brakeForce;
                autoAccelerateState = 0;
                // carRb.drag = 1;
                //ourPowerUp.DefaultState();

                //if (ourParticles != null)
                //    ourParticles.PauseNitroFx();

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
            //else if(verticalInput==0.5f)
            //{
            //    carRb.drag = 0.4f;
            //    carRb.angularDrag = 2f;
            //}
            else
            {
                carRb.drag = 0f;
                carRb.angularDrag = 0f;
            }
            //carRb.drag = enable ? 0.3f : 0;        
        }

        //void UpdateSpeedToSpeedometer()
        //{
        //    if (carRb.velocity.magnitude > 0.5f)
        //    {
        //        KeyThingsHolder.instance.GetSpeedo.UpdateSpeedometer(frontLeftWC.rpm / maxRpmForrealPlayer);
        //    }
        //    else
        //    {
        //        KeyThingsHolder.instance.GetSpeedo.UpdateSpeedometer(0);
        //    }
        //}

        void HandleMotor()
        {
            //currentSpeed = 2 * Mathf.PI * frontLeftWC.radius * 60 * frontLeftWC.rpm / 1000;
            //Debug.Log("currentSpeed: " + currentSpeed);
            //        Debug.Log($"From Handle Motor {gameObject.name}");
            currentSpeed = 2 * Mathf.PI * frontLeftWC.radius * 60 * frontLeftWC.rpm / 1000;
            //Debug.Log($"rpm: {frontLeftWC.rpm}");
            //if (amIBot)
            //{
            //    //currentSpeed = 2 * Mathf.PI * frontLeftWC.radius * 60 * frontLeftWC.rpm / 1000;
            //    //Debug.Log("currentSpeed: " + currentSpeed);
            //    if (currentSpeed < maxSpeed)
            //    {
            //        frontLeftWC.motorTorque = verticalInput * motorForce;
            //        frontRightWC.motorTorque = verticalInput * motorForce;

            //        backLeftWC.motorTorque = verticalInput * motorForce;
            //        backRightWC.motorTorque = verticalInput * motorForce;
            //    }
            //    else
            //    {
            //        frontLeftWC.motorTorque = 0;
            //        frontRightWC.motorTorque = 0;

            //        backLeftWC.motorTorque = 0;
            //        backRightWC.motorTorque = 0;
            //    }

            //    return;
            //}
            currentmagnitude = carRb.velocity.magnitude;

            //Debug.LogWarning("Magnitude: " + carRb.velocity.magnitude);
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

            //        Debug.Log("from handleMotor: "+ verticalInput * motorForce);

            //currentBrakingForce = isBraking ? brakeForce : 0f;

            //ApplyBraking();
        }

        void ApplyBraking()
        {
            //backLeftWC.brakeTorque = currentBrakingForce;
            //backRightWC.brakeTorque = currentBrakingForce;
            frontLeftWC.brakeTorque = currentBrakingForce;
            frontRightWC.brakeTorque = currentBrakingForce;
        }



        void HandleSteering()
        {
            //old implementation: good working, but 180 problem

            //steeringSensitivity = horiSteerSensitivityCurve.Evaluate(Math.Abs(horizontalInput));

            // * steeringSensitivity;//0.85f; //steeringSensitivity;
            currentSteeringAngle = maxSteeringAngle * horizontalInput;
            //currentSteeringAngle = horizontalInput;// * steeringSensitivity;//0.85f; //steeringSensitivity;
            frontLeftWC.steerAngle = currentSteeringAngle;
            frontRightWC.steerAngle = currentSteeringAngle;


            //UpdateDrag();

            //new implementation for speed based steer sensitivity.
            //float speed = carRb.velocity.magnitude;

            ////calculate the steering sensitivity based on speed using the animation curve
            //float steeringSensitivity = steeringCurve.Evaluate(speed);
            ////Debug.Log(steeringSensitivity);
            ////Get the steering input from the player
            ////float steeringInput = Input.GetAxis("Horizontal");

            ////apply the modified steering input based on the steering sensitivity.
            //float modifiedSteeringInput = horizontalInput * steeringSensitivity;

            ////apply the modified steering input to the wheel colliders.
            ////foreach (WheelCollider wheelCollider in wheelColliders)
            ////{
            ////    wheelCollider.steerAngle = modifiedSteeringInput * maxSteeringAngle;
            ////}
            //frontLeftWC.steerAngle = modifiedSteeringInput * maxSteeringAngle;
            //frontRightWC.steerAngle = modifiedSteeringInput * maxSteeringAngle;
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
                carRb.centerOfMass = new Vector3(0, 0.5f, -0.25f);

                WheelFrictionCurve myWfc;
                myWfc = backRightWC.sidewaysFriction;
                myWfc.stiffness = 2f;

                backRightWC.sidewaysFriction = myWfc;
                backLeftWC.sidewaysFriction = myWfc;
            }

            //.Debug.Log("driftdone " + doDrift);
        }


        //instead of making default state at exit of collider:
        //make a drift for 1-2 sec then return to normal state: no need to wait till exit:
        //otherwise what is happening is: when stayed in trigger now, no movement can be done with the inputs

        float dragAmountToHave = 0;

        //new brake method with enums
        //public void Brake(BrakeType type)
        //{
        //    bool driftBool = false;
        //    switch (type)
        //    {
        //        case BrakeType.DefaultState:
        //            //autoAccelerateState = 1;
        //            // currentBrakingForce = 0;
        //            carRb.drag = 0;
        //            carRb.angularDrag = 0;
        //            driftBool = false;
        //            break;
        //        case BrakeType.HardBrake:
        //            //autoAccelerateState = 0;
        //            //currentBrakingForce = brakeForce;
        //            driftBool = true;
        //            break;
        //        case BrakeType.DriftBrakeLow:
        //            //autoAccelerateState = 0.5f;//0.5f;
        //            //currentBrakingForce = brakeForce / 2;
        //            //carRb.drag = 0.5f;
        //            //carRb.drag = 1.5f;
        //            carRb.drag = dragAmountToHave;
        //            carRb.angularDrag = 15f;
        //            driftBool = true;
        //            break;
        //        default:
        //            break;
        //        //case BrakeType.DriftBrakeHigh:
        //        //    break;

        //    }

        //    MakeCarDrift(driftBool);
        //    //autoAccelerateState = brake ? 0 : 1;

        //    //if (brake)
        //    //{
        //    //    currentBrakingForce = brakeForce;
        //    //    autoAccelerateState = 0;
        //    //}
        //    //else
        //    //{
        //    //    currentBrakingForce = 0;
        //    //    autoAccelerateState = 1;
        //    //}

        //   // MakeCarDrift(type);


        //    //ApplyBraking();
        //}




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

        //private void ClampCarSpeed()
        //{
        //    Vector3 carSpeed = carRb.velocity;            
        //    carSpeed.z =  Mathf.Clamp(carRb.velocity.magnitude, 0, 5);
        //    carRb.velocity = carSpeed;
        //}

        public float CarSpeedRigidBody()
        {
            return carRb.velocity.magnitude;
        }



        #region SpeedoMeter

        //private void ControlSpeedoMeter()
        //{
        //    float newSpeed = maxSpeed * 4;
        //    float speedRatio = Mathf.Abs(currentSpeed / newSpeed);
        //    float targetAngle = Mathf.Lerp(needleMinRotation,needleMaxRotation, speedRatio);

        //    needle.transform.localRotation = Quaternion.Euler(-17, 180, targetAngle);

        //}
        #endregion

        public void ResetCarPosition()
        {
            transform.position = carsInitialPosition;
            transform.rotation = carsInitialRotation;
        }


        #region code related to change in InputScheme/controls
        //public void OnChangeInputType(InputType currInputType)
        //{
        //    if (amIBot)
        //        return;
        //    // Debug.Log("Onchange input type 1");
        //    switch (currInputType)
        //    {
        //        //brake is exception in all controls, it will be present in all cases.

        //        case InputType.AccelerateAutoSteerTilt:
        //            //here all onscreen controls will be turned off: accelerate auto, and steer will be from gyro, SO GYRO is ON
        //            //autoAccelerate = true;
        //            autoAccelerateState = 1;
        //            steerWithGyro = true;
        //            steeringWheel.gameObject.SetActive(false);
        //            //forwardBackwardArrows.transform.GetChild(0).gameObject.SetActive(false);
        //            forBackArrowsGameObject.transform.GetChild(0).gameObject.SetActive(false);
        //            //ApplyDrag(false);
        //            break;

        //        case InputType.AccelerateAutoSteerOnScreen:
        //            //here onscreen steering will be ON and up down controls will be OFF: accelerate auto, and steer with onscreen steering, GYRO OFF
        //            //autoAccelerate = true;
        //            autoAccelerateState = 1;
        //            steerWithGyro = false;
        //            //ApplyDrag(false);

        //            steeringWheel.gameObject.SetActive(true);
        //            //forwardBackwardArrows.transform.GetChild(0).gameObject.SetActive(false);
        //            forBackArrowsGameObject.transform.GetChild(0).gameObject.SetActive(false);
        //            break;

        //        case InputType.AccelerateOnScreenSteerTilt:
        //            //here onscreen up down ON, and onscreen steering will be OFF, and steering will be done with GYRO ON
        //            autoAccelerate = false;
        //            steerWithGyro = true;

        //            steeringWheel.gameObject.SetActive(false);
        //            //forwardBackwardArrows.transform.GetChild(0).gameObject.SetActive(true);
        //            forBackArrowsGameObject.transform.GetChild(0).gameObject.SetActive(true);
        //            //ApplyDrag(false);
        //            break;
        //    }
        //Debug.Log("Onchange input type 2");
    }
    #endregion
}

public enum BrakeType
{
    HardBrake = 1,
    DriftBrakeLow = 2,
    DriftBrakeHigh = 3,

    //DefaultState will reset car as it was at start: 0 drag, 0velocity, 0brake force etc...
    DefaultState = 4
}



