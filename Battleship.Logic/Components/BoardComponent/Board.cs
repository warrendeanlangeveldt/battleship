using Battleship.Logic.Components.PlayerComponent;
using Battleship.Logic.Components.ShipComponent;
using Battleship.Logic.Game.Helper;

namespace Battleship.Logic.Components.BoardComponent
{
    /// <summary>
    /// 
    /// </summary>
    public class Board
    {

        /// <summary>
        /// A booard is made of a list of board positions
        /// Decided to go with a flat structure instead of nested arrays
        /// </summary>
        public List<BoardPosition> BoardPositions { get; set; }

        public int BOARDSIZEX { get; set; }
        public int BOARDSIZEY { get; set; }

        public Board()
        {
            BoardPositions = new List<BoardPosition>();
        }

        /// <summary>
        /// Creates the board positions based on the boundary criteria
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void GenerateBoard(int width, int height)
        {
            BOARDSIZEX = width;
            BOARDSIZEY = height;

            for(int x = 1; x <= width; x++)
            {
                for (int y = 1; y <= height; y++)
                {
                    BoardPositions.Add(new BoardPosition(new Coordinate(x,y)));
                }
            }
        }

        /// <summary>
        /// A ship gets deployed to the board.
        /// We check boundary placements and attempt to invert the position
        /// We also check for overlapping placements.
        /// if the criteria is not met, we try again with another random coordinate
        /// 
        /// NB. Im aware that im not testing to whether the amount of ships can actually fit on the board size. a bit complicated for the time spent on this.
        ///     An alternative is to provide an exit criteria for how many attempts is made.
        /// </summary>
        /// <param name="ship"> this ship thats being deployed</param>
        /// <param name="directiton">the direction the ship will be deployed</param>
        /// <param name="givenCoordinate"> if no given coordinate is provided, a random one will be generated</param>
        /// <returns></returns>
        public bool DeployShip(Warship ship, Direction directiton,Coordinate? givenCoordinate = null)
        {
            var rnd = new Random();

            //get random coordinate
            var coordinate = givenCoordinate == null ? new Coordinate(rnd.Next(1, BOARDSIZEX), rnd.Next(1, BOARDSIZEY)): givenCoordinate;

            // we check along the axis based on the ship length if the ship will be placed out of bounds
            //and try and update it with a new coordinate in the opposite direcation;
            coordinate = CheckBoundaryPlacement(ship, coordinate, directiton);

            var boardPosition = BoardPositions.Single(x => x.Coordinate.PositionX == coordinate.PositionX && x.Coordinate.PositionY == coordinate.PositionY);

            //we got a position on the board that is not taken
            if (!boardPosition.IsOccupied)
            {
                //now check overlap placeent
                CheckOverlapPlacement(ship, coordinate, directiton);
            }
            else
            {
                DeployShip(ship, directiton);
            }


            return true;

        }

        /// <summary>
        /// If the ship is placed on the boundary and will overlap, we attempt a placement in the negative directtion on the axis by calculating a new starting coordinate
        /// </summary>
        /// <param name="ship"></param>
        /// <param name="coordinate"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        private Coordinate CheckBoundaryPlacement(Warship ship,Coordinate coordinate,Direction direction)
        {
            //if the placement is going to go over the boundary, 
            if(direction == Direction.HORIZONTAL && ((ship.ShipLength+ coordinate.PositionX) > BOARDSIZEX)){

                //will attempt to do a placement on the boundary but in the negative direction on the axis
                var negativeStartingCoordinate = BoardPositions.Single(x => x.Coordinate.PositionY == coordinate.PositionY && x.Coordinate.PositionX == coordinate.PositionX - ship.ShipLength).Coordinate;

                return negativeStartingCoordinate;
            }
            else if(direction == Direction.VERTICAL && ((ship.ShipLength + coordinate.PositionY) > BOARDSIZEY)){

                var negativeStartingCoordinate = BoardPositions.Single(x => x.Coordinate.PositionX == coordinate.PositionX && x.Coordinate.PositionY == (coordinate.PositionY - ship.ShipLength)).Coordinate;

                return negativeStartingCoordinate;
            }

            return coordinate;
        }

