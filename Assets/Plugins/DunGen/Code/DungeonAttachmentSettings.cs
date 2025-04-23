using UnityEngine;
using UnityEngine.Assertions;

namespace DunGen
{
	public class DungeonAttachmentSettings
	{
		/// <summary>
		/// The doorway to attach the dungeon to. If set, the new dungeon must be attached to this doorway
		/// </summary>
		public Doorway AttachmentDoorway { get; private set; }

		/// <summary>
		/// The tile to attach the dungeon to. If set, the new dungeon will attach to this tile, but the doorway
		/// will be chosen randomly
		/// </summary>
		public Tile AttachmentTile { get; private set; }

		public TileProxy TileProxy { get; private set; }


		public DungeonAttachmentSettings(Doorway attachmentDoorway)
		{
			Assert.IsNotNull(attachmentDoorway, "attachmentDoorway cannot be null");
			AttachmentDoorway = attachmentDoorway;

			if (AttachmentDoorway.Tile.UsedDoorways.Contains(AttachmentDoorway))
				Debug.LogError($"Cannot attach dungeon to doorway '{attachmentDoorway.name}' as it is already in use");
		}

		public DungeonAttachmentSettings(Tile attachmentTile)
		{
			Assert.IsNotNull(attachmentTile, "attachmentTile cannot be null");
			AttachmentTile = attachmentTile;
		}

		public TileProxy GenerateAttachmentProxy(bool ignoreSpriteRendererBounds, Vector3 upVector, RandomStream randomStream)
		{
			if (AttachmentTile != null)
			{
				// This tile wasn't placed by DunGen so we'll need to do
				// some extra setup to ensure we have all the data we'll need later
				if (AttachmentTile.Prefab == null)
					PrepareManuallyPlacedTile(ignoreSpriteRendererBounds, upVector, randomStream);

				TileProxy = new TileProxy(AttachmentTile.Prefab,
					ignoreSpriteRendererBounds,
					upVector,
					(doorway, index) => AttachmentTile.UnusedDoorways.Contains(AttachmentTile.AllDoorways[index])); // Ensure chosen doorway is unused

				TileProxy.Placement.Position = AttachmentTile.transform.localPosition;
				TileProxy.Placement.Rotation = AttachmentTile.transform.localRotation;
			}
			else if (AttachmentDoorway != null)
			{
				var attachmentTile = AttachmentDoorway.Tile;

				TileProxy = new TileProxy(AttachmentDoorway.Tile.Prefab,
					ignoreSpriteRendererBounds,
					upVector,
					(doorway, index) => index == attachmentTile.AllDoorways.IndexOf(AttachmentDoorway));

				TileProxy.Placement.Position = AttachmentDoorway.Tile.transform.localPosition;
				TileProxy.Placement.Rotation = AttachmentDoorway.Tile.transform.localRotation;
			}

			return TileProxy;
		}

		private void PrepareManuallyPlacedTile(bool ignoreSpriteRendererBounds, Vector3 upVector, RandomStream randomStream)
		{
			AttachmentTile.Prefab = AttachmentTile.gameObject;

			foreach (var doorway in AttachmentTile.GetComponentsInChildren<Doorway>())
			{
				doorway.Tile = AttachmentTile;

				AttachmentTile.AllDoorways.Add(doorway);
				AttachmentTile.UnusedDoorways.Add(doorway);

				doorway.ProcessDoorwayObjects(false, randomStream);
			}

			Bounds bounds;

			if (AttachmentTile.OverrideAutomaticTileBounds)
				bounds = AttachmentTile.TileBoundsOverride;
			else
				bounds = UnityUtil.CalculateProxyBounds(AttachmentTile.gameObject, ignoreSpriteRendererBounds, upVector);

			AttachmentTile.Placement.LocalBounds = UnityUtil.CondenseBounds(bounds, AttachmentTile.AllDoorways);
		}

		public Tile GetAttachmentTile()
		{
			Tile attachmentTile = null;

			if (AttachmentTile != null)
				attachmentTile = AttachmentTile;
			else if (AttachmentDoorway != null)
				attachmentTile = AttachmentDoorway.Tile;

			return attachmentTile;
		}
	}
}