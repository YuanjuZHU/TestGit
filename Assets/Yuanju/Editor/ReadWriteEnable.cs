using UnityEditor;
using UnityEngine;

public class ReadWriteEnable : AssetPostprocessor
{
    /// <summary>
    /// import setting(s)
    /// </summary>
    public void OnPreprocessModel()
    {
        var modelImporter = (ModelImporter) assetImporter;

        if (modelImporter.isReadable == false)
        {
            modelImporter.isReadable = true;
            modelImporter.SaveAndReimport();
        }

    }

}

