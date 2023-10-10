using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using WheelsSystem;

public delegate void VehicleInputHandler (VehiclesInputType inputType);
public class VehicleUserController : MonoBehaviour
{
    public Vehicle vehicle;
    public bool isMain = true;
    public event VehicleInputHandler OnInput;
    [SerializeField]
    private float turnK = 0.3f;

    public float horizontal
    {
        get;
        set;
    }

    public float vertical
    {
        get;
        set;
    }

    public float turn
    {
        get;
        private set;
    }

    private float speed;

    public float frontBrake
    {
        get;
        set;
    }
    public float backBrake
    {
        get;
        set;
    }

    public float forceAccelerate
    {
        get;
        set;
    }

    public void SetVehicle(Vehicle vehicle)
    {
        this.vehicle = vehicle;
    }
    void Awake ()
    {
        VehicleInput.StartBreak += VehicleInput_StartBreak;
        VehicleInput.EndBreak += VehicleInput_EndBreak;

        if (!vehicle)
        {
            enabled = false;
            return;
        }
    }

    void VehicleInput_EndBreak (bool isFront)
    {
        vehicle.EndBrake(isFront);
    }

    void VehicleInput_StartBreak (bool isFront)
    {
        vehicle.StartBrake(isFront);
    }
 

    public void StopControl()
    {
        vehicle.ForceStopControl();
    }
    void FixedUpdate()
    {
        if (isMain)
        {
            horizontal = -VehicleInput.steering;
            vertical = VehicleInput.gas;
            frontBrake = VehicleInput.frontBrake;
            backBrake = VehicleInput.backBrake;
            forceAccelerate = VehicleInput.forceAccelerate;
        }

        float hh = Mathf.Clamp(horizontal * Mathf.Lerp(1.0f, turnK, vehicle.body.velocity.magnitude / vehicle.parameters.maxVelocityMPS), -1.0f, 1.0f);

        turn = Mathf.Lerp(turn, hh, 10.0f * Time.fixedDeltaTime);
        speed = Mathf.Lerp(speed, vertical, 10.0f * Time.fixedDeltaTime);


        if (Mathf.Abs(turn) > 0.1)
        {
            vehicle.RotateSteering(turn);
            if (OnInput != null)
            {
                OnInput(turn < 0.0f? VehiclesInputType.LeftSteering : VehiclesInputType.RightSteering);
            }
        }
        else
        {
            vehicle.NormalizeSteering();
        }

        if (frontBrake > 0.1f)
        {
            vehicle.FrontBrake(frontBrake);
            if (OnInput != null)
            {
                OnInput(VehiclesInputType.FrontBrake);
            }
        }
        else if (backBrake > 0.1f)
        {
            vehicle.BackBrake(backBrake);
            if (OnInput != null)
            {
                OnInput(VehiclesInputType.BackBrake);
            }
        }
        else
        {
            if (forceAccelerate > 0.1f)
            {
                vehicle.ForceAccelerate(100);
                if (OnInput != null)
                {
                    OnInput(VehiclesInputType.ForceAccelerate);
                }
            }
            else if (Mathf.Abs(speed) > 0.1)
            {
                vehicle.Accelerate(speed);
                if (OnInput != null)
                {
                    OnInput(speed > 0.0f? VehiclesInputType.Gas: VehiclesInputType.BackGas);
                }
            }
            else
            {
                vehicle.SlowDown();
            }
        }
    }
}
