using System;
using System.Collections.Generic;
using System.Linq;
using C7Engine.Pathing;
using C7GameData;
using C7GameData.AIData;

namespace C7Engine
{
	public class PlayerAI
	{
		public static void PlayTurn(Player player, Random rng)
		{
			if (player.isHuman || player.isBarbarians) {
				return;
			}
			//Do things with units.  Copy into an array first to avoid collection-was-modified exception
			foreach (MapUnit unit in player.units.ToArray())
			{
				if (unit.currentAIBehavior == null) {
					SetAIForUnit(unit, player);
				}
				
				//Now actually take actions
				//TODO: Move these into an AI method
				if (unit.currentAIBehavior is SettlerAI settlerAi) {
					if (settlerAi.goal == SettlerAI.SettlerGoal.JOIN_CITY && unit.location.cityAtTile != null) {
						//TODO: Actually join the city.  Haven't added that action.
						//For now, just get rid of the unit.  Sorry, bro.
						UnitInteractions.disbandUnit(unit.guid);
					}
					else if (unit.location == settlerAi.destination) {
						Console.WriteLine("Building city with " + unit);
						unit.buildCity(unit.owner.GetNextCityName());
					}
					else {
						if (settlerAi.pathToDestination == null) {
							PathingAlgorithm algorithm = PathingAlgorithmChooser.GetAlgorithm();
							settlerAi.pathToDestination = algorithm.PathFrom(unit.location, settlerAi.destination);
						}
						Tile nextTile = settlerAi.pathToDestination.Next();
						Console.WriteLine("Settler unit moving from " + unit.location + " to " + nextTile + " towards " + settlerAi.destination);
						unit.move(unit.location.directionTo(nextTile));
					}
				}
				else if (unit.currentAIBehavior is DefenderAI defenderAI) {
					if (defenderAI.destination == unit.location) {
						if (!unit.isFortified) {
							unit.fortify();
							Console.WriteLine("Fortifying " + unit + " at " + defenderAI.destination);
						}
					}
					else {
						//TODO: Move towards destination
						Console.WriteLine("Moving defender towards " + defenderAI.destination);
					}
				}
				else if (unit.currentAIBehavior is ExplorerAI explorerAi) {
					// Console.Write("Moving explorer AI for " + unit);
					//TODO: Distinguish between types of exploration
					//TODO: Make sure ON_A_BOAT units stay on the boat
					//Move randomly
					List<Tile> possibleNewLocations = unit.unitType is SeaUnit ? unit.location.GetCoastNeighbors() : unit.location.GetLandNeighbors();
					Tile newLocation = possibleNewLocations[rng.Next(possibleNewLocations.Count)];
					//Because it chooses a semi-cardinal direction at random, not accounting for map, it could get none
					//if it tries to move e.g. north from the north pole.  Hence, this check.
					if (newLocation != Tile.NONE) {
						// Console.WriteLine("Moving unit at " + unit.location + " to " + newLocation);
						unit.move(unit.location.directionTo(newLocation));
					}
				}
			}
		}

		private static void SetAIForUnit(MapUnit unit, Player player) 
		{
			//figure out an AI behavior
			//TODO: Use strategies, not names
			if (unit.unitType.name == "Settler") {
				SettlerAI settlerAI = new SettlerAI();
				settlerAI.goal = SettlerAI.SettlerGoal.BUILD_CITY;
				//If it's the starting settler, have it settle in place.  Otherwise, use an AI to find a location.
				if (unit.location.cityAtTile == null) {
					settlerAI.destination = unit.location;
				}
				else {
					settlerAI.destination = SettlerLocationAI.findSettlerLocation(unit.location, player.cities, player.units);
					if (settlerAI.destination == Tile.NONE) {
						//This is possible if all tiles within 4 tiles of a city are either not land, or already claimed
						//by another colonist.  Longer-term, the AI shouldn't be building settlers if that is the case,
						//but right now we'll just spike the football to stop the clock and avoid building immediately next to another city.
						settlerAI.goal = SettlerAI.SettlerGoal.JOIN_CITY;
					}
				}
				Console.WriteLine("Set AI for unit to settler AI with destination of " + settlerAI.destination);
				unit.currentAIBehavior = settlerAI;
			}
			else if (unit.location.cityAtTile != null && unit.location.unitsOnTile.Count(u => u.unitType.defense > 0 && u != unit) == 0) {
				DefenderAI ai = new DefenderAI();
				ai.goal = DefenderAI.DefenderGoal.DEFEND_CITY;
				ai.destination = unit.location;
				Console.WriteLine("Set defender AI for " + unit + " with destination of " + ai.destination);
				unit.currentAIBehavior = ai;
			}
			else {
				ExplorerAI ai = new ExplorerAI();
				if (unit.unitType is SeaUnit) {
					ai.type = ExplorerAI.ExplorationType.COASTLINE;
					Console.WriteLine("Set coastline exploration AI for " + unit);
				}
				else if (unit.location.unitsOnTile.Exists((x) => x.unitType is SeaUnit)) {
					ai.type = ExplorerAI.ExplorationType.ON_A_BOAT;
					//TODO: Actually put the unit on the boat
					Console.WriteLine("Set ON_A_BOAT exploration AI for " + unit);
				}
				else {
					ai.type = ExplorerAI.ExplorationType.RANDOM;
					Console.WriteLine("Set random exploration AI for " + unit);
				}
				unit.currentAIBehavior = ai;
			}
		}
	}
}
