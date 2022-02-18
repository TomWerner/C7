namespace C7Engine
{
    using C7GameData;

    public class CityInteractions
    {
        //Eventually, this will need more info such as player
        public static void BuildCity(int x, int y, string playerGuid, string name)
        {
            GameData gameData = EngineStorage.gameData;

            Player owner = gameData.players.Find(player => player.guid == playerGuid);

			Tile tileWithNewCity = MapInteractions.GetTileAt(x, y);
			City newCity = new City(tileWithNewCity, owner, name);
			newCity.SetItemBeingProduced(gameData.unitPrototypes["Warrior"]);
            gameData.cities.Add(newCity);
            owner.cities.Add(newCity);

            tileWithNewCity.cityAtTile = newCity;
        }
    }
}