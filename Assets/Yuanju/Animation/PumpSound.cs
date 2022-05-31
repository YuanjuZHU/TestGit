using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PumpSound : MonoBehaviour
{
    /// <summary>
    /// play water pump engine noise
    /// </summary>
    public void PlayPumpSound()
    {
        GetComponent<AudioSource>().Play();
    }

    /// <summary>
    /// stop water pump engine noise
    /// </summary>
    public void StopPumpSound()
    {
        GetComponent<AudioSource>().Stop();
    }
}
