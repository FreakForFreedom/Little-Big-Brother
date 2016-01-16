using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Device22.Game.Logic;
using Device22.Abstract.Base;
using Device22.Game.Objects;

namespace Device22.Logic.AI
{
	public class Memory : MonoBehaviour
	{
		public GameObject Laws { get; private set; }
		public LawManager Dictator { get; private set; }
		public GameObject Stories { get; private set; }
		public StoryManager Director { get; private set; }
		public HPlayerModel PlayerModel { get; private set; }
		public Dictionary<uint, HPlayerModel> PlayersModel { get; private set; }

		private bool storyCheckIsRunning = false;

		void Awake()
		{			
			DontDestroyOnLoad(gameObject);
		}

		void Start()
		{
			// Get the hierarical player model for all players.
			PlayerModel = GameObject.Find("Universe").GetComponent<UniverseState>().PlayerModel; //new HPlayerModel(true);

			// Initialize the list containing each players individual model based on their id.
			PlayersModel = new Dictionary<uint, HPlayerModel>();

			// Add the law component to Big Brother's memory
			Laws = new GameObject("Laws");
			Laws.transform.parent = this.gameObject.transform;
			Dictator = Laws.AddComponent<LawManager>();

			// Initialize the AI Director
			Stories = new GameObject("Stories");
			Stories.transform.parent = this.gameObject.transform;
			Director = Stories.AddComponent<StoryManager>();
		}

		void LateUpdate()
		{
			if (!storyCheckIsRunning)
			{
				// Get the queries
				FactQuery fqUniverse = GameObject.Find("Universe").GetComponent<UniverseState>().GetCurrentState();
				StartCoroutine(Director.CheckForPossibleTransitionAndDoIt(result => storyCheckIsRunning = result, 60.0f, PlayerModel, fqUniverse));
			}
		}

		private sealed class WatchableObjectList<T> : IEnumerable<T> where T : BaseObject
		{
			public static ArrayList objectLists = new ArrayList();

			private readonly IList<T> collection;

			public WatchableObjectList()
			{
				collection = new List<T>();

				objectLists.Add(this);
			}

			public void Add(T o)
			{
				foreach(var obj in collection)
				{
					if (o.ID == obj.ID)
					{
						Debug.LogError("(WatchableObjectList) T o.ID already exists!");
						return;
					}
				}

				collection.Add(o);
			}
			
			/*public void AddObjects<U>(IEnumerable<U> objects)
			{
				List<U> oList = objects.ToList<U>();

				foreach(U obj in oList)
				{
					if (obj is T) {
						collection.Add((T)obj);;
					} else {
						try {
							collection.Add((T)Convert.ChangeType(readData, typeof(T)));
						} catch (InvalidCastException e) {
							Debug.LogError(e.ToString());
						}
					}
				}

				Debug.Log(string.Format("Yep, I know now {0} more things than I knew before!", objects.Count()));
			}*/

			public void RemoveObject(T o)
			{
				collection.Remove((T)o);
			}

			public int Count { get { return this.collection.Count; } }

			public T TryGetByID(uint id)
			{
				try
				{
					foreach(var obj in collection)
					{
						if (obj.ID == id)
						{
							return (T)obj;
						}
					}
					Debug.LogError("(WatchableObjectList) TryGetByID: Object with ID " + id + " was not found!");
					return null;
				}
				catch (Exception e)
				{
					Debug.LogError(e.ToString());
					return null;
				}
			}

			#region IEnumerable<T> Members
			public IEnumerator<T> GetEnumerator() {
				foreach (var item in collection) yield return item;
			}
			#endregion
			
			#region IEnumerable Members
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
				return collection.GetEnumerator();
			}
			#endregion

		}

		/*public void AddWatchableObjects(System.Object o)
		{
			Type t;

			// If o is an array or an collection
			if ((Util.IsArray(o)) || (Util.IsGenericCollection(o)))
			{
				IEnumerable tmp = (IEnumerable)o;
				t = tmp.GetType().GetGenericArguments().Single();
				var list = GetWatchableObjectListWithType(t).list.AddRange(tmp);
			}
			// or just an object
			else
			{
				t = typeof(o);
				GetWatchableObjectListWithType(t).list.Add(o);

			}
		}*/

