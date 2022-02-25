using Leap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity;
using System;
using NPOI.SS.Formula.Functions;

public class LeapControl : MonoBehaviour
{

    LeapProvider provider;
    private Camera cam;
    private Frame frame;
    private const float rotate_sensitive = 1500f;  //rotate sensibility
    [SerializeField]
    private float displacement_amplifier = 0.015f; //translate sensibility
    private const float rotate_initial_value = 0f;  //the initial value of rotation
    private Vector3 leftHandMoveVector;//the translation distances for the camera
    private Vector3 rightHandMoveVector;
    public GameObject AttachmentHands;
    public GameObject AttachmentHandSlides;
    public GameObject AttachmentHandMenu;
    public GameObject Presentation;
    public static bool AttachmentHasBeenSelected;

    private bool leftHandSwipedForward;
    private bool rightHandSwipedForward;
    private bool leftHandSwipedBackward;
    private bool rightHandSwipedBackward;

    private bool leftHandSwipedLeft;
    private bool rightHandSwipedLeft;
    private bool leftHandSwipedRight;
    private bool rightHandSwipedRight;

    private bool leftHandSwipedUpward;
    private bool rightHandSwipedUpward;
    private bool leftHandSwipedDownward;
    private bool rightHandSwipedDownward;


    private bool leftHandBackToCam;
    private bool rightHandBackToCam;

    /// <summary>
    /// judgment condition
    /// </summary>
    [SerializeField]
    public float smallestVelocity = 0.1f;
    const float deltaVelocity = 0.000001f;
    const float deltaCloseFinger = 0.06f;

    void Start()
    {
        provider = FindObjectOfType<LeapProvider>() as LeapProvider;
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        AttachmentHasBeenSelected = false;
    }


    void Update()
    {
        SwipeTwoHandsMoveCamera();
        //SwipeHandPlaySlides();
        DisplayHideSubHandAttachments();
        HideInvisibleAttachment();
    }

    /// <summary>
    /// Swipe hands to move the camera
    /// </summary>
    public void SwipeTwoHandsMoveCamera()
    {
        leftHandMoveVector = Vector3.zero;
        rightHandMoveVector = Vector3.zero;


        leftHandSwipedForward = false;
        rightHandSwipedForward = false;
        leftHandSwipedBackward = false;
        rightHandSwipedBackward = false;

        leftHandSwipedLeft = false;
        rightHandSwipedLeft = false;
        leftHandSwipedRight = false;
        rightHandSwipedRight = false;

        leftHandSwipedUpward = false;
        rightHandSwipedUpward = false;
        leftHandSwipedDownward = false;
        rightHandSwipedDownward = false;

        leftHandBackToCam = false;
        rightHandBackToCam = false;

        frame = provider.CurrentFrame;
        if (frame.Hands.Count == 2)
        {
            foreach (var hand in frame.Hands)
            {
                //judge the movement of the hands
                //move forward
                if (hand.IsLeft && isOpenFullHand(hand))
                {
                    //Debug.Log("left hand palm velocity: " + hand.PalmVelocity);
                    if (isMoveForward(hand))
                    {
                        leftHandSwipedForward = true;
                    }
                    if (isMoveBackward(hand))
                    {
                        leftHandSwipedBackward = true;
                    }
                    if (isMoveLeft(hand))
                    {
                        leftHandSwipedLeft = true;
                    }
                    if (isMoveRight(hand))
                    {
                        leftHandSwipedRight = true;
                    }
                    if (isMoveUpward(hand))
                    {
                        leftHandSwipedUpward = true;
                    }
                    if (isMoveDownward(hand))
                    {
                        leftHandSwipedDownward = true;
                    }
                    var palmVel = hand.PalmVelocity;
                    rightHandMoveVector = new Vector3(palmVel.x, palmVel.y, palmVel.z) * displacement_amplifier;
                }

                if (hand.IsRight && isOpenFullHand(hand))
                {
                    if (isMoveForward(hand))
                    {
                        rightHandSwipedForward = true;
                    }
                    if (isMoveBackward(hand))
                    {
                        rightHandSwipedBackward = true;
                    }
                    if (isMoveLeft(hand))
                    {
                        rightHandSwipedLeft = true;
                    }
                    if (isMoveRight(hand))
                    {
                        rightHandSwipedRight = true;
                    }
                    if (isMoveUpward(hand))
                    {
                        rightHandSwipedUpward = true;
                    }
                    if (isMoveDownward(hand))
                    {
                        rightHandSwipedDownward = true;
                    }

                    var palmVel = hand.PalmVelocity;
                    rightHandMoveVector = new Vector3(palmVel.x, palmVel.y, palmVel.z) * displacement_amplifier;
                }

                if (hand.IsLeft && Vector3.Dot(hand.PalmNormal.ToVector3(), cam.transform.forward) > 0)
                {
                    leftHandBackToCam=true;
                }
                if (hand.IsRight && Vector3.Dot(hand.PalmNormal.ToVector3(), cam.transform.forward) > 0)
                {
                    rightHandBackToCam = true;
                }
            }

            if (leftHandBackToCam && rightHandBackToCam)
            {
                if ((leftHandSwipedLeft || leftHandSwipedRight) && (rightHandSwipedLeft || rightHandSwipedRight))
                {
                    //Debug.Log("both hands swiped laterally");
                    transform.localPosition = (leftHandMoveVector + rightHandMoveVector) / 2 + transform.localPosition;
                }
                if ((leftHandSwipedUpward || leftHandSwipedDownward) && (rightHandSwipedUpward || rightHandSwipedDownward))
                {
                    //Debug.Log("both hands swiped in height");
                    transform.localPosition = (leftHandMoveVector + rightHandMoveVector) / 2 + transform.localPosition;
                }
                if ((leftHandSwipedForward || leftHandSwipedBackward) && (rightHandSwipedForward || rightHandSwipedBackward))
                {
                    //Debug.Log("both hands swiped frontal");
                    //TODO add a if here to limit the height of the camera.
                    transform.localPosition = (leftHandMoveVector + rightHandMoveVector) / 2 + transform.localPosition;
                }


            }
            //Debug.Log("(leftHandSwipedLeft || leftHandSwipedRight) && (rightHandSwipedLeft || rightHandSwipedRight),"+ ((leftHandSwipedLeft || leftHandSwipedRight) && (rightHandSwipedLeft || rightHandSwipedRight)));
            //Debug.Log("(leftHandSwipedUpward || leftHandSwipedDownward) && (rightHandSwipedUpward || rightHandSwipedDownward)," + ((leftHandSwipedUpward || leftHandSwipedDownward) && (rightHandSwipedUpward || rightHandSwipedDownward)));
            //Debug.Log("((leftHandSwipedForward || leftHandSwipedBackward) && (rightHandSwipedForward || rightHandSwipedBackward))," + ((leftHandSwipedForward || leftHandSwipedBackward) && (rightHandSwipedForward || rightHandSwipedBackward)));


        }
    }

