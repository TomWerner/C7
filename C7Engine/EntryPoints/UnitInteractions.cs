namespace C7Engine
{
    using C7GameData;
    using System;
    using System.Collections.Generic;

    public class UnitInteractions
    {

        private static Queue<MapUnit> waitQueue = new Queue<MapUnit>();

        public static MapUnit getNextSelectedUnit()
        {
            GameData gameData = EngineStorage.gameData;
            foreach (MapUnit unit in gameData.mapUnits)
            {
                //Eventually we'll have to check ownership,
                //but we haven't added the concepts of players or civilizations yet.
                if (unit.movementPointsRemaining > 0 && !unit.isFortified)
                {
                    if (!waitQueue.Contains(unit)) {
                        return unit;
                    }
                }
            }
            if (waitQueue.Count > 0) {
                return waitQueue.Dequeue();
            }
            return MapUnit.NONE;
        }

        public static void fortifyUnit(string guid)
        {
            GameData gameData = EngineStorage.gameData;
            //This is inefficient, perhaps we'll have a map someday.  But with three units,
            //we'll survive for now.
            Console.WriteLine("Trying to fortify unit " + guid);
            foreach (MapUnit unit in gameData.mapUnits)
            {
                if (unit.guid == guid)
                {
                    Console.WriteLine("Set unit " + guid + " of type " + unit.GetType().Name + " to fortified");
                    unit.isFortified = true;
                    return;
                }
            }
            Console.WriteLine("Failed to find unit " + guid);
        }

        /**
         * Super dumb movement where you can only move to one tile, and back to the first one.
         * We'll build out more movement later.
         **/
        public static void moveUnit(string guid)
        {
            GameData gameData = EngineStorage.gameData;
            //This is inefficient, perhaps we'll have a map someday.  But with three units,
            //we'll survive for now.
            foreach (MapUnit unit in gameData.mapUnits)
            {
                if (unit.guid == guid)
                {
                    if (unit.location == gameData.map.tiles[168])
                    {
                        unit.location = gameData.map.tiles[169];
                    }
                    else
                    {
                        unit.location = gameData.map.tiles[168];
                    }
                }
            }
        }

        /**
         * I'd like to enhance this so it's like Civ4, where the hold action takes
         * the unit out of the rotation, but you can change your mind if need be.
         * But for now it'll be like Civ3, where you're out of luck if you realize
         * that unit was needed for something; that also simplifies things here.
         **/
        public static void holdUnit(string guid)
        {
            GameData gameData = EngineStorage.gameData;
            //This is inefficient, perhaps we'll have a map someday.  But with three units,
            //we'll survive for now.
            foreach (MapUnit unit in gameData.mapUnits)
            {
                if (unit.guid == guid)
                {
                    Console.WriteLine("Found matching unit with guid " + guid + " of type " + unit.GetType().Name + "; settings its movement to zero");
                    unit.movementPointsRemaining = 0;
                    return;
                }
            }
            Console.WriteLine("Failed to find a matching unit with guid " + guid);
        }

        public static void waitUnit(string guid)
        {
            GameData gameData = EngineStorage.gameData;
            foreach (MapUnit unit in gameData.mapUnits)
            {
                if (unit.guid == guid)
                {
                    Console.WriteLine("Found matching unit with guid " + guid + " of type " + unit.GetType().Name + "; adding it to the wait queue");
                    waitQueue.Enqueue(unit);
                }
            }
            Console.WriteLine("Failed to find a matching unit with guid " + guid);
        }

        public static void ClearWaitQueue()
        {
            waitQueue.Clear();
        }
    }
}