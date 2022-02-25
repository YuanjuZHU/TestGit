using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// interactions of opening and closing safety pressure box: cover is highlighted when grasped and close to its default position.  
/// </summary>
public class OpenCloseCover : MonoBehaviour
{
    public GameObject Cover;
    public float Angle;
    public float Distance;
    public Material HighlightedCoverMaterial;
    private Dictionary<MeshRenderer, Material> defaultCoverMeshAndMaterials = new Dictionary<MeshRenderer, Material>();
    private Vector3 defaultCoverPosition;
    private Vector3 defaultCoverAngle;
    private Vector3 defaultCoverNormal;
    private bool isAimSucceded;

    void Start()
    {
        for (int i = 0; i < Cover.GetComponentsInChildren<MeshRenderer>().Length; i++)
        {
            var renderer = Cover.GetComponentsInChildren<MeshRenderer>()[i];
            defaultCoverMeshAndMaterials.Add(renderer,renderer.material); 
        }
        defaultCoverPosition = Cover.transform.localPosition;
        defaultCoverAngle = Cover.transform.eulerAngles;
        defaultCoverNormal = Cover.transform.forward;
        isAimSucceded = false;
    }


    /// <summary>
    /// cover aims at the base(for both angle and distance), the student can put on the cover to the base, this method is subscribed to "grasp stay" event
    /// </summary>
    public void AimBase()
    {
        double angleNormal = Vector3.Angle(defaultCoverNormal, Cover.transform.forward); //can not use euler angle to compute angle directly
        double distance = Vector3.Distance(defaultCoverPosition, Cover.transform.localPosition);
        if (angleNormal <= Angle && distance < Distance) 
        {
            foreach (var key in defaultCoverMeshAndMaterials.Keys)
            {
                key.material= HighlightedCoverMaterial;//highlight the cover
            }
            isAimSucceded = true;
        }
        else
        {
            foreach (var key in defaultCoverMeshAndMaterials.Keys)
            {
                key.material = defaultCoverMeshAndMaterials[key];//set back default material to the cover
            }
            isAimSucceded = false;
        }
    }
    /// <summary>
    /// set the default material back and also the default positions, this method is subscribed to "grasp end" event
    /// </summary>
    public void ReleaseCover()
    {
        if (isAimSucceded)
        {
            foreach (var key in defaultCoverMeshAndMaterials.Keys)
            {
                key.material = defaultCoverMeshAndMaterials[key];//set back default material to the cover
            }
            Cover.transform.localPosition = defaultCoverPosition; //set back the default position
            Cover.transform.localEulerAngles = defaultCoverAngle; //set back the default angle 
            Cover.GetComponent<Rigidbody>().isKinematic = true;
            isAimSucceded = false;
        }
        else
        {
            Cover.GetComponent<Rigidbody>().isKinematic = false;
        }
    }
}