    //hand.velocity X denotes frontal, Z denotes lateral, Y denotes up and down
    protected bool isMoveLeft(Hand hand)// hand swipe to left
    {
        return hand.PalmVelocity.z > deltaVelocity && Mathf.Abs(hand.PalmVelocity.z) > Mathf.Abs(hand.PalmVelocity.x) && Mathf.Abs(hand.PalmVelocity.z)> Mathf.Abs(hand.PalmVelocity.y) && !isStationary(hand);
    }


    protected bool isMoveRight(Hand hand)   // hand swipe to right
    {
        return hand.PalmVelocity.z < -deltaVelocity && Mathf.Abs(hand.PalmVelocity.z) > Mathf.Abs(hand.PalmVelocity.x) && Mathf.Abs(hand.PalmVelocity.z) > Mathf.Abs(hand.PalmVelocity.y) && !isStationary(hand);
    }

    protected bool isMoveForward(Hand hand)// hand swipe forward
    {
        return hand.PalmVelocity.x > deltaVelocity && Mathf.Abs(hand.PalmVelocity.x) > Mathf.Abs(hand.PalmVelocity.z) && Mathf.Abs(hand.PalmVelocity.x) > Mathf.Abs(hand.PalmVelocity.y) && !isStationary(hand);
    }

    protected bool isMoveBackward(Hand hand)// hand swipe backward
    {
        return hand.PalmVelocity.x <- deltaVelocity && Mathf.Abs(hand.PalmVelocity.x) > Mathf.Abs(hand.PalmVelocity.z) && Mathf.Abs(hand.PalmVelocity.x) > Mathf.Abs(hand.PalmVelocity.y) && !isStationary(hand);
    }

    protected bool isMoveUpward(Hand hand)// hand swipe Upward
    {
        return hand.PalmVelocity.y > deltaVelocity && Mathf.Abs(hand.PalmVelocity.y) > Mathf.Abs(hand.PalmVelocity.x) && Mathf.Abs(hand.PalmVelocity.y) > Mathf.Abs(hand.PalmVelocity.z) && !isStationary(hand);
    }

    protected bool isMoveDownward(Hand hand)// hand swipe Downward
    {
        return hand.PalmVelocity.y < -deltaVelocity && Mathf.Abs(hand.PalmVelocity.y) > Mathf.Abs(hand.PalmVelocity.x) && Mathf.Abs(hand.PalmVelocity.y) > Mathf.Abs(hand.PalmVelocity.z) && !isStationary(hand);
    }



    protected bool isStationary(Hand hand)// hand is still 
    {
        return hand.PalmVelocity.Magnitude < smallestVelocity;
    }

    protected bool isCloseHand(Hand hand)     //hand is a  fist (closed)
    {
        List<Finger> listOfFingers = hand.Fingers;
        int count = 0;
        for (int f = 0; f < listOfFingers.Count; f++)
        { //check all the fingers
            Finger finger = listOfFingers[f];
            if ((finger.TipPosition - hand.PalmPosition).Magnitude < deltaCloseFinger)
            {
                count++;
                //  if (finger.Type == Finger.FingerType.TYPE_THUMB)
                //  Debug.Log ((finger.TipPosition - hand.PalmPosition).Magnitude);
            }
        }
        return (count == 5);
    }

    protected bool isOpenFullHand(Hand hand)         //hand is fully open
    {
        //Debug.Log (hand.GrabStrength + " " + hand.PalmVelocity + " " + hand.PalmVelocity.Magnitude);
        return hand.GrabStrength == 0;
    }

    private void HideInvisibleAttachment()
    {
        frame = provider.CurrentFrame;
        if (frame.Hands.Count == 0)
        {
            AttachmentHands.SetActive(false);
        }
        else if (frame.Hands.Count == 1)
        {
            if (frame.Hands[0].IsLeft)
            {
                AttachmentHands.SetActive(true);
            }
        }
        else
        {
            AttachmentHands.SetActive(true);
        }
    }

    private void DisplayHideSubHandAttachments()
    {
        if (!AttachmentHasBeenSelected)
        {
            if (Presentation.activeSelf)
            {
                AttachmentHandSlides.SetActive(true);
                AttachmentHandMenu.SetActive(false);
            }

            else
            {
                AttachmentHandSlides.SetActive(false);
                AttachmentHandMenu.SetActive(true);
            }

        }
        AttachmentHasBeenSelected = true;
    }

}

