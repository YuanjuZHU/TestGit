using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leap.Unity;
using UnityEngine;
using Debug = UnityEngine.Debug;
using NPOI.HSSF.UserModel;
using NPOI.SS.Formula.Functions;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System.IO;
using NPOI.SS.UserModel;


namespace Assets.Yuanju.Interfaces_and_classes.generator_components
{
    class SteamGenerator : MonoBehaviour
    {
        //TODO write a function like "SetGeneratorSettings" accepts the "InitialSettings", "FinalSettings"(strings, sheet names), another arguement(lists in GetDtateFromExcel,ref)  [SetSettingParameters(string sheetName, /ref List<ElementSettings> settingList)/ ref part can be removed]
        public GameObject steamGenerator;

        #region initial final settings
        public static List<ElementSettings> InitialSettings = new List<ElementSettings>(); //each element in the list is a row in the excel
        public static List<ElementSettings> FinalSettings = new List<ElementSettings>();//each element in the list is a row in the excel
        public static List<ElementSettings> FinalParametersSetting = new List<ElementSettings>();
        public string FilePath = "Assets/Yuanju/Values and situations plus Parameters Tirreno Power.xlsx"; /*Assets/Yuanju/Values and situations plus Parameters.xlsx*/
        public static IEnumerable<string> Tasks; //the tasks in the initial and final setting sheets
        public static IEnumerable<string> SubTasks; // the sub tasks in the initial and final setting sheets
        public static IEnumerable<string> Problems; //the "Generator Status" in element setting and excel sheet
        public static List<string> InitialSettingElements; //the elements that appear in the initial setting sheet
        public static List<string> FinalSettingElements; //the elements that appear in the final setting sheet
        public static List<string> FinalParamterElements; //the elements that appear in the final setting paramters sheet
        public static IEnumerable<string> SequenceElements; //the elements that appear in the yellow row in the sequence sheets
        #endregion

        #region Sequences
        public static List<Sequence> SequenceMatrices = new List<Sequence>(); //this list contains all the tables from the sequence sheets

        #endregion

        void Awake()
        {
            SetGeneratorSettings();
        }

        public SteamGenerator(GameObject steamGenerator)
        {
            this.steamGenerator = steamGenerator;
        }

        public List<Serializer.item> GetComponentsStatus() {

            List<Serializer.item> components = new List<Serializer.item>();
            var childrenList = new List<Transform>();
            steamGenerator.transform.GetAllChildren(childrenList);

            foreach(string compName in SequenceElements/*NPOIGetSequenceTable.OperableComponentNames*/)
            {
                var compGameObject = childrenList.Find(comp => comp.name == compName);

                if (compGameObject)
                {
                    var switchComponent = compGameObject.GetComponent<Switch>();
                    var buttonComponent = compGameObject.GetComponent<Button>();
                    var valveComponent = compGameObject.GetComponent<Valvola>();
                    var lcdComponent = compGameObject.GetComponent<LiquidCristalDisplay>();

                    var powerBloched = 0;
                    var status = 0;
                    if (switchComponent)
                    {
                        //powerBloched = switchComponent.IsPowerConnected;
                        status = switchComponent.Status;
                    }
                    else if (buttonComponent)
                    {
                        status = buttonComponent.Status;
                    }
                    else if(valveComponent)
                    {
                        status = valveComponent.Status;
                    }
                    else if(lcdComponent) {
                        status = lcdComponent.Status;
                    }


                    var comp = new Serializer.item(compName, status, powerBloched);
                    //var comp = new Serializer.item(compName, switchComponent.Status, powerBloched);
                    components.Add(comp);

                }
                else
                {
                    Debug.Log("comp da serializzare non trovata " + compName);
                }
            }

            return components;

        }

