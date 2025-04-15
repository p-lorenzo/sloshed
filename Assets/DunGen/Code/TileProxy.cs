using DunGen.Tags;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace DunGen
{
	public sealed class DoorwayProxy
	{
		public bool Used { get { return ConnectedDoorway != null; } }
		public TileProxy TileProxy { get; private set; }
		public int Index { get; private set; }
		public DoorwaySocket Socket { get; private set; }
		public Doorway DoorwayComponent { get; private set; }
		public Vector3 LocalPosition { get; private set; }
		public Quaternion LocalRotation { get; private set; }
		public DoorwayProxy ConnectedDoorway { get; private set; }
		public Vector3 Forward { get { return (TileProxy.Placement.Rotation * LocalRotation) * Vector3.forward; } }
		public Vector3 Up { get { return (TileProxy.Placement.Rotation * LocalRotation) * Vector3.up; } }
		public Vector3 Position { get { return TileProxy.Placement.Transform.MultiplyPoint(LocalPosition); } }
		public TagContainer Tags { get; private set; }
		public bool IsDisabled { get; internal set; }


		public DoorwayProxy(TileProxy tileProxy, DoorwayProxy other)
		{
			TileProxy = tileProxy;
			Index = other.Index;
			Socket = other.Socket;
			DoorwayComponent = other.DoorwayComponent;
			LocalPosition = other.LocalPosition;
			LocalRotation = other.LocalRotation;
			Tags = new TagContainer(other.Tags);
		}

		public DoorwayProxy(TileProxy tileProxy, int index, Doorway doorwayComponent, Vector3 localPosition, Quaternion localRotation)
		{
			TileProxy = tileProxy;
			Index = index;
			Socket = doorwayComponent.Socket;
			DoorwayComponent = doorwayComponent;
			LocalPosition = localPosition;
			LocalRotation = localRotation;
		}

		public static void Connect(DoorwayProxy a, DoorwayProxy b)
		{
			Debug.Assert(a.ConnectedDoorway == null, "Doorway 'a' is already connected to something");
			Debug.Assert(b.ConnectedDoorway == null, "Doorway 'b' is already connected to something");

			a.ConnectedDoorway = b;
			b.ConnectedDoorway = a;
		}

		public void Disconnect()
		{
			if (ConnectedDoorway == null)
				return;

			ConnectedDoorway.ConnectedDoorway = null;
			ConnectedDoorway = null;
		}
	}

	public sealed class TileProxy
	{
		public GameObject Prefab { get; private set; }
		public Tile PrefabTile { get; private set; }
		public TilePlacementData Placement { get; internal set; }
		public List<DoorwayProxy> Entrances { get; private set; }
		public List<DoorwayProxy> Exits { get; private set; }
		public ReadOnlyCollection<DoorwayProxy> Doorways { get; private set; }
		public IEnumerable<DoorwayProxy> UsedDoorways { get { return doorways.Where(d => d.Used); } }
		public IEnumerable<DoorwayProxy> UnusedDoorways { get { return doorways.Where(d => !d.Used); } }
		public TagContainer Tags { get; private set; }

		private List<DoorwayProxy> doorways = new List<DoorwayProxy>();


		public TileProxy(TileProxy existingTile)
		{
			Prefab = existingTile.Prefab;
			PrefabTile = existingTile.PrefabTile;
			Placement = new TilePlacementData(existingTile.Placement);
			Tags = new TagContainer(existingTile.Tags);

			// Copy proxy doorways
			Doorways = new ReadOnlyCollection<DoorwayProxy>(doorways);
			Entrances = new List<DoorwayProxy>();
			Exits = new List<DoorwayProxy>();

			foreach(var existingDoorway in existingTile.doorways)
			{
				var doorway = new DoorwayProxy(this, existingDoorway);
				doorways.Add(doorway);

				if (existingTile.Entrances.Contains(existingDoorway))
					Entrances.Add(doorway);

				if(existingTile.Exits.Contains(existingDoorway))
					Exits.Add(doorway);
			}
		}

		public TileProxy(GameObject prefab, bool ignoreSpriteRendererBounds, Vector3 upVector, Func<Doorway, int, bool> allowedDoorwayPredicate = null)
		{
			prefab.transform.localPosition = Vector3.zero;
			prefab.transform.localRotation = Quaternion.identity;

			Prefab = prefab;
			PrefabTile = prefab.GetComponent<Tile>();

			if (PrefabTile == null)
				PrefabTile = prefab.AddComponent<Tile>();

			Placement = new TilePlacementData();
			Tags = new TagContainer(PrefabTile.Tags);

			// Add proxy doorways
			Doorways = new ReadOnlyCollection<DoorwayProxy>(doorways);
			Entrances = new List<DoorwayProxy>();
			Exits = new List<DoorwayProxy>();

			var allDoorways = prefab.GetComponentsInChildren<Doorway>();
			for (int i = 0; i < allDoorways.Length; i++)
			{
				var doorway = allDoorways[i];

				Vector3 localPosition = doorway.transform.position;
				Quaternion localRotation = doorway.transform.rotation;

				var proxyDoorway = new DoorwayProxy(this, i, doorway, localPosition, localRotation);
				doorways.Add(proxyDoorway);

				if (PrefabTile.Entrances.Contains(doorway))
					Entrances.Add(proxyDoorway);
				if (PrefabTile.Exits.Contains(doorway))
					Exits.Add(proxyDoorway);

				if (allowedDoorwayPredicate != null && !allowedDoorwayPredicate(doorway, i))
					proxyDoorway.IsDisabled = true;
			}

			// Calculate bounds
			Bounds bounds;

			if (PrefabTile != null && PrefabTile.OverrideAutomaticTileBounds)
				bounds = PrefabTile.TileBoundsOverride;
			else
				bounds = UnityUtil.CalculateProxyBounds(Prefab, ignoreSpriteRendererBounds, upVector);

			// Let the user know if the automatically calculated bounds are incorrect
			if (bounds.size.x <= 0f || bounds.size.y <= 0f || bounds.size.z <= 0f)
				Debug.LogError(string.Format("Tile prefab '{0}' has automatic bounds that are zero or negative in size. The bounding volume for this tile will need to be manually defined.", prefab), prefab);

			Placement.LocalBounds = UnityUtil.CondenseBounds(bounds, Prefab.GetComponentsInChildren<Doorway>());
		}

		public void PositionBySocket(DoorwayProxy myDoorway, DoorwayProxy otherDoorway)
		{
			Quaternion targetRotation = Quaternion.LookRotation(-otherDoorway.Forward, otherDoorway.Up);
			Placement.Rotation = targetRotation * Quaternion.Inverse(Quaternion.Inverse(Placement.Rotation) * (Placement.Rotation * myDoorway.LocalRotation));

			Vector3 targetPosition = otherDoorway.Position;
			Placement.Position = targetPosition - (myDoorway.Position - Placement.Position);
		}

		public bool IsOverlapping(TileProxy other, float maxOverlap)
		{
			return UnityUtil.AreBoundsOverlapping(Placement.Bounds, other.Placement.Bounds, maxOverlap);
		}

		public bool IsOverlappingOrOverhanging(TileProxy other, AxisDirection upDirection, float maxOverlap)
		{
			return UnityUtil.AreBoundsOverlappingOrOverhanging(Placement.Bounds, other.Placement.Bounds, upDirection, maxOverlap);
		}
	}
}
