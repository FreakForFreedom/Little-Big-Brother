using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Device22.Game.Logic;

namespace Device22.Logic
{
	public class HPlayerModel
	{
		public static string XMLDocUrl = Application.streamingAssetsPath + "/Modelable Traits.xml";
		public static float CURRENT_NUM_ACTORS = 1;

		private HModelNode root;

		/*private static HPlayerModel instance;
		public static HPlayerModel Instance
		{
			get
			{
				if (instance == null)
					instance = new HPlayerModel();
				return instance;
			}
		}*/

		public HPlayerModel(bool multiplayer)
		{
			XmlDocument xdoc = new XmlDocument();
			xdoc.Load(XMLDocUrl);

			root = new HModelNode(new Trait(0, "Player's style", 0f, multiplayer), null);
			Debug.Log(xdoc.SelectNodes("/TRAITS/TRAIT").Count + " modelable traits where found!");
			foreach (XmlNode trait in xdoc.SelectNodes("/TRAITS/TRAIT"))
			{
				RecuAddNodeFromXML(trait, root, multiplayer);
			}
		}

		private void RecuAddNodeFromXML(XmlNode node, HModelNode parent, bool multiplayer)
		{
			Debug.Log (node["ID"].InnerText);
			Trait t = new Trait(Convert.ToUInt32(node["ID"].InnerText), node["NAME"].InnerText, Convert.ToDouble(node["LEARNINGRATE"].InnerText), multiplayer);
			HModelNode tmp = new HModelNode(t, parent);
			foreach (XmlNode subTrait in node.SelectNodes("/CHILDREN/*"))
			{
				RecuAddNodeFromXML(subTrait, tmp, multiplayer);
			}
		}

		public void UpdateTrait(Trait trait, float observedValue)
		{
			HModelNode leaf = FindNode(trait);
			leaf.UpdateTrait(observedValue);
		}
		
		public void UpdateTrait(String traitName, float observedValue)
		{
			Trait t = Trait.TryGetTrait(traitName);
			if (t == null)
			{
				Debug.LogError("Trait " + traitName + "could not be found!");
				return;
			}
			HModelNode leaf = FindNode(t);
			leaf.UpdateTrait(observedValue);
		}
		
		public void UpdateTrait(uint traitID, float observedValue)
		{
			Trait t = Trait.TryGetTrait(traitID);
			if (t == null)
			{
				Debug.LogError("Trait " + traitID + "could not be found!");
				return;
			}
			HModelNode leaf = FindNode(t);
			leaf.UpdateTrait(observedValue);
		}

		public void UpdateTrait(FactQuery.Concept traitConcept, float observedValue)
		{
			Trait t = Trait.TryGetTrait((uint)traitConcept);
			if (t == null)
			{
				Debug.LogError("Trait " + (uint)traitConcept + "could not be found!");
				return;
			}
			HModelNode leaf = FindNode(t);
			leaf.UpdateTrait(observedValue);
		}

		public void NormalizeTraits()
		{
			Trait.NormalizeAll();
		}

		public uint GetNodeValue(Trait trait)
		{
			return FindNode(trait).GetTraitValueAsUInt();
		}

		public HModelNode FindNode(Trait trait)
		{
			//return RecuFindNode(trait, root);
			foreach (HModelNode n in HModelNode.collection)
			{
				if (n.HasTrait(trait)) return n;
			}
			return null;
		}

		private HModelNode RecuFindNode(Trait trait, HModelNode node)
		{
			if (node.HasTrait(trait)) return node;
			HModelNode tmp = null;
			foreach (HModelNode n in node.children)
				tmp = RecuFindNode(trait, n);
			return tmp != null? tmp : null;
		}

		public class Trait
		{
			private static List<Trait> traits = new List<Trait>();

			public uint ID { get; private set; }
			public string Name { get; private set; }
			public double Value = 0.5d;
			public double LearningRate { get; private set; }
			// set this bool to true, if the trait should be an average for all players style.
			private bool multiplay = false;

