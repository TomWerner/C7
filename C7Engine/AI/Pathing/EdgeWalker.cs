using System.Collections.Generic;
using C7GameData;

namespace C7Engine.Pathing {
	public abstract class EdgeWalker<TNode>
	{
		public abstract IEnumerable<Edge<TNode>> getEdges(TNode node);
	}

	public class WalkerOnLand: EdgeWalker<Tile> {
		public override IEnumerable<Edge<Tile>> getEdges(Tile node) {
			List<Edge<Tile>> result = new List<Edge<Tile>>();
			foreach (KeyValuePair<TileDirection, Tile> pair in node.neighbors) {
				TileDirection direction = pair.Key;
				Tile neighbor = pair.Value;
				if (neighbor.IsLand()) {
					float movementCost = MapUnitExtensions.getMovementCost(neighbor, direction, neighbor);
					result.Add(new Edge<Tile>(node, neighbor, movementCost));
				}
			}
			return result;
		}
	}

	public class WalkerOnWater: EdgeWalker<Tile> {
		public override IEnumerable<Edge<Tile>> getEdges(Tile node) {
			List<Edge<Tile>> result = new List<Edge<Tile>>();
			foreach (KeyValuePair<TileDirection, Tile> pair in node.neighbors) {
				TileDirection direction = pair.Key;
				Tile neighbor = pair.Value;

				// Allow navigating to water tiles or tiles with a city, to support
				// costal and canal cities.
				//
				// If the unit can't enter the city (for example an enemy city), we
				// will path right next to it and then refuse to actually enter it.
				if (neighbor.IsWater() || neighbor.HasCity) {
					float movementCost = MapUnitExtensions.getMovementCost(neighbor, direction, neighbor);
					result.Add(new Edge<Tile>(node, neighbor, movementCost));
				}
			}
			return result;
		}
	}
}
