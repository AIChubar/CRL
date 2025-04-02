using System;
using System.IO;
using UnityEngine;

/// <summary>
/// Script that manages writing and reading data from the file.
/// </summary>
public class FileDataHandler
{
    private string dataDirPath = "";

    private string dataFileName = "";

    public FileDataHandler(string dataDirPath, string dataFileName)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
    }

    public void Load(GameData existingGameData)
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = File.ReadAllText(fullPath);
                GameDataSerializable serializableData = JsonUtility.FromJson<GameDataSerializable>(dataToLoad);
                serializableData.ApplyToGameData(existingGameData);
            }
            catch (Exception e)
            {
                Debug.Log("Error occurred when trying to load data from file: " + fullPath + "\n" + e);
            }
        }
    }


    public void Save(GameData data)
    {
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            GameDataSerializable serializableData = new GameDataSerializable(data);
            string dataToStore = JsonUtility.ToJson(serializableData, true);
            File.WriteAllText(fullPath, dataToStore);
        }
        catch (Exception e)
        {
            Debug.LogError("Error occurred when trying to save data to file: " + fullPath + "\n" + e);
        }
    }


}
