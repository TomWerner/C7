namespace C7Engine.Pathing
{
	/**
	 * Returns a pathing algorithm to use.
	 * Eventually, this will depend on some map considerations.
	 * For now, just return the first one.
	 */
	public class PathingAlgorithmChooser
	{
		private static PathingAlgorithm landAlgorithm = new DijkstrasAlgorithm(new WalkerOnLand());
		private static PathingAlgorithm waterAlgorithm = new DijkstrasAlgorithm(new WalkerOnWater());

		public static PathingAlgorithm GetAlgorithm(bool isLandUnit)
		{
			return isLandUnit ? landAlgorithm : waterAlgorithm;
		}
	}
}
