namespace C7Engine
{

using C7GameData;

public static class MapUnitExtensions {
	public static void fortify(this MapUnit unit)
	{
		unit.facingDirection = TileDirection.SOUTHEAST;
		unit.isFortified = true;
		new MsgStartAnimation(unit, MapUnit.AnimatedAction.FORTIFY, null).send();
	}


	public static void move(this MapUnit unit, TileDirection dir)
	{
		(int dx, int dy) = dir.toCoordDiff();
		var newLoc = EngineStorage.gameData.map.tileAt(dx + unit.location.xCoordinate, dy + unit.location.yCoordinate);
		if ((newLoc != null) && (unit.movementPointsRemaining > 0)) {
			if (!unit.location.unitsOnTile.Remove(unit))
				throw new System.Exception("Failed to remove unit from tile it's supposed to be on");
			newLoc.unitsOnTile.Add(unit);
			unit.location = newLoc;
			unit.facingDirection = dir;
			unit.movementPointsRemaining -= newLoc.overlayTerrainType.movementCost;
			unit.isFortified = false;
			new MsgStartAnimation(unit, MapUnit.AnimatedAction.RUN, null).send();
		}
	}

	public static void skipTurn(this MapUnit unit)
	{
		/**
		* I'd like to enhance this so it's like Civ4, where the skip turn action takes the unit out of the rotation, but you can change your
		* mind if need be.  But for now it'll be like Civ3, where you're out of luck if you realize that unit was needed for something; that
		* also simplifies things here.
		**/
		unit.movementPointsRemaining = 0;
	}
}

}
