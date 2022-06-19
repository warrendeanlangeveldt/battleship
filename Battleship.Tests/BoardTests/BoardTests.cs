using Battleship.Logic.Components.BoardComponent;
using Battleship.Logic.Components.ShipComponent;
using Battleship.Logic.Game.Helper;
using NUnit.Framework;

namespace Battleship.Tests.BoardTests
{
    [TestFixture]
    public class BoardTests
    {
        private Board board;

        public BoardTests()
        {
            board = new Board();
        }

        [TestCase(10,10),Order(1)]
        public void Can_Generate_a_board_based_on_width_and_height(int width,int height)
        {
            //act
            board.GenerateBoard(width, height);

            //assert
            Assert.That(board.BoardPositions.Count, Is.EqualTo(width * height));
        }

        [Test,Order(2)]
        public void Can_deploy_ship_to_board_on_the_boundary()
        {
            //arrange
            var ship = new Warship(3, "Cruiser", "c3");
            var direction = Direction.HORIZONTAL;
            var coordinate = new Coordinate(9, 9);

            //act
            var result = board.DeployShip(ship, direction, coordinate);

            //assert
            Assert.That(result, Is.EqualTo(true));

            var positions = board.BoardPositions.Where(x => x.ShipId == ship.ShipId).ToList();
            Assert.That(positions.Count, Is.EqualTo(3));

            //The Algorithm attmpts to reverse the position if it will overflow in the positive direction of the Direction 
           
            //direction  = horizontal
            var x = positions[0].Coordinate.PositionX;
            Assert.That(x, Is.EqualTo(9 - 3));

            x = positions[1].Coordinate.PositionX;
            Assert.That(x, Is.EqualTo(9 - 2));

            x = positions[2].Coordinate.PositionX;
            Assert.That(x, Is.EqualTo(9 - 1));


        }

        [Test,Order(3)]
        public void Can_deploy_ship_to_board_with_overlapping_coordinates()
        {
            //arrange
            var ship = new Warship(5, "Battleship", "c5");
            var direction = Direction.VERTICAL;

            //these coordinates should overlap with the previous placement
            var coordinate = new Coordinate(4, 4);

            //act
            var result = board.DeployShip(ship, direction, coordinate);

            //assert
            Assert.That(result, Is.EqualTo(true));
            Assert.That(board.BoardPositions.Where(x => x.ShipId == ship.ShipId).Count, Is.EqualTo(5));

            //note that the algorithm gets a new random coordinate if an overlap is detetected

        }
    }
}
