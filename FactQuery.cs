using System;
using System.Collections;
using System.Collections.Generic;

namespace Device22.Game.Logic
{
	public class FactQuery
	{
		public enum Concept : uint
		{
			// Player stats
			WHO = 0,			// Player/NPC: 0
			POSITION = 1,
			ROTATION = 2,
			// Individual player
			SHIPTYPE = 3,		// Fighter: 0, Miner: 1
			POINTS = 4,
			MAXDRONES = 5,
			DESTROYED_SHIPS = 6,
			DESTROYED_ASTEROIDS = 7,
			SHIP_ATTACKS = 8,
			ASTEROID_ATTACKS = 9,
			PLAYER_ATTACKS = 10,

			// ...
			// All player stats
			GLOBAL_NUM_PLAYERS = 100,
			GLOBAL_NUM_SHIPS = 101,
			GLOBAL_NUM_DRONES = 102,
			GLOBAL_NUM_STATIONS = 103,
			GLOBAL_NUM_ASTEROIDS = 104,
			GLOBAL_NUM_STRANGEFOLKS = 105,
			GLOBAL_NUM_REBELS = 106,
			GLOBAL_NUM_DESTROYED_PLAYERS = 110,
			GLOBAL_NUM_DESTROYED_SHIPS = 111,
			GLOBAL_NUM_DESTROYED_DRONES = 112,
			GLOBAL_NUM_DESTROYED_STATIONS = 113,
			GLOBAL_NUM_DESTROYED_ASTEROIDS = 114,

			// Game specific stats
			GLOBAL_PLAYER_FIGHT_STRANGEFOLK = 10001,
			GLOBAL_PLAYER_MINE_ASTEROIDS = 10002,
			GLOBAL_PLAYER_DESTROY_ASTEROIDS = 10003
		}

		private SortedDictionary<Concept, int> facts;

		public int Count { get { return facts.Count; } }

		public FactQuery ()
		{
			facts = new SortedDictionary<Concept, int>();
		}

		public void Add(Concept concept, int value)
		{
			facts.Add(concept, value);
		}

		public bool TryGetFactValue(Concept concept, out int value)
		{
			if (facts.ContainsKey(concept))
			{
			    value = facts[concept];
			    return true;
			}
			else
				value = 0;
				return false;
		}
	}
}

