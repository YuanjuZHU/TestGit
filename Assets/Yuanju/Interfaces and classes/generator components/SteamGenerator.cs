using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leap.Unity;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Assets.Yuanju.Interfaces_and_classes.generator_components
{
    class SteamGenerator : MonoBehaviour
    {
        public GameObject steamGenerator;

        public SteamGenerator(GameObject steamGenerator)
        {
            this.steamGenerator = steamGenerator;
        }

        public List<Serializer.item> GetComponentsStatus() {

            List<Serializer.item> components = new List<Serializer.item>();
            
            foreach(string compName in NPOIGetDatatable.OperableComponentNames)
            {
                var childrenList = new List<Transform>();
                steamGenerator.transform.GetAllChildren(childrenList);
                var compGameObject = childrenList.Find(comp => comp.name == compName);

                if (compGameObject)
                {
                    var switchComponent = compGameObject.GetComponent<Switch>();
                    var powerBloched = false;
                    if (switchComponent)
                    {
                        powerBloched = switchComponent.IsPowerConnected;
                    }


                    var comp = new Serializer.item(compName, SetScenes.ScriptComponents[compName].Status, powerBloched);
                    components.Add(comp);
                }
                else
                {
                    Debug.Log("comp da serializzare non trovata " + compName);
                }
            }

            return components;

        }

        public void SerializeGeneratorStatus()
        {
            var componentsStatusList = GetComponentsStatus();

            var compList = new Serializer.list(componentsStatusList);
            string json = JsonUtility.ToJson(compList, true);
            System.IO.File.WriteAllText(Application.persistentDataPath + "/OperableComponentStatus.json", json);

            Debug.Log("Salvato JSON in: " + Application.persistentDataPath);

        }
    }
}
