using System.Collections.Generic;
using C7Engine;
using C7Engine.AI.StrategicAI;

namespace C7GameData.AIData {
	public class ExpansionPriority : StrategicPriority {
		private readonly int TEMP_GAME_LENGTH = 540;
		private readonly int EARLY_GAME_CUTOFF = 25;	//what percentage of the game is early game, which should give expansion a boost?

		public ExpansionPriority() {
			key = "Expansion";
			this.data.priorityKey = "Expansion";
		}

		public override void CalculateWeightAndMetadata(Player player) {
			if (player.cities.Count < 2) {
				this.calculatedWeight = 1000;
			} else {
				int score = CalculateAvailableLandScore(player);
				score = ApplyEarlyGameMultiplier(score);
				score = ApplyNationTraitMultiplier(score, player);

				this.calculatedWeight = score;
			}
		}
		private static int CalculateAvailableLandScore(Player player)
		{
			//Figure out if there's land to settle, and how much
			Dictionary<Tile, int> possibleLocations = SettlerLocationAI.GetPossibleNewCityLocations(player.cities[0].location, player);
			int score = possibleLocations.Count * 10;
			foreach (int i in possibleLocations.Values) {
				score += i / 10;
			}
			return score;
		}
		private int ApplyEarlyGameMultiplier(int score)
		{
			//If it's early game, multiply this score.
			//TODO: We haven't implemented the part for "how many turns does the game have?" yet.  So this is hard-coded.
			int gameTurn = EngineStorage.gameData.turn;
			int percentOfGameFinished = (gameTurn * 100) / TEMP_GAME_LENGTH;
			if (percentOfGameFinished < 25) {
				score = score * (25 - percentOfGameFinished) / 5;
			}
			return score;
		}

		private int ApplyNationTraitMultiplier(int score, Player player) {
			// TODO: The "Expansionist" trait should give a higher priority to this strategic priority.
			return score;
		}
	}
}
