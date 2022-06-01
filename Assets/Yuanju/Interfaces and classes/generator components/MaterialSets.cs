using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSets : MonoBehaviour
{
    /// <summary>
    /// a switch have several status, for each status, there should be a set of materials 
    /// </summary>
    [System.Serializable]
    public class StatusMaterialSets
    {
        //[SerializeField]
        public List<PartsMaterialSets> MaterialSet = new List<PartsMaterialSets>(); /*{ get; set; }*/

    }

    /// <summary>
    /// a switch may have several parts
    /// </summary>
    [System.Serializable]
    public class PartsMaterialSets
    {
        //[SerializeField]
        [HideInInspector]
        public string FontName;

        public List<Material> MaterialsOfDifferentParts = new List<Material>(); 
    }
}