        /// <summary>
        /// If an overlap onto another ship is likely, we attempt a placement in the opposite direction on the axis.
        /// If both not possible, we randomise a new starting coordinate
        /// If we do get a valid placement, we update the ship sections to match the coordinates of the board position
        /// </summary>
        /// <param name="ship"></param>
        /// <param name="coordinate"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        private bool CheckOverlapPlacement(Warship ship,Coordinate coordinate, Direction direction)
        {


            if(direction == Direction.HORIZONTAL)
            {
                //get any other board positions on the horizontal axis based on the ships length and check if they are occupied in the positive direction
                var shipPlacement = BoardPositions.Where(x =>
                    x.Coordinate.PositionY == coordinate.PositionY &&
                    (x.Coordinate.PositionX >= coordinate.PositionX && x.Coordinate.PositionX < (coordinate.PositionX + ship.ShipLength)));

                if (shipPlacement.Any(x => x.IsOccupied))
                {

                    //try and get in the opposite direction
                    shipPlacement = BoardPositions.Where(x =>
                    x.Coordinate.PositionY == coordinate.PositionY &&
                    (x.Coordinate.PositionX > coordinate.PositionX && x.Coordinate.PositionX >= (coordinate.PositionX - ship.ShipLength)));
                }

                //a bit untidy i know, but we check again in th opposite direction
                if (shipPlacement.Any(x => x.IsOccupied))
                {
                    //try again :(
                    DeployShip(ship, direction);
                }
                else
                {
                    //we finally get a valid placement. Lets update the board and set the board positions to occupied and tag the position with a ship id
                    //will need the ship id later to determine whether ships are damaged and destroyed

                    //Tells the board where the ship is based on the ship id
                    //makes the board position occupied
                    foreach (BoardPosition position in shipPlacement)
                    {
                        position.IsOccupied = true;
                        position.ShipId = ship.ShipId;
                    }

                    //this gives each section of the ship a reference point to the board position
                    //will help when determine missile strike outcomes
                    ship.UpdateShipPlacement(shipPlacement.Select(x => x.Coordinate).ToList());

                }
            }
            else
            {
                //get any other board positions on the vertical axis based on the ships length and check if they are occupied in the positive direction
                var shipPlacement = BoardPositions.Where(x =>
                    x.Coordinate.PositionX == coordinate.PositionX &&
                    (x.Coordinate.PositionY >= coordinate.PositionY && x.Coordinate.PositionY <= (coordinate.PositionY + ship.ShipLength-1)));

                if (shipPlacement.Any(x => x.IsOccupied))
                {

                    //try and get in the opposite direction
                    shipPlacement = BoardPositions.Where(x =>
                    x.Coordinate.PositionX == coordinate.PositionX &&
                    (x.Coordinate.PositionY >= coordinate.PositionY && x.Coordinate.PositionY >= (coordinate.PositionY - ship.ShipLength-1)));
                }

                //a bit untidy i know, but we check again in th opposite direction
                if (shipPlacement.Any(x => x.IsOccupied))
                {
                    //try again :(
                    DeployShip(ship, direction);
                }
                else
                {
                    //we finally get a valid placement. Lets update the board and set the board positions to occupied and tag the position with a ship id
                    //will need the ship id later to determine whether ships are damaged and destroyed

                    //Tells the board where the ship is based on the ship id
                    //makes the board position occupied
                    foreach (BoardPosition position in shipPlacement)
                    {
                        position.IsOccupied = true;
                        position.ShipId = ship.ShipId;
                    }

                    //this gives each section of the ship a reference point to the board position
                    //will help when determine missile strike outcomes
                    ship.UpdateShipPlacement(shipPlacement.Select(x => x.Coordinate).ToList());

                }
            }


            return true;
        }

    }


    public class BoardPosition
    {
        public bool IsOccupied { get; set; }

        public Coordinate Coordinate { get; set; }
        public string ShipId { get; set; }

        public BoardPosition(Coordinate coordinate)
        {
            Coordinate = coordinate;

        }

    }


}
