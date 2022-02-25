using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPressableComponent : IGeneratorComponent
{
    /// <summary>
    /// this can be used for the max and min height: downOffset=Max-Min
    /// </summary>
    float DownOffset { set; get; }

    /// <summary>
    /// Self lock offset percent.
    /// </summary>
    float LockPercent { set; get; }

    /// <summary>
    /// Button is down already?
    /// </summary>
    bool IsPressedDown { get; }

}