		// Set all lists to watch
		private void AddToWatchableObjects<T>(IEnumerable<T> objectList) where T : BaseObject
		{
			// If param is an array or an collection
			if ((Util.IsArray(objectList)) || (Util.IsGenericCollection(objectList)))
			{
				Type type = objectList.GetType().GetGenericArguments().SingleOrDefault();
				foreach (WatchableObjectList<T> wol in WatchableObjectList<T>.objectLists)
				{
					Type type2 = wol.GetType().GetGenericArguments().SingleOrDefault();
					// Is this comparison really obligatory? Is foreach(someDic<T> in ...) not enough?
					if (type == type2)
					{
						foreach (T o in objectList)
							wol.Add(o);

						Debug.Log("'Sup, I remember now " + objectList.Count() + " new objects, and they are " + objectList.FirstOrDefault().GetType().ToString() + ". There are now " + wol.Count() + " of them.");
						return;
					}
				}
				
				WatchableObjectList<T> tmp = new WatchableObjectList<T>();
				foreach (T o in objectList)
					tmp.Add(o);

				Debug.Log("'Sup, I remember now " + tmp.Count() + " new objects, and they are " + objectList.FirstOrDefault().GetType().ToString());
			}
			// or some weird custom list/array/thingy
			// (do nothing)
		}

		public void RemoveObject<T>(T a) where T : BaseObject
		{
			Type type = a.GetType();
			//Debug.Log ("Original type: " + type.ToString());
			foreach (WatchableObjectList<T> wol in WatchableObjectList<T>.objectLists)
			{
				Type type2 = wol.GetType().GetGenericArguments().SingleOrDefault();
				//Debug.Log ("Search type2: " + type.ToString());
				if (type == type2)
				{
					// Check if there is a ID of the destroyer.
					if (a.LastDamageDealerID > 0)
					{
						StartCheckingLaws(a.LastDamageDealerID);
					}

					wol.RemoveObject(a);
					return;
				}
			}
		}

		public void RemoveActor<T>(T a) where T : Actor
		{
			Type type = a.GetType().GetGenericArguments().SingleOrDefault();
			foreach (WatchableObjectList<T> wol in WatchableObjectList<T>.objectLists)
			{
				Type type2 = wol.GetType().GetGenericArguments().SingleOrDefault();
				if (type == type2)
				{
					wol.RemoveObject(a);
					HPlayerModel.CURRENT_NUM_ACTORS--;
					PlayersModel.Remove(a.ID);
					return;
				}
			}
		}

		public void AddNewActor<T>(T a) where T : Actor
		{
			AddNewObject(a);
			HPlayerModel.CURRENT_NUM_ACTORS++;
			PlayersModel.Add(a.ID, new HPlayerModel(false));
		}

		public void AddNewObject<T>(T o) where T : BaseObject
		{
			List<T> list = new List<T>();
			list.Add(o);
			AddToWatchableObjects(list);
		}

		public void AddNewActors<T>(IEnumerable<T> list) where T : Actor
		{
			foreach(Actor a in list)
			{
				AddNewObject(a);
			}
			HPlayerModel.CURRENT_NUM_ACTORS += list.Count();
		}
		
		public void AddNewObjects<T>(IEnumerable<T> list) where T : BaseObject
		{
			Debug.Log("'Oi, I remember now a few new objects, and there are " + list.Count().ToString() + " of 'em.");
			AddToWatchableObjects(list);
		}

		public void StartCheckingLaws(uint actorID)
		{
			// First get the destroyers model
			HPlayerModel m;
			PlayersModel.TryGetValue(actorID, out m);

			// Get the queries
			FactQuery fqUniverse = GameObject.Find("Universe").GetComponent<UniverseState>().GetCurrentState();
			FactQuery fqActor = GetCurrentStateOfActor<Actor>(actorID);
			// Then check against all laws
			StartCoroutine(Dictator.CheckLawsWithFactQuery(m, fqUniverse, fqActor));
		}

		public FactQuery GetCurrentStateOfActor<T>(uint id) where T : Actor
		{
			foreach (WatchableObjectList<T> wol in WatchableObjectList<T>.objectLists)		// Get the list. There should't be more than one, but you can never be too sure...
			{
				T a = wol.TryGetByID(id);
				if (a != null)
				{
					FactQuery q = new FactQuery();

					q.Add(FactQuery.Concept.WHO, 0);
					q.Add(FactQuery.Concept.SHIPTYPE, (int)a.Type);
					q.Add(FactQuery.Concept.DESTROYED_SHIPS, a.numShipsDestroyed);
					q.Add(FactQuery.Concept.DESTROYED_ASTEROIDS, a.numAsteroidsDestroyed);
					q.Add(FactQuery.Concept.PLAYER_ATTACKS, a.numPlayerAttacks);
					// ...

					return q;
				}
			}

			Debug.LogError("Hey, I don't remember the Actor with the ID " + id + "! Whats up?"); 
			return null;
		}
	}
}