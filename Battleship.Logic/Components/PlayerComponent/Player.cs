using Battleship.Logic.Components.BoardComponent;
using Battleship.Logic.Game.Helper;

namespace Battleship.Logic.Components.PlayerComponent
{
    public class Player
    {
        /// <summary>
        /// List of ships in the player's fleer
        /// </summary>
        public List<ShipComponent.Warship> PlayerShips { get; private set; }

        /// <summary>
        /// The game board where the player's fleet is deployed to
        /// </summary>
        public Board PlayerBoard { get; set; }


        /// <summary>
        /// A collection of all the moves the player has made in the game.
        /// Utilised to determine calculated strikes
        /// </summary>
        public List<StrikeReport> PlayerMoves { get; set; }

        /// <summary>
        /// The player keeps a record of the strikes that ws launched by the opponent
        /// </summary>
        public List<StrikeReport> OpponentMoves { get; set; }


        public Player()
        {
            PlayerMoves = new List<StrikeReport>();
            OpponentMoves = new List<StrikeReport>();
            PlayerShips = new List<ShipComponent.Warship>(); ;
        }

        /// <summary>
        /// Adds a board to the player
        /// </summary>
        /// <param name="board"></param>
        public void AddBoard(Board board)
        {
            PlayerBoard = board;
        }

        /// <summary>
        /// Adds a ship to the player's fleet
        /// </summary>
        /// <param name="ship"></param>
        public  void AddShip(ShipComponent.Warship ship)
        {
            PlayerShips.Add(ship);
        }


        /// <summary>
        /// The player can make a random missile launch
        /// The only constraints is that it is within the boundaries of the board and that its not a repeated strike
        /// </summary>
        /// <returns></returns>
        public MissileLaunch MakeRandomLaunch()
        {
            var rnd = new Random();
            var randomX = rnd.Next(1, PlayerBoard.BOARDSIZEX + 1);
            var randomY = rnd.Next(1, PlayerBoard.BOARDSIZEY + 1);

            var coord = new Coordinate(randomX, randomY);

            //Lets do another random launch where we've not launched before
            if(PlayerMoves.Select(x=> x.LaunchedMissile).Any(x=>x.Coordinate.PositionX == coord.PositionX && x.Coordinate.PositionY == coord.PositionY))
            {
                return MakeRandomLaunch();
            }

            var launch = new MissileLaunch(coord);
            return launch;
        }

