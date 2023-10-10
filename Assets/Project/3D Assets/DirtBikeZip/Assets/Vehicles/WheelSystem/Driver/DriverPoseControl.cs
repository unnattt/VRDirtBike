using UnityEngine;
using System;
using System.Collections;
using WheelsSystem;

public enum PoseState
{
    FrontBack = 0,
    LeftTurn,
    RightTurn,
    LeftRight
}

//[RequireComponent(typeof(Animator))] 
public class DriverPoseControl : MonoBehaviour
{

    protected Animator animator;

    [System.NonSerialized]
    public Vehicle vehicle;
    private Transform driverIK;
    private Transform rightHand = null;
    private Transform leftHand = null;

    private Transform rightElbow = null;
    private Transform leftElbow = null;

    private Transform rightFoot = null;
    private Transform leftFoot = null;

    private Transform leftKnee = null;
    private Transform rightKnee = null;

    private PoseState oldState;
    private PoseState state;
    [SerializeField] private float min01 = 0.0f;
    [SerializeField] private float max01 = 1.0f;

    [System.NonSerialized] public float animTime;

    void Awake()
    {
        vehicle = GetComponentInParent<Vehicle>();
        if (!vehicle)
        {
            enabled = false;
            return;
        }

        driverIK = vehicle.transform.Find("DriverIK");
        rightHand = driverIK.Find("RightHandPivot").GetChild(0);
        leftHand = driverIK.Find("LeftHandPivot").GetChild(0);
        rightFoot = driverIK.Find("RightFootPivot").GetChild(0);
        leftFoot = driverIK.Find("LeftFootPivot").GetChild(0);

        rightElbow = driverIK.Find("RightElbow");
        leftElbow = driverIK.Find("LeftElbow");

        leftKnee = driverIK.Find("LeftKnee");
        rightKnee = driverIK.Find("RightKnee");

        animator = GetComponent<Animator>();
        if (!animator)
        {
            enabled = false;
        }
    }

    void Start ()
    {
        rightHand.parent.parent = vehicle.steering.transform;
        leftHand.parent.parent = vehicle.steering.transform;

        rightFoot.parent.parent = vehicle.transform;
        leftFoot.parent.parent = vehicle.transform;
    }

    void OnAnimatorIK()
    {
        animator.SetLookAtWeight(0.5f);
        animator.SetLookAtPosition(vehicle.transform.position + Vector3.ClampMagnitude( vehicle.body.velocity, 20.0f));

        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);  
        animator.SetIKPosition(AvatarIKGoal.RightHand, rightHand.position);
        animator.SetIKRotation(AvatarIKGoal.RightHand, rightHand.rotation);

        SetIK(rightHand, AvatarIKGoal.RightHand);
        SetIK(leftHand, AvatarIKGoal.LeftHand);
        SetIK(rightFoot, AvatarIKGoal.RightFoot);
        SetIK(leftFoot, AvatarIKGoal.LeftFoot);

        if (leftElbow)
        {
            animator.SetIKHintPosition(AvatarIKHint.LeftElbow, leftElbow.position);
            animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, 1.0f);
        }
        if (rightElbow)
        {
            animator.SetIKHintPosition(AvatarIKHint.RightElbow, rightElbow.position);
            animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, 1.0f);
        }

        if (leftKnee)
        {
            animator.SetIKHintPosition(AvatarIKHint.LeftKnee, leftKnee.position);
            animator.SetIKHintPositionWeight(AvatarIKHint.LeftKnee, 1.0f);
        }
        if (rightKnee)
        {
            animator.SetIKHintPosition(AvatarIKHint.RightKnee, rightKnee.position);
            animator.SetIKHintPositionWeight(AvatarIKHint.RightKnee, 1.0f);
        }     
    }

    private void SetIK(Transform target, AvatarIKGoal ikGoal)
    {
        animator.SetIKPositionWeight(ikGoal, 1);
        animator.SetIKRotationWeight(ikGoal, 1);  
        animator.SetIKPosition(ikGoal, target.position);
        animator.SetIKRotation(ikGoal, target.rotation);
    }

	void FixedUpdate()
	{
		animTime = Mathf.Lerp(animTime, Mathf.Clamp((0.5f - Vector3.Dot(vehicle.transform.forward, Vector3.up)) / (max01 - min01), min01, max01), 15.0f * Time.fixedDeltaTime);
		animator.Play(state.ToString(),-1, animTime);
	}
}
