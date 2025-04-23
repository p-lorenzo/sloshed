namespace DunGen
{
	public enum TilePlacementResult
	{
		Success,

		// Retry Codes
		NoFromDoorway,
		NoTilesWithMatchingDoorway,
		NoValidTile,
		TemplateIsNull,
		NoMatchingDoorwayInTile,
		TileIsColliding,
		NewTileIsNull,
		OutOfBounds,
	}
}
