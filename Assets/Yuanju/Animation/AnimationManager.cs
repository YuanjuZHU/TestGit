using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// this class is dedicated to aniamtion control
/// </summary>
public class AnimationManager : MonoBehaviour
{
    //public GameObject WaterInBoiler;
    //public GameObject WaterInIndicator;
    //public GameObject WaterAlarmLED;




    public Animator WaterInBoilerAnimator;
    public Animator WaterInIndicatorAnimator;
    public Animator WaterAlarmLEDAnimator;

    public static bool IsStartAnimation;
    public static bool isAnimationStateChanged = false;


    void Update()
    {
        //if (isAnimationStateChanged)
        //{

        //    if (IsStartAnimation)
        //    {
        //        PlayAnimations();
        //    }
        //    else
        //    {
        //        StopAnimations();
        //    }
        //    isAnimationStateChanged = false;
        //}
    }


    /// <summary>
    /// play all the animations
    /// </summary>
    public void PlayAnimations()
    {
        PlayAnAnimations(WaterInBoilerAnimator);
        PlayAnAnimations(WaterInIndicatorAnimator);
        PlayAnAnimations(WaterAlarmLEDAnimator);
        HelpMenuButtons.IsAnimationButtonClicked = true;
    }

    /// <summary>
    /// stop all the animations
    /// </summary>
    public void StopAnimations()
    {
        StopAnAnimations(WaterInBoilerAnimator);
        StopAnAnimations(WaterInIndicatorAnimator);
        StopAnAnimations(WaterAlarmLEDAnimator);
        GameObject.Find("water 1").GetComponent<AudioSource>().Stop();
        HelpMenuButtons.IsAnimationButtonClicked = false;
    }


    /// <summary>
    /// play an animation by taking the animator
    /// </summary>
    /// <param name="animator"></param>
    public void PlayAnAnimations(Animator animator)
    {
        animator.SetBool("IsStartAnimation", true);
    }

    /// <summary>
    /// stop an animation by taking the animator
    /// </summary>
    /// <param name="animator"></param>
    public void StopAnAnimations(Animator animator)
    {
        animator.SetBool("IsStartAnimation", false);
    }


}
