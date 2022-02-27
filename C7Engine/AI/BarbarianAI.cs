using System.Collections.Generic;
using System.IO;

namespace C7Engine {
    using C7GameData;
    using System;

    public class BarbarianAI {
        public void PlayTurn(Player player, GameData gameData) {
            if (!player.isBarbarians) {
                throw new System.Exception("Barbarian AI can only play barbarian players");
            }

            foreach(MapUnit unit in gameData.mapUnits) {
	            //TODO: Make it better fit the barbs and not be hard-coded to a magic number
                if (unit.owner == gameData.players[0]) {
                    if (unit.location.unitsOnTile.Count > 1 || unit.location.hasBarbarianCamp == false) {
                        //Move randomly
                        List<Tile> validTiles = unit.unitType is SeaUnit ? unit.location.GetCoastNeighbors() : unit.location.GetLandNeighbors();
                        if (validTiles.Count == 0) {
	                        //This can happen if a barbarian galley spawns next to a 1-tile lake, moves there, and doesn't have anywhere else to go.
	                        Console.WriteLine("WARNING: No valid tiles for barbarian to move to");
	                        continue;
                        }
                        Tile newLocation = validTiles[gameData.rng.Next(validTiles.Count)];
                        //Because it chooses a semi-cardinal direction at random, not accounting for map, it could get none
                        //if it tries to move e.g. north from the north pole.  Hence, this check.
                        if (newLocation != Tile.NONE) {
                            //Console.WriteLine("Moving barbarian at " + unit.location + " to " + newLocation);
                            unit.location.unitsOnTile.Remove(unit);
                            newLocation.unitsOnTile.Add(unit);
                            unit.location = newLocation;
                        }
                    }
                }
            }
        }
    }
}