			public Trait(uint id, string name, double learningRate, bool multiplayer)
			{
				//Trait tmp = TryGetTrait(id);
				//if (tmp != null)
				//{
					//Debug.LogError(string.Format("ERROR (Trait): Trait with ID {0} already exists!", id));
					//return;
				//traits.Add(this);
				//}

				Debug.Log ("New trait with id: " + id);
				traits.Add(this);

				this.ID = id;
				this.Name = name;
				// A trait starts with a value of 0.5 since it hasn't been checked or updated yet.
				this.Value = 0.5d;
				this.LearningRate = learningRate;
				this.multiplay = multiplayer;
			}

			public void Update(double observedValue)
			{
				double tmp = UpdateGetOldValue(observedValue);
			}

			public double UpdateGetOldValue(double observedValue)
			{
				double oldValue = this.Value;
				double delta = observedValue - this.Value;
				double weightedDelta;
				if (multiplay)
					weightedDelta = (this.LearningRate / HPlayerModel.CURRENT_NUM_ACTORS) * delta;
				else
					weightedDelta = this.LearningRate * delta;
				this.Value =+ weightedDelta;
				return oldValue;
			}

			/*public static bool operator == (Trait t1, Trait t2)
			{
				if ((t1
				return t1.ID == t2.ID;
			}

			public static bool operator != (Trait t1, Trait t2)
			{
				return t1.ID != t2.ID;
			}*/

			public static Trait TryGetTrait(string name)
			{
				foreach (Trait t in traits)
					if (t.Name == name) return t;
				return null;
			}

			public static Trait TryGetTrait(uint id)
			{
				foreach (Trait t in traits)
					if (t.ID == id) return t;
				return null;
			}

			public static bool TryUpdateTrait(string name, bool actionWasSuccessfull)
			{
				Trait t = TryGetTrait(name);
				if (t == null) return false;
				if (actionWasSuccessfull)
					t.Update(1.0d);
				else
					t.Update(0.0d);
				return true;
			}

			/*public static double TryGetTraitValue(uint id)
			{
				Trait t = TryGetTrait(id);
				return t.Value;
			}*/

			// Update all traits so they go back to around 0.5f after a certain period of time.
			public static void NormalizeAll()
			{
				foreach(Trait t in traits)
				{
					if (t.Value > 0.51d)
						if (t.multiplay)
							t.Value -= 0.01d * (t.LearningRate / HPlayerModel.CURRENT_NUM_ACTORS);
						else
							t.Value -= 0.01d * t.LearningRate;
					else if (t.Value < 0.49d)
						if (t.multiplay)
							t.Value += 0.01d * (t.LearningRate / HPlayerModel.CURRENT_NUM_ACTORS);
						else
							t.Value += 0.01d * t.LearningRate;
				}
			}
		}

		public class HModelNode
		{
			public static List<HModelNode> collection = new List<HModelNode>();

			public List<HModelNode> children { get; private set; }
			private Trait trait;
			private HModelNode parent;

			public HModelNode(Trait trait, HModelNode parent)
			{
				this.trait = trait;
				this.parent = parent;

				HModelNode.collection.Add(this);
			}

			public void UpdateTrait(double observedValue)
			{
				double oldValue = trait.UpdateGetOldValue(observedValue);
				if (parent != null)
					parent.Propagate(oldValue, trait.Value);
			}

			public bool HasTrait(Trait trait)
			{
				return this.trait == trait;
			}

			public double GetTraitValueAsDouble()
			{
				return trait.Value;
			}

			public uint GetTraitValueAsUInt()
			{
				return (uint)(trait.Value * 100d);
			}

			private void Propagate(double oldValue, double newValue)
			{
				int numChilds = children.Count;
				double tmpVal = trait.Value * numChilds - oldValue;
				double newTraitValue = (tmpVal + newValue) / numChilds;
				if (parent != null)
					parent.Propagate(trait.Value, newTraitValue);
				trait.Value = newTraitValue;
			}
		}
	}
}