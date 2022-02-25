using System.Collections;
using System.Collections.Generic;
using Leap;
using Leap.Unity;
using UnityEngine;
using UnityEngine;
using Leap.Unity.Attributes;
using UnityEngine.Serialization;

public class LeapGestureControl : MonoBehaviour
{
    [SerializeField]
    public HandModel handModel;
    [SerializeField]
    public HandModelBase leftHandBase;

    //[SerializeField]
    //public Hand leftHand;

    public void SayHello()
    {
        Debug.Log("A pinch is done!");
    }



    ////hand move four direction
    //public bool isMoveRight(HandModelBase handbase)
    //{

    //    return leftHand.PalmVelocity > deltaVelocity && !isStationary(hand);
    //}

    //// 手划向右边
    //public bool isMoveLeft(Hand hand)
    //{

    //    //print (hand.PalmVelocity.x );
    //    return hand.PalmVelocity.x < -deltaVelocity && !isStationary(hand);
    //}

    ////手向上 
    //public bool isMoveUp(Hand hand)
    //{
    //    //print ("hand.PalmVelocity.y" + hand.PalmVelocity.y);

    //    return hand.PalmVelocity.y > deltaVelocity && !isStationary(hand);
    //}

    ////手向下  
    //public bool isMoveDown(Hand hand)
    //{
    //    return hand.PalmVelocity.y < -deltaVelocity && !isStationary(hand);
    //}


    ////手向前
    //public bool isMoveForward(Hand hand)
    //{
    //    //print (hand.PalmVelocity.z);
    //    return hand.PalmVelocity.z > deltaVelocity && !isStationary(hand);
    //}

    ////手向后 
    //public bool isMoveBack(Hand hand)
    //{
    //    return hand.PalmVelocity.z < -deltaVelocity && !isStationary(hand);
    //}







    void Update()
    {
        if (leftHandBase.IsTracked)
        {
            Debug.Log("I see my left hand");
        }

    }

}
