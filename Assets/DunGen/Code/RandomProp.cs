using System.Collections.Generic;
using UnityEngine;

namespace DunGen
{
	public abstract class RandomProp : MonoBehaviour
	{
		public abstract void Process(RandomStream randomStream, Tile tile, ref List<GameObject> spawnedObjects);
	}
}
