using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
//using Mono.Data.Sqlite;

public static class DBHelper 
{
    //public static void CreateDB(string p_dbName) {
    //    var folder = Directory.CreateDirectory($"{Application.streamingAssetsPath}/Inventory/");
    //    string dbPath = $"URI=file:{folder}/{p_dbName}.db";

    //    using (var connection  = new SqliteConnection(dbPath)) {
    //        connection.Open();
    //        connection.Close();
    //    }
    //}

    public static void SaveToJson(string p_json, string p_fileName) {
        string path = Application.persistentDataPath + Path.AltDirectorySeparatorChar + p_fileName;

        using (StreamWriter writer = new StreamWriter(path)) {
            writer.Write(p_json);
        }
    }

    public static T LoadFromJson<T>(string p_fileName) where T : new() {
        string path = Application.persistentDataPath + Path.AltDirectorySeparatorChar + p_fileName;
        string json = "";
        if(File.Exists(path)) {
            using (StreamReader reader = new StreamReader(path)) {
                json = reader.ReadToEnd();
            }
        }

        T ob = JsonUtility.FromJson<T>(json);

        if(ob == null) {
            return new T();
        }
        return ob;
    }


}
