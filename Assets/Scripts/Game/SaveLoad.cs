﻿using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;

namespace GenericData{

	/// Generic Data class
	public static class SaveLoad {
		public static string namePlayerProgress = "PlayerData";
		public static string nameLevel = "LevelData";
		public static string nameUnits = "UnitsData";

		public static bool Save(System.Object _data, string _fileName) {
			Debug.Log ("Trying to save: " + _fileName);
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
				file.Close();
				File.Delete(GetFullPath(_fileName));
				Debug.LogWarning ("Data wasn't saved: " + _fileName);
				return false;
			}
			file.Close();
			Debug.Log ("Data saved: " + _fileName);
			return true;
		}
		
		public static System.Object Load(string _fileName) {
			Debug.Log ("Trying to load: " + _fileName);
			if(!File.Exists(GetFullPath(_fileName))) return null;
			
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(GetFullPath(_fileName), FileMode.Open);
			
			System.Object data;
			
			try{
				data = bf.Deserialize(file);
			}
			catch {
				file.Close();
				Debug.LogWarning ("Data wasn't loaded: " + _fileName);
				return null;
			}
			
			file.Close();
			Debug.Log ("Data loaded: " + _fileName);
			return data;
		}


		public static bool Delete(string _fileName) {
			Debug.Log ("Trying to delete: " + _fileName);

			try {
				File.Delete(GetFullPath(_fileName));
			}
			catch (Exception) {
				Debug.LogWarning ("Data wasn't deleted: " + _fileName);
				return false;
			}

			Debug.Log ("Data deleted: " + _fileName);
			return true;
		}

		static string GetFullPath(string name) {
			return Path.Combine(Application.persistentDataPath, name + ".gd");
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