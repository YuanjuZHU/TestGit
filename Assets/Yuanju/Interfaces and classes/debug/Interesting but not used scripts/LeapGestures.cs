using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Leap;
using Leap.Unity;

public class LeapGestures : MonoBehaviour
{

    public static bool Gesture_left = false;
    public static bool Gesture_right = false;
    public static bool Gesture_up = false;
    public static bool Gesture_down = false;
    public static bool Gesture_zoom = false;
    public static float movePOs = 0.0f;

    private LeapProvider mProvider;
    private Frame mFrame;
    private Hand mHand;



    private Vector leftPosition;
    private Vector rightPosition;
    public static float zoom = 1.0f;
    [Tooltip("Velocity (m/s) of Palm ")]

    public float smallestVelocity = 1.45f;//手掌移动的最小速度

    [Tooltip("Velocity (m/s) of Single Direction ")]
    [Range(0, 1)]
    public float deltaVelocity = 1.0f;//单方向上手掌移动的速度

    // Use this for initialization
    void Start()
    {
        mProvider = FindObjectOfType<LeapProvider>() as LeapProvider;
    }

    // Update is called once per frame
    void Update()
    {

        mFrame = mProvider.CurrentFrame;//获取当前帧
                                        //获得手的个数
                                        //print ("hand num are " + mFrame.Hands.Count);

        if (mFrame.Hands.Count > 0)
        {
            if (mFrame.Hands.Count == 2)
                zoom = CalcuateDistance(mFrame);

            if (mFrame.Hands.Count == 1)
                LRUDGestures(mFrame, ref movePOs);
        }
    }


    float CalcuateDistance(Frame mFrame)
    {
        Gesture_zoom = true;
        Gesture_left = false;
        Gesture_right = false;

        float distance = 0f;
        //print ("Two hands");
        foreach (var itemHands in mFrame.Hands)
        {
            if (itemHands.IsLeft)
            {
                leftPosition = itemHands.PalmPosition;
                //print ("leftPosition" + leftPosition);
            }
            if (itemHands.IsRight)
            {
                rightPosition = itemHands.PalmPosition;
                //print ("rightPosition" + rightPosition);
            }
        }

        if (leftPosition != Vector.Zero && rightPosition != Vector.Zero)
        {

            Vector3 leftPos = new Vector3(leftPosition.x, leftPosition.y, leftPosition.z);
            Vector3 rightPos = new Vector3(rightPosition.x, rightPosition.y, rightPosition.z);

            distance = 10 * Vector3.Distance(leftPos, rightPos);
            print("distance" + distance);
        }

        if (distance != 0)
            return distance;
        else
            return distance = 1;
    }




    void LRUDGestures(Frame mFrame, ref float movePOs)
    {
        Gesture_zoom = false;
        foreach (var item in mFrame.Hands)
        {
            int numFinger = item.Fingers.Count;
            //print ("item is  " + numFinger);

            //print("hand are " + isOpenFullHand (item));
            // print ("isOpenFullHands is  " + isOpenFullHands(item));


            if (item.GrabStrength == 1)
            {
                //print ("num is 0, gestures is woquan");

            }
            else if (item.GrabStrength == 0)
            {
                //print ("num is 5, open your hand");
                //print("PalmVelocity" + item.PalmVelocity);
                //print("PalmPosition" + item.PalmPosition);
                movePOs = item.PalmPosition.x;
                if (isMoveLeft(item))
                {
                    Gesture_left = true;
                    Gesture_right = false;
                    print("move left");

                }
                else if (isMoveRight(item))
                {
                    Gesture_left = false;
                    Gesture_right = true;
                    print("move Right");

                }
                else if (isMoveUp(item))
                {
                    Gesture_left = false;
                    Gesture_right = false;
                    print("move Up");

                }
                else if (isMoveDown(item))
                {
                    Gesture_left = false;
                    Gesture_right = false;
                    print("move Down");

                }
                else if (isMoveForward(item))
                {
                    Gesture_left = false;
                    Gesture_right = false;
                    print("move Forward");

                }
                else if (isMoveBack(item))
                {
                    Gesture_left = false;
                    Gesture_right = false;
                    print("move back");

                }
            }
        }
    }



    private bool isStone(Hand hand)
    {
        //print ("hand.GrabAngle" + hand.GrabAngle);
        return hand.GrabAngle > 2.0f;
    }
    //是否抓取
    public bool isGrabHand(Hand hand)
    {
        return hand.GrabStrength > 0.8f;        //抓取力 
    }


    //hand move four direction
    public bool isMoveRight(Hand hand)
    {

        return hand.PalmVelocity.x > deltaVelocity && !isStationary(hand);
    }

    // 手划向右边
    public bool isMoveLeft(Hand hand)
    {

        //print (hand.PalmVelocity.x );
        return hand.PalmVelocity.x < -deltaVelocity && !isStationary(hand);
    }

    //手向上 
    public bool isMoveUp(Hand hand)
    {
        //print ("hand.PalmVelocity.y" + hand.PalmVelocity.y);

        return hand.PalmVelocity.y > deltaVelocity && !isStationary(hand);
    }

    //手向下  
    public bool isMoveDown(Hand hand)
    {
        return hand.PalmVelocity.y < -deltaVelocity && !isStationary(hand);
    }


    //手向前
    public bool isMoveForward(Hand hand)
    {
        //print (hand.PalmVelocity.z);
        return hand.PalmVelocity.z > deltaVelocity && !isStationary(hand);
    }

    //手向后 
    public bool isMoveBack(Hand hand)
    {
        return hand.PalmVelocity.z < -deltaVelocity && !isStationary(hand);
    }

    //固定不动的
    public bool isStationary(Hand hand)
    {
        return hand.PalmVelocity.Magnitude < smallestVelocity;      //Vector3.Magnitude返回向量的长度
    }


}

