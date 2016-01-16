using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Device22.Networking.Server;

namespace Device22.Game.Logic
{
	public class Law
	{
		public struct CriteriaValue
		{
			public int value;
			public Func<int, int, bool> compareTo;
			
			// linear comparison to speed things up a little bit
			public CriteriaValue(int value, uint compareType)
			{
				this.value = value;
				switch(compareType)
				{
					// ==
				case(0):
					this.compareTo = (x, y) => x <= y && x >= y ? true : false;
					break;
					// <
				case(1):
					this.compareTo = (x, y) => x < y ? true : false;
					break;
					// >
				case(2):
					this.compareTo = (x, y) => x > y ? true : false;
					break;
				default:
					Debug.LogWarning("Criteriavalue compare not found, assuming default '=='.");
					this.compareTo = (x, y) => x <= y && x >= y ? true : false;
					break;
				}
			}
		}
		
		public SortedDictionary<uint, CriteriaValue> criteria { get; private set; }
		private String response;
		private bool active = true;
		
		public Law(bool dummy = false)
		{
			if (!dummy)
			{
				Debug.Log("new Law()");
				criteria = new SortedDictionary<uint, CriteriaValue>();
				LawManager.collection.Add(this);
				this.SaveToDatabase();
			}
		}
		
		public void AddCriteria(uint key, int value, uint compareType)
		{
			if (criteria.ContainsKey(key))
			{
				Debug.LogWarning("Tried to add a criteria to a law which already exists in it!");
				return;
			}
			criteria.Add(key, new CriteriaValue(value, compareType));
			//Debug.Log("new Criteria: " + value + " : " + compareType);
		}
		
		public void RemoveCriteria(uint key)
		{
			if (criteria.ContainsKey(key))
				criteria.Remove(key);
		}
		
		public void ApplyToGameplay()
		{
			Debug.Log("Calling Lua Function: " + response);
			GameServer gs = GameObject.Find("_Server").GetComponent<GameServer>();
			Lua.CallLuaFunction(response, gs);
		}
		
		public IEnumerator ConvertDataToLaws(uint id)
		{
			string data;
			//r.Data.TryGetValue(id, out data);
			
			// Add some laws for debugging - ToDo: Store in an extra file.
			//data = "4:10|2,5:6|1?spawnenemy;1:0|0?spawnenemy";
			// Who: Player, Destroyed_Asteroids > 0 => Spawnasteroids();"
			// Law: All players destroyed
			data = "100:0|0?gameover";
			// Law: All strange folks destroyed
			data += ";105:0|0?gamewon";
			// Law: All rebels destroyed
			data += ";106:0|0?gamewon";
			// Law: Player attacks player
			data += ";0:0|0,10:0|2?stopattackingyourfriends";

			Debug.Log("Law Collection contains already " + LawManager.collection.Count + " laws.");

			string[] laws = data.Split(';');
			// For each law found in the string
			foreach (string law in laws)
			{
				Law l = new Law();
				
				string[] criteriaResponse = law.Split('?');
				if (criteriaResponse[1] != "null") l.response = criteriaResponse[1];
				string[] criteria = criteriaResponse[0].Split(',');
				// And each criteria found that way
				foreach (string c in criteria)
				{
					string[] keyValue = c.Split(':');
					string[] valueCompare = keyValue[1].Split('|');
					uint key = uint.MaxValue, compareType = uint.MaxValue;
					int value = int.MaxValue;
					// Convert the values
					key = Convert.ToUInt32(keyValue[0]);
					value = Convert.ToInt32(valueCompare[0]);
					compareType = Convert.ToUInt32(valueCompare[1]);
					
					if ((key == uint.MaxValue) || (value == int.MaxValue) || (compareType == uint.MaxValue))
						Console.WriteLine("ERROR (Law): Input string is not a sequence of digits.");
					// And add them as criteria
					l.AddCriteria(key, value, compareType);
				}

				yield return null;

				//LawManager.collection.Add(l);
			}

			Debug.Log(LawManager.collection.Count + " new Laws added!");

			yield break;
		}
		
		public void LoadFromDatabase()
		{ 
			try
			{
				
			}
			catch (Exception e)
			{
				
			}
		}
		
		public void SaveToDatabase()
		{
			try
			{
				
			}
			catch (Exception e)
			{
				
			}
		}
	}
}