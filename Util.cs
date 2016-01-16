using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Device22
{
	//http://stackoverflow.com/questions/2944299/in-c-how-to-declare-a-generic-dictionary-with-a-type-as-key-and-an-ienumerable
	public interface IBase	{ }
	
	static class Container
	{
		static class PerType<T> where T : IBase
		{
			public static IEnumerable<T> list;
		}
		
		public static IEnumerable<T> Get<T>() where T : IBase
		{
			return PerType<T>.list;
		}
		
		public static void Set<T>(IEnumerable<T> newlist) where T : IBase
		{
			PerType<T>.list = newlist;
		}
		
		public static IEnumerable<T> GetByExample<T>(T ignoredExample) where T : IBase
		{
			return Get<T>();
		}
	}

	public class Coroutiner
	{
		public static Coroutine StartCoroutine(IEnumerator iterationResult)
		{
			//Create GameObject with MonoBehaviour to handle task.
			GameObject routeneHandlerGo = new GameObject("Coroutiner");
			CoroutinerInstance routeneHandler 
				= routeneHandlerGo.AddComponent(typeof(CoroutinerInstance)) 
					as CoroutinerInstance;
			return routeneHandler.ProcessWork(iterationResult);
		}
		
	}
	
	public class CoroutinerInstance : MonoBehaviour
	{
		public Coroutine ProcessWork(IEnumerator iterationResult)
		{
			return StartCoroutine(DestroyWhenComplete(iterationResult));
		}
		
		public IEnumerator DestroyWhenComplete(IEnumerator iterationResult)
		{
			yield return StartCoroutine(iterationResult);
			Destroy(gameObject);
		}
		
	}

	public class Util
	{
		// This function will return true 50% of the time
		public static bool RandomBool()
		{
			return UnityEngine.Random.value > 0.5f;
		}

		public static bool IsCollectionType(Type type)
		{
			return (type.GetInterface("ICollection") != null);
		}

		public static bool IsEnumerableType(Type type)
		{
			return (type.GetInterface("IEnumerable") != null);
		}

		public static bool IsGenericCollection(object o)
		{
			return (o.GetType().IsGenericType && o is IEnumerable);
		}

		public static bool IsArray(object o)
		{
			return o is Array;
		}

		public static void SetLayerRecursively(GameObject go, int layerNumber)
		{
			foreach (Transform t in go.GetComponentsInChildren<Transform>(true))
				t.gameObject.layer = layerNumber;
		}

		public static Vector3 SuperSmoothLerp(Vector3 pastPosition, Vector3 pastTargetPosition, Vector3 targetPosition, float time, float speed)
		{	
			Vector3 f = pastPosition - pastTargetPosition + (targetPosition - pastTargetPosition) / (speed * time);
			return targetPosition - (targetPosition - pastTargetPosition) / (speed*time) + f * Mathf.Exp(-speed*time);
		}

		/*public static Quaternion SuperSmoothLerp(Quaternion pastRotation, Quaternion pastTargetRotation, Quaternion targetRotation, float time, float speed)
		{	
			Vector3 f = pastRotation - pastTargetRotation + (targetRotation - pastTargetRotation) / (speed * time);
			return targetRotation - (targetRotation - pastTargetRotation) / (speed*time) + f * Mathf.Exp(-speed*time);
		}*/

		public static Transform FindNearestTargetInSphere(Transform goTransform, float radius, int layer)
		{
			float minDist = Mathf.Infinity;
			Transform nearest = null;
			
			if (layer == -1) return null;
			
			Collider[] colliders = Physics.OverlapSphere (goTransform.position, radius, 1 << layer);
			
			foreach (Collider coll in colliders)
			{
				if (coll != goTransform.collider)
				{
					float dist = Vector3.Distance (goTransform.position, coll.transform.position);
					
					if (dist < minDist)
					{
						minDist = dist;
						nearest = coll.transform;
					}
				}
			}	
			return nearest;
		}

		public static Transform FindTransformWithContainsName(Transform parent, string name)
		{
			if (parent.name.Contains(name)) return parent;
			foreach (Transform child in parent)
			{
				Transform result = FindTransformWithContainsName(child, name);
				if (result != null) return result;
			}
			return null;
		}

        public static Bounds GetColliderBounds(GameObject go)
        {
            Bounds totalBounds = go.GetComponentInChildren<Collider>().bounds;
            Collider[] extends = go.GetComponentsInChildren<Collider>();
            foreach (Collider col in extends) totalBounds.Encapsulate(col.bounds);
            return totalBounds;
        }

        public static Bounds GetMeshFilterBounds(GameObject go)
        {
            Bounds totalBounds = go.GetComponentInChildren<MeshFilter>().mesh.bounds;
            MeshFilter[] extends = go.GetComponentsInChildren<MeshFilter>();
            foreach (MeshFilter mf in extends) totalBounds.Encapsulate(mf.mesh.bounds);
            return totalBounds;
        }
	}

	public class Lua
	{
		private static LuaInterface.Lua lua = new LuaInterface.Lua();

		public static void RegisterLuaMethod(string path, object target, System.Reflection.MethodInfo function)
		{
			lua.RegisterFunction(path, target, function);
			Debug.Log (string.Format("LUA: Method {0} was registered.", function.Name));
		}

		public static void CallLuaFunction(string path, object target)
		{
			LuaInterface.LuaFunction tmp = lua.GetFunction(path);
			tmp.Call(target);
		}

		public static void CompileScript(string fileurl)
		{
			lua.DoFile(fileurl);
		}
	}
}