        /// <summary>
        /// A calculated strike takes into account previous successful strikes and attempts to attack a neighbor board position of the successful strike.
        /// There can be multiple possibilities here, so we randomise the choice 
        /// </summary>
        /// <param name="xInput">if provided, the target location will be fired upon based on the coordinate</param>
        /// <param name="yInput">if provided, the target location will be fired upon based on the coordinate</param>
        /// <returns></returns>
        public MissileLaunch MakeCalculatedLaunch(int xInput = 0, int yInput = 0)
        {
            //just launch if x and y is more that 0 but less that the board constraint
            if(((xInput>0 && xInput <=PlayerBoard.BOARDSIZEX) && (yInput > 0 && yInput <= PlayerBoard.BOARDSIZEY))){
                if(!PlayerMoves.Any(x => x.LaunchedMissile.Coordinate.PositionX == xInput && x.LaunchedMissile.Coordinate.PositionY == yInput))
                {
                    return new MissileLaunch(new Coordinate(xInput, yInput));
                }
                else
                {
                    //you've already attempted this strike;
                    return null;
                }

            }

            //lets make calculated launches
            var previousSuccessStrikes = PlayerMoves.Where(x => x.IsHit);

            //only get ships that have not been destroyed
            var targetedShips = previousSuccessStrikes.GroupBy(x => x.ShipIdHit).Where(x => x.All(y => !y.IsShipDestroyed)).ToList();

            //this part gets tricky. find the axis to next target
            if (targetedShips.Any())
            {
                foreach(var group in targetedShips)
                {
                    //get the lowestX and highestX xCoordinate. if they are the same. we should target in the y axis and visa versa
                    var xCoords = group.Select(x => x.LaunchedMissile.Coordinate.PositionX);
                    var firstXcoord = xCoords.OrderBy(x=>x).First();

                    var yCoords = group.Select(x => x.LaunchedMissile.Coordinate.PositionY);
                    var firstYcoord = yCoords.OrderBy(x=>x).First();

                    var newCoordsOptions = new List<Coordinate>();

                    //nice little new gem in c# 6.0 I just discoverd
                    //if x is the same for all, it means the ship is on the x-axis
                    if (xCoords.Any() && Array.TrueForAll(xCoords.ToArray(), x => x == xCoords.First()))
                    {
                        //choose either lowestX Y-1 or highestX Y+1 AS next target, provided they not out of bounds and not already a targeted strike
                        var lowestY = firstYcoord;
                        var highestY = yCoords.OrderBy(x => x).Last();

                        if(lowestY-1 >= 1 && (!PlayerMoves.Any(x => x.LaunchedMissile.Coordinate.PositionX == firstYcoord && x.LaunchedMissile.Coordinate.PositionY == lowestY - 1)))
                        {
                           newCoordsOptions.Add(new Coordinate(firstXcoord, lowestY - 1));          
                        }

                        if(highestY+1 <= PlayerBoard.BOARDSIZEY && (!PlayerMoves.Any(x => x.LaunchedMissile.Coordinate.PositionX == firstXcoord && x.LaunchedMissile.Coordinate.PositionY == highestY + 1)))
                        {
                            newCoordsOptions.Add(new Coordinate(firstXcoord, highestY + 1));
                        }

                    }
                    //if y is the same for all, it means the ship is on the y-axis
                    if (yCoords.Any() && Array.TrueForAll(yCoords.ToArray(), y => y == yCoords.First())){

                        //choose either lowestX X-1 or highestX X+1 AS next target, provided they not out of bounds
                        var lowestX = xCoords.OrderBy(x => x).First();
                        var highestX = xCoords.OrderBy(x => x).Last();

                        if (lowestX - 1 >= 1 && (!PlayerMoves.Any(x=>x.LaunchedMissile.Coordinate.PositionX == lowestX-1 && x.LaunchedMissile.Coordinate.PositionY == firstYcoord)))
                        {
                            newCoordsOptions.Add(new Coordinate(lowestX - 1, firstYcoord));
                        }

                        if (highestX + 1 <= PlayerBoard.BOARDSIZEY && (!PlayerMoves.Any(x=> x.LaunchedMissile.Coordinate.PositionX == highestX +1 && x.LaunchedMissile.Coordinate.PositionY == firstYcoord)))
                        {
                            newCoordsOptions.Add(new Coordinate(highestX + 1, firstYcoord));
                        }
                       
                    }

                    if (newCoordsOptions.Any())
                    {
                        //now lets randomise the next shot
                        var rnd = new Random();
                        return new MissileLaunch(newCoordsOptions[rnd.Next(0, newCoordsOptions.Count)]);
                    }
                    else
                    {
                        return MakeRandomLaunch();
                    }


                }
            }

            return MakeRandomLaunch();
        }

        /// <summary>
        /// Once a strike is launched, the opponent sends back a strike report
        /// The player persists the report in the player moves collection
        /// </summary>
        /// <param name="report"></param>
        public void DamageReport(StrikeReport report)
        {
            PlayerMoves.Add(report);
        }

