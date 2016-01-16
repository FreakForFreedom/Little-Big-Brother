using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Device22;
using Device22.Networking;
using Device22.Logic;
using Device22.Messaging;
using LuaInterface;

namespace Device22.Game.Logic
{	
	public class LawManager : MonoBehaviour
	{
		public static List<Law> collection { get; private set; }

		private static HTTPRequest r = new HTTPRequest("127.0.0.1:3000");

		// This is needed to load all laws from the database at start. (Coroutines can't be static unfortunately)
		void Awake()
		{
			collection = new List<Law>();
		}
		
		void Start()
		{
			LawManager.LoadAllFromDatabase();

			//Lua.RegisterLuaMethod("SpawnEnemy", objectGen, objectGen.GetType().GetMethod("SpawnEnemy"));

			string luaScriptsFolder = Application.streamingAssetsPath;
			Lua.CompileScript(luaScriptsFolder + "/LawResponses.lua");
		}

		public static void RemoveLaw(Law law)
		{
			collection.Remove(law);
			DeleteFromDatabase();
		}
		
		public static bool ContainsLaw(Law law)
		{
			return collection.Contains(law);	
		}

		public IEnumerator CheckLawsWithFactQuery(HPlayerModel model, params FactQuery[] queries)
		{
			Messenger.Broadcast("I have to check with the laws what happened there.");

			// Sort list of laws by number of criteria desc.
			collection.OrderByDescending(l => l.criteria.Count()).ToList();

			Debug.Log("Checking with " + collection.Count + " laws.");

			// for each law
			foreach(Law l in collection)
			{
				// and each criteria in it
				int criteriaCount = l.criteria.Count;
				foreach(KeyValuePair<uint, Law.CriteriaValue> c in l.criteria)
				{
					// check HPlayerModel
					HPlayerModel.Trait t = HPlayerModel.Trait.TryGetTrait(c.Key);
					if (t != null)
					{
						if (c.Value.compareTo(c.Value.value, (int)model.GetNodeValue(t)))
							criteriaCount--;
					}
					else
					{
						// if criteria is not in a HPlayerModel
						// check against all queries
						foreach (FactQuery q in queries)						
						{
							int value = 0;
							// try get the concept value from player/world/... facts
							FactQuery.Concept concept = (FactQuery.Concept)c.Key;
							Debug.Log (concept);
							if (q.TryGetFactValue(concept, out value))
							{
								// if it matches with the laws criteria, continue
								if (c.Value.compareTo(value, c.Value.value))
									criteriaCount--;
								// query doesn't contain the corresponding fact value, move to next query
								else
									continue;
							}
						}
					}
					// return in 3 seconds and resume work
					yield return new WaitForSeconds(3);

					// if matching law with the most criteria was found
					if (criteriaCount == 0)
					{
						l.ApplyToGameplay();
						Messenger.Broadcast("O.K. You broke the law.");
						yield break;
					}
				}
			}
			Messenger.Broadcast("O.K. You're clean. For now...");
		}
		
		public static void DeleteFromDatabase()
		{
			try
			{
				
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}
		
		public static void SaveAllToDatabase()
		{
			try
			{
				foreach (Law l in collection)
					l.SaveToDatabase();
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}
		
		public static void LoadAllFromDatabase()
		{
			//try
			//{
				/*Dictionary<string, string> param = new Dictionary<string, string>();
				uint freeID = r.LastRequestID + 1;
				r.POST(param, freeID, ConvertDataToLaws(freeID));*/
				// ...
				Law dummy = new Law(true);
				//StartCoroutine(dummy.ConvertDataToLaws(0)); //freeID
				Coroutiner.StartCoroutine(dummy.ConvertDataToLaws(0));
			//}
			//catch (Exception e)
			//{
			//	Debug.LogError ("Exception: " + e.ToString());
			//}
		}
	}
}