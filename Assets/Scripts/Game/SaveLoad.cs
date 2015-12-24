using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

namespace GenericData {
	
	// Generic Data class
	public static class SaveLoad {
		public static string namePlayerProgress = "PlayerData";
		public static string nameGameSessionWaypoints = "GameSessionWaypoints";
		public static string nameGameSessionUnits = "GameSessionUnits";
		public static string nameGameSessionTriggers = "GameSessionTriggers";
		
		public static bool Save(System.Object _data, string _fileName) {
			Debug.Log ("SL: Trying to save: " + _fileName);
			_data = Clone(_data);
			FileStream file;
			try {
				file = File.Create(GetFullPath(_fileName));
			}
			catch {
				return false;
			}
			
			BinaryFormatter bf = new BinaryFormatter();
			
			try{
				bf.Serialize(file, _data);
			}
			catch {
				Debug.LogWarning ("SL: Unsuccesfull saving: " + _fileName);
				file.Close();
				File.Delete(GetFullPath(_fileName));
				return false;
			}
			Debug.Log ("SL: Data saved: " + _fileName);
			file.Close();
			return true;
		}
		
		public static System.Object Load(string _fileName) {
			Debug.Log ("SL: Trying to load: " + _fileName);
			if(!File.Exists(GetFullPath(_fileName))) return null;
			
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(GetFullPath(_fileName), FileMode.Open);
			
			System.Object data;
			
			try{
				data = bf.Deserialize(file);
			}
			catch {
				Debug.LogWarning ("SL: Unsuccesfull loading: " + _fileName);
				file.Close();
				return null;
			}
			
			Debug.Log ("SL: Data loaded: " + _fileName);
			file.Close();
			return data;
		}
		
		
		public static bool Delete(string _fileName) {
			Debug.Log ("SL: Trying to delete: " + _fileName);
			
			try {
				File.Delete(GetFullPath(_fileName));
			}
			catch (Exception) {
				Debug.LogWarning ("SL: Unsuccesfull removing: " + _fileName);
				return false;
			}
			
			Debug.Log ("SL: Data deleted: " + _fileName);
			return true;
		}
		
		static string GetFullPath(string name) {
			return Path.Combine(Application.persistentDataPath, name + ".sav");
		}
		
		static T Clone<T>(T ObjToCopy) {
			using (MemoryStream ms = new MemoryStream()) {
				// serialise object
				BinaryFormatter formatter = new BinaryFormatter();
				formatter.Serialize(ms, ObjToCopy);
				ms.Position = 0;
				// deserialize object as a new one
				return (T) formatter.Deserialize(ms);
			}
		}
	}
}