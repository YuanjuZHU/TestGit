using System.Collections;
using System.Collections.Generic;
using Leap;
using UnityEngine;

public class BlinkMaterial : MonoBehaviour
{
    //MeshRenderer components
    private MeshRenderer[] componentRenderers;
    private List<Material> originalMaterials=new List<Material>();
    public static Material blinkMat;
    [HideInInspector]public Material BlinkMat;
    public Material FirstErrorActuatorBlinkMat;
    public Material ErrorAlarmBlinkMat;

    //delay to start blinking
    float BlinkTime = 0f;
    //if start blinking
    public bool IsBlink = false;
    private bool isMaterialGet;
    private int previousStatus;

    void Awake()
    {
        blinkMat = BlinkMat;
    }

    void Start() {
        componentRenderers = gameObject.GetComponentsInChildren<MeshRenderer>();
        isMaterialGet = false;
        previousStatus = gameObject.GetComponent<IGeneratorComponent>().Status;
    }


    // Update is called once per frame
    void Update() {
        //detect if the status of the components has been changed, if yes, needs to re-pick the new materials 
        if(previousStatus != gameObject.GetComponent<IGeneratorComponent>().Status) {
            isMaterialGet = false;
            gameObject.GetComponent<IGeneratorComponent>().UpdateMaterials();
            previousStatus = gameObject.GetComponent<IGeneratorComponent>().Status;
        }

        ChangeColor();
    }
    /// <summary>
    /// logic to change color
    /// </summary>
    public void ChangeColor()
    {
        if(componentRenderers[0]!= BlinkMat)
        {
            if (!isMaterialGet)
            {
                originalMaterials.Clear();
                foreach(var meshRenderer in componentRenderers) {
                    var material = new Material(Shader.Find("Standard"));
                    material.color = meshRenderer.material.color;
                    originalMaterials.Add(material);
                }
                isMaterialGet = true;
            }
            ////// set the default material
            ////for(int i = 0; i < componentRenderers.Length; i++) {
            ////    componentRenderers[i].material = originalMaterials[i];
            ////    Debug.Log("the original material color: " + originalMaterials[i].color);
            ////}
            //gameObject.GetComponent<IGeneratorComponent>().UpdateMaterials();

            //for(int i = 0; i < componentRenderers.Length; i++) {
            //    componentRenderers[i].material = originalMaterials[i];
            //}
        }
        if(IsBlink)
        {
            //isMaterialGet = false;
            BlinkTime += Time.deltaTime;
            if (BlinkTime % 1 > 0.8f)
            {
                foreach (var renderer in componentRenderers)
                {
                    renderer.material = BlinkMat;
                }
                
            }
            if(BlinkTime % 1 <= 0.8f) 
            {

                for (int i = 0; i < componentRenderers.Length; i++)
                {
                    componentRenderers[i].material = originalMaterials[i];
                }                 
            }
        }

    }
    public void SetDefaultMaterialBack()
	{
	    gameObject.GetComponent<IGeneratorComponent>().UpdateMaterials();
        //IGeneratorComponent[] IGeneratorComponent = GetComponents<IGeneratorComponent>();
        //var foundTextMeshObjects = FindObjectsOfType(typeof(IGeneratorComponent));
        //Debug.Log("the count of the IGeneratorComponent: " + foundTextMeshObjects.Length);
        //Debug.Log("the name of the IGeneratorComponent: " + foundTextMeshObjects[1].name);
        //Debug.Log("Set default material back in blink material: " +gameObject);
    }

    public void DisableBlinkMaterial()
    {
        gameObject.GetComponent<BlinkMaterial>().IsBlink = false;
        gameObject.GetComponent<BlinkMaterial>().enabled = false;
    }
}