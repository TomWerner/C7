using System.Collections.Generic;
using C7GameData;
using Godot;
using Serilog;

namespace C7.Map {
	public class CityLayer : LooseLayer {

		private ILogger log = LogManager.ForContext<CityLayer>();
		private Dictionary<City, CityScene> citySceneLookup = new Dictionary<City, CityScene>();
		private Dictionary<Tile, City> tileCityLookup = new();

		public CityLayer()
		{
		}

		public override void drawObject(LooseView looseView, GameData gameData, Tile tile, Vector2 tileCenter)
		{
			if (tile.cityAtTile is null) {
				tileCityLookup.TryGetValue(tile, out City maybeCity);

				// The tile doesn't have a city and we have no record of a city. We're done.
				if (maybeCity == null) {
					return;
				}

				// The tile doesn't have a city but we have record of a city. It must have
				// just been destroyed. Remove our tracking of it.
				citySceneLookup.Remove(maybeCity, out CityScene cityScene);
				tileCityLookup.Remove(tile);
				cityScene.Hide();
				return;
			}

			City city = tile.cityAtTile;
			if (!citySceneLookup.ContainsKey(city)) {
				CityScene cityScene = new CityScene(city, tile, new Vector2I((int)tileCenter.X, (int)tileCenter.Y));
				looseView.AddChild(cityScene);
				citySceneLookup[city] = cityScene;
				tileCityLookup[tile] = city;
			} else {
				CityScene scene = citySceneLookup[city];
				scene._Draw();
			}
		}
	}
}
