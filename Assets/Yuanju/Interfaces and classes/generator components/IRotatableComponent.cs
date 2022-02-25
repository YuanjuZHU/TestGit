using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRotatableComponent:IGeneratorComponent
{
    /// <summary>
    /// The component is adsorbent or not
    /// </summary>
    bool IsAdsorbent { get; set; }

    /// <summary>
    /// The position of the mobile part of a component
    /// </summary>
    float Angle { get; set; } 
}