        public List<Serializer.item> SetComponentsStatus(List<Serializer.item> components) {

            var childrenList = new List<Transform>();
            steamGenerator.transform.GetAllChildren(childrenList);
            foreach(Serializer.item itemComp in components) {
                var compGameObject = childrenList.Find(comp => comp.name == itemComp.name);
                //Debug.Log("comp da serializzare " + itemComp.name);

                if(compGameObject) {
                    var switchComponent = compGameObject.GetComponent<Switch>();
                    var buttonComponent = compGameObject.GetComponent<Button>();
                    var gaugeComponent = compGameObject.GetComponent<PressureGauge>();
                    var waterLevelIndicatorComponent = compGameObject.GetComponent<WaterLevelIndicator>();
                    var waterTankComponent = compGameObject.GetComponent<WaterTank>();
                    var ledComponent = compGameObject.GetComponent<LED>();

                    if(switchComponent) {
                        if (itemComp.powerBlock == 0)
                        {
                            switchComponent.IsPowerConnected = false;
                        }
                        else
                        {
                            switchComponent.IsPowerConnected = true;
                        }

                        //Debug.Log(String.Format("switch {0} aggiornato stato {1}", itemComp.name, switchComponent.Status));
                    }
                    else if(gaugeComponent)
                    {
                        double temporaryStatus = itemComp.status;
                        gaugeComponent.Status = (int)Math.Round(temporaryStatus, 0);
                        //Debug.Log(String.Format("pressione {0} aggiornato stato {1}", itemComp.name, itemComp.status));
                    }
                    else if(waterLevelIndicatorComponent) {
                        double temporaryStatus = itemComp.status;
                        waterLevelIndicatorComponent.Status = (int)Math.Round(temporaryStatus, 0);
                        Debug.Log(String.Format("water level {0} aggiornato stato {1}", temporaryStatus, waterLevelIndicatorComponent.Status));
                    }
                    else if(waterTankComponent) {
                        double temporaryStatus = itemComp.status;
                        waterTankComponent.Status = (int)Math.Round(temporaryStatus, 0);
                        //Debug.Log(String.Format("pressione {0} aggiornato stato {1}", itemComp.name, itemComp.status));
                    }
                    else if(ledComponent) {
                        if (itemComp.status==1)
                        {
                            ledComponent.Status = 1;
                        }
                        else
                        {
                            ledComponent.Status = 0;
                        }
                        
                        //Debug.Log(String.Format("pressione {0} aggiornato stato {1}", itemComp.name, itemComp.status));
                    } else
                    {
                        Debug.Log("classe della comp da deserializzare non trovata " + itemComp.name);
                    }


                } else {
                    Debug.Log("comp da deserializzare non trovata " + itemComp.name);
                }
            }

            return components;

        }

        public Serializer.list SerializeGeneratorStatus()
        {
            var componentsStatusList = GetComponentsStatus();
            var compList = new Serializer.list(componentsStatusList);
            return compList;
        }

        public string GetJson(Serializer.list compList)
        {
            string json = JsonUtility.ToJson(compList, true);
            Debug.Log(json);

            return json;
        }

        public List<Serializer.item> DeserializeGeneratorStatus(string json) {

            //Debug.Log(json);
            var compList = JsonUtility.FromJson<Serializer.list>("{\"componentList\":" + json + "}");
            return compList.componentList;
        }





        public void SetGeneratorSettings(/*string sheetName*/)
        {

            IWorkbook wk =GetDataFromExcel.OpenCloseExcelFile(FilePath/*"Assets/Yuanju/Values and situations plus Parameters.xlsx"*/); //TODO should be included in "SteamGenerator.cs" for the setting of steam generator(s)
            InitialSettings = GetDataFromExcel.ReadInitialFinalSetting(wk, "initial settings");
            FinalSettings = GetDataFromExcel.ReadInitialFinalSetting(wk, "final settings");
            FinalParametersSetting = GetDataFromExcel.ReadInitialFinalSetting(wk, "final settings parameters");
            SequenceMatrices = GetDataFromExcel.ReadSequenceSheets(wk);


            Tasks = InitialSettings.Select(x => x.Task);
            SubTasks = InitialSettings.Select(x => x.SubTask);
            Problems= InitialSettings.Select(x => x.GeneratorStatus);
            Debug.Log("SequenceMatrices[0]: " + SequenceMatrices[0]);
            Debug.Log("SequenceMatrices[0].ActuatorToCheck[0]: " + SequenceMatrices[0].ActuatorToCheck[0]);
            SequenceElements = SequenceMatrices[0].ActuatorToCheck.Select(x => x.Name);

            InitialSettingElements = new List<string>(InitialSettings[0].ElementStatus.Keys);
            FinalSettingElements = new List<string>(FinalSettings[0].ElementStatus.Keys);
            FinalParamterElements = new List<string>(FinalParametersSetting[0].ParameterSetting.Keys);
        }
    }
}
