using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Device22.Networking.Server;

namespace Device22.Game.Logic
{
	public class StoryElement
	{
		public static List<StoryElement> collection = new List<StoryElement>();

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
		
		public SortedDictionary<uint, CriteriaValue> transitionToCriteria { get; private set; }
		private string stateResponse;
		public uint ID = 0;
		public List<uint> neighborStateID { get; private set; }

		public StoryElement(bool dummy = false)
		{
			if (!dummy)
			{
				Debug.Log("new StoyElement()");
				transitionToCriteria = new SortedDictionary<uint, CriteriaValue>();
				neighborStateID = new List<uint>();
				collection.Add(this);
				this.SaveToDatabase();
			}
		}
		
		public void AddTransitionCriteria(uint key, int value, uint compareType)
		{
			if (transitionToCriteria.ContainsKey(key))
			{
				Debug.LogWarning("Tried to add a criteria to a law which already exists in it!");
				return;
			}
			transitionToCriteria.Add(key, new CriteriaValue(value, compareType));
			//Debug.Log("new Criteria: " + value + " : " + compareType);
		}
		
		public void RemoveTransitionCriteria(uint key)
		{
			if (transitionToCriteria.ContainsKey(key))
				transitionToCriteria.Remove(key);
		}
		
		public void ApplyToGameplay()
		{
			Debug.Log("Calling Lua Function: " + stateResponse);
			GameServer gs = GameObject.Find("_Server").GetComponent<GameServer>();
			Lua.CallLuaFunction(stateResponse, gs);
		}

		public static StoryElement GetByID(uint ID)
		{
			StoryElement returnSE = new StoryElement(true);
			foreach (StoryElement se in collection)
			{
				if (se.ID == ID) returnSE = se;
			}
			return returnSE;
		}
		
		public IEnumerator ConvertDataToStoryElements(uint id, Action<StoryElement> setRootCallback)
		{
			string data;
			int count = 0;
			bool rootAssigned = false;

			// Normal StoryElement
			// data = "4:10|2,5:6|1?spawnenemy!0:1,2;";
			// Who: Player, Destroyed_Asteroids > 0 => Spawnasteroids();"
			
			// Add some storyElements for debugging - ToDo: Store in an extra file.
			// Root story element: Players arrives at the asteroid field (ist's a dummy!)
			data = "0:0|0?donothing!0:1,2";
			// First node: Players stats to mine on those asteroids (uint80 == float0.8)
			data += ";10002:80|2?asteroidminingstarted!1:2,3";
			// Second node: Players starts to attack the strange folks
			data += ";10001:80|2?playerattacks!2:3,4,5";
			// Third node: Players destroys asteroid
			data += ";10003:80|2?playerdestroysasteroid!3:2,6";
			// Fourth node: Strange folks attacks the players, if there are less players than strange folks (<3)
			data += ";100:2|1?attackallplayers!4:7,9";
			// Fifth node: Strange folks runs like chicken if there are more players than strange folks (>3)
			data += ";100:2|2?fleefromplayers!5:7";
			// Sixth node: Rebel fleet arrives when there are no asteroids left
			data += ";104:0|0?spawnrebels!6:8,9";
			// Seventh node: Players win the battle against the strange volks
			data += ";105:0|0?gamewon!7:-1";
			// Eigth node: Players win the battle against the rebel fleet
			data += ";106:0|0?gamewon!8:-1";
			// Ninth node: Players loose the battle
			data += ";100:0|0?gameover!9:-1";
			
			Debug.Log("Story Collection contains already " + StoryElement.collection.Count + " elements.");
			
			string[] storyElements = data.Split(';');
			foreach (string sElement in storyElements)
			{
				StoryElement se = new StoryElement();
				count++;

				string[] criteriaResponse = sElement.Split('?');
				string[] criteriaResponseID = criteriaResponse[1].Split('!');
				string[] IDs = criteriaResponseID[1].Split(':');
				se.ID = Convert.ToUInt32(IDs[0]);
				string[] toIDs = IDs[1].Split(',');
				foreach (string toID in toIDs)
				{
					int tmpID = Convert.ToInt32(toID);
					if (tmpID >= 0) se.neighborStateID.Add((uint)tmpID);
				}
				if (!criteriaResponse[1].Contains("null")) se.stateResponse = criteriaResponseID[0];
				string[] criteria = criteriaResponse[0].Split(',');
				foreach (string c in criteria)
				{
					string[] keyValue = c.Split(':');
					string[] valueCompare = keyValue[1].Split('|');
					uint key = uint.MaxValue, compareType = uint.MaxValue;
					int value = int.MaxValue;
					
					key = Convert.ToUInt32(keyValue[0]);
					value = Convert.ToInt32(valueCompare[0]);
					compareType = Convert.ToUInt32(valueCompare[1]);
					
					if ((key == uint.MaxValue) || (value == int.MaxValue) || (compareType == uint.MaxValue))
						Console.WriteLine("ERROR (StoryElement): Input string is not a sequence of digits.");
					
					se.AddTransitionCriteria(key, value, compareType);
				}
				
				if (!rootAssigned)
				{
					setRootCallback(se);
					rootAssigned = true;
				}

				//yield return null;
			}
			
			Debug.Log(count + " new StoryElements added!");
			
			yield break;
		}
		
		public void LoadFromDatabase() { }
		
		public void SaveToDatabase() { }
	}
}