        /// <summary>
        /// Calculates the result of an opposition strke. 
        /// Determines a hit, a miss, a destroyed ship, and a winning game
        /// </summary>
        /// <param name="missile"></param>
        /// <returns></returns>
        public StrikeReport Incoming(MissileLaunch missile)
        {
            var strikeReport = new StrikeReport(missile);

            var boardposition = PlayerBoard.BoardPositions.Single(x => x.Coordinate.PositionX == missile.Coordinate.PositionX && x.Coordinate.PositionY == missile.Coordinate.PositionY);

            if (boardposition.IsOccupied)
            {
                var ship = PlayerShips.Single(x => x.ShipId == boardposition.ShipId);
                var section = ship.Sections.Single(x => x.Coordinate.PositionX == boardposition.Coordinate.PositionX && x.Coordinate.PositionY == boardposition.Coordinate.PositionY);
                section.isHit = true;

                strikeReport.IsHit = true;
                strikeReport.ShipIdHit = ship.ShipId;

                if (ship.Sections.All(x => x.isHit))
                {
                    //ship is down
                    strikeReport.IsShipDestroyed = true;
                }

                if (PlayerShips.SelectMany(x => x.Sections).All(x => x.isHit))
                {
                    strikeReport.warOver = true;
                }

            }

            //we keep track of the attacks from the opponent
            OpponentMoves.Add(strikeReport);

            return strikeReport;
        }


        /// <summary>
        /// sorry, bit of a shortcut. shouldnt be using console here. its presumptious of the the client in use.
        /// Should provide a data model for the client to do the printing.
        /// </summary>
        public void PrintToConsole()
        {
            var gridLabelX = new List<int>();
            for(var i=1;i<=PlayerBoard.BOARDSIZEX; i++)
            {
                gridLabelX.Add(i);
            }
            Console.WriteLine("    " + String.Join("   ", gridLabelX.Select(x => $"{x,2}")) + "      **       " + String.Join("   ", gridLabelX.Select(x => $"{x,2}")) + "  ");

            foreach (var xGroup in PlayerBoard.BoardPositions.OrderBy(x => x.Coordinate.PositionX).GroupBy(x => x.Coordinate.PositionX))
            {
                var xIndex = xGroup.First().Coordinate.PositionX;

                //show the board with the ships visible
                Console.Write(GridMap.Alphabet[xIndex-1] + " | " + String.Join(" | ", xGroup.Select(x => x.IsOccupied ? x.ShipId : "  ")) + " |");


                //show the board with only the attack reports
                var boardStrikes = new List<string>();
                foreach (var position in xGroup)
                {
                    var report = OpponentMoves.FirstOrDefault(x =>x.LaunchedMissile != null? x.LaunchedMissile.Coordinate.PositionY == position.Coordinate.PositionY && x.LaunchedMissile.Coordinate.PositionX == position.Coordinate.PositionX:false);
                    
                    boardStrikes.Add(report == null? "  ": ( report.IsHit? " X": " 0"));
                }

                //if we have a strike at a position and its a hit, show X. If it was miss show 0. if there was no report show 'water' lol
                Console.Write($"    **   {GridMap.Alphabet[xIndex-1]} | " + String.Join(" | ", boardStrikes) + " |");
                Console.WriteLine();

            }


        }

    }

    /// <summary>
    /// Used as player move, and the result of the attack
    /// </summary>
    public class StrikeReport
    {
        public MissileLaunch LaunchedMissile { get; set; }
        public bool IsHit { get; set; }

        public string ShipIdHit { get; set; }

        public string GridKey => GridMap.GetGridKey(LaunchedMissile.Coordinate);

        public bool IsShipDestroyed { get; set; }
        public bool warOver { get; set; }

        public StrikeReport(MissileLaunch missileLaunch)
        {
            LaunchedMissile = missileLaunch;
        }
    }

    /// <summary>
    /// This could be simplified as just a coordinate, but if required we can persist additional info later on if the need to extend
    /// </summary>
    public class MissileLaunch
    {

        public Coordinate Coordinate { get; set; }

        public MissileLaunch(Coordinate coordinate)
        {
            Coordinate = coordinate;
        }

    }


}
