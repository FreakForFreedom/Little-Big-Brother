using System;
using UnityEngine;
using Device22.Logic;
using Device22.Game.Logic;
using System.Collections;

namespace Device22.Game.Objects
{
	public class Actor : UniverseObject
	{
		[HideInInspector]
		public GameObject Ship;

        private GameObject GUIInfoText;

        public enum ObjectType { Unknown = 0, Fighter, Bomber, Scout, Miner, Station, StrangeFolk, Rebel, Asteroid }
        public ObjectType Type = ObjectType.Unknown;

		[HideInInspector]
		public int numAsteroidsDestroyed = 0,
					numAsteroidAttacks = 0,
					numShipsDestroyed = 0,
					numShipAttacks = 0,
					numPlayerAttacks;

        private static Octree _Octree;
        private bool _Destroyed;

		protected override void Awake ()
		{
			// login to UniverseState
			//Messenger.Broadcast<BaseObject>("subscribe object to universe", this);
			base.Awake ();

            if (uLink.Network.isServerOrCellServer)
            {
                GUIInfoText = Instantiate(RessourceManager.Instance.Get3DInfoText()) as GameObject;
                GUIInfoText.GetComponent<GUITextFollow>().target = this;
                GUIInfoText.GetComponent<GUIText>().text = Type.ToString();
            }
		}
	}
}

