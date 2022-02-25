using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGeneratorComponent
{

    // The component is enabled to control?
    bool Enabled { get; set; }
    int Status { get; set; }
    bool IsNeedCheck { get; set; }
    void Awake();

    // Initialize component.
    void Initialize();

    void GetOperatedComponent();
    void UpdateMaterials();
}
