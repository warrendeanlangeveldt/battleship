using Battleship.Logic.Components.BoardComponent;
using Battleship.Logic.Components.PlayerComponent;
using Battleship.Logic.Components.ShipComponent;
using NUnit.Framework;

namespace Battleship.Tests.PlayerTests
{
    [TestFixture]
    public class PlayerTests
    {
        private Player player;

        public PlayerTests()
        {
            player = new Player();
        }

        [Test, Order(1)]
        public void Can_Add_a_board_to_player()
        {
            //arrange
            var board = new Board();
            board.GenerateBoard(10, 10);

            //act
            player.AddBoard(board);

            //assert
            Assert.That(player.PlayerBoard, Is.Not.Null);

        }

        [Test, Order(2)]
        public void Can_Add_Ship_to_player()
        {
            //arrange
            var ship = new Warship(4, "Cruiser", "c4");

            //act
            player.AddShip(ship);
            player.PlayerBoard.DeployShip(ship, Logic.Game.Helper.Direction.VERTICAL);

            //assert
            Assert.That(player.PlayerShips.Contains(ship), Is.True);

        }

        [Test, Order(3)]
        public void Can_create_a_random_missile_launch()
        {
            //arrange
            var missile = player.MakeRandomLaunch();

            //assert
            Assert.That(missile.Coordinate.PositionX, Is.InRange(1, 10));
            Assert.That(missile.Coordinate.PositionY, Is.InRange(1, 10));

        }

        [Test, Order(4)]
        public void Can_create_a_calculated_missile_launch_with_valid_coordinates()
        {
            //arrange
            var missile = player.MakeCalculatedLaunch(1, 3);

            //assert
            Assert.That(missile, Is.Not.Null);
            Assert.That(missile.Coordinate.PositionX, Is.EqualTo(1));
            Assert.That(missile.Coordinate.PositionY, Is.EqualTo(3));
        }

        [Test, Order(5)]
        public void Can_create_a_calculated_missile_launch_with_invalid_coordinates()
        {
            //arrange
            var missile = new MissileLaunch(new Logic.Game.Helper.Coordinate(1, 3));

            //we need strike report where the coordinate was already attempted, its irrelevant if it was a successful hit though
            player.PlayerMoves.Add(new StrikeReport(missile) { IsHit = true, warOver = false });

            //using the same coordinates from previous test
            var newMissile = player.MakeCalculatedLaunch(1, 3);

            //assert
            Assert.That(newMissile, Is.Null); ;

        }

        [Test, Order(6)]
        public void Can_create_a_calculate_launch_without_coordinates()
        {
            //arrange
            var missile = new MissileLaunch(new Logic.Game.Helper.Coordinate(3, 3));

            //we need to have some successful strkes for this to work, else it would be a random launch
            player.PlayerMoves.Add(new StrikeReport(missile) { IsHit = true, warOver = false });

            var newMissile = player.MakeCalculatedLaunch();

            //assert
            Assert.That(newMissile.Coordinate.PositionX, Is.InRange(2, 4)); //3 or next to,note. if 3 it means  the vertical axis was randomised
            Assert.That(newMissile.Coordinate.PositionY, Is.InRange(2, 4)); //3 or next to,note. if 3 it means  the horizontal axis was randomised
        }

        [Test, Order(7)]
        public void Can_destroy_a_ship()
        {
            //arrange

            var ship = new Warship(3, "Frigate", "f3");
            player.AddShip(ship);
            player.PlayerBoard.DeployShip(ship, Logic.Game.Helper.Direction.HORIZONTAL, new Logic.Game.Helper.Coordinate(5, 5));

            var opponent = new Player();
            var opponentBoard = new Board();
            opponentBoard.GenerateBoard(10, 10);
            opponent.AddBoard(opponentBoard);

            //give the starting succeful strike
            opponent.PlayerMoves.Add(player.Incoming((new MissileLaunch(new Logic.Game.Helper.Coordinate(5, 5)))));

            var destroyed = false;
            var destroyedShip = "";
            //act
            do
            {
                var opponontReport = player.Incoming(opponent.MakeCalculatedLaunch());
                opponent.DamageReport(opponontReport);
                destroyed = opponontReport.IsShipDestroyed;
                destroyedShip = opponontReport.ShipIdHit;

            } while (destroyed == false);

            //Assert
            Assert.That(destroyed, Is.True);
            Assert.That(player.PlayerShips.Any(x=>x.Sections.All(y=>y.isHit)),Is.True);
        }

        [Test, Order(7)]
        public void Can_win_the_game()
        {
            //arrange

            var ship = new Warship(5, "Battleship", "s1");
            player.AddShip(ship);
            player.PlayerBoard.DeployShip(ship, Logic.Game.Helper.Direction.VERTICAL, new Logic.Game.Helper.Coordinate(7, 1));

            var opponent = new Player();
            var opponentBoard = new Board();
            opponentBoard.GenerateBoard(10, 10);
            opponent.AddBoard(opponentBoard);

            //give the starting succeful strike
            opponent.PlayerMoves.Add(player.Incoming((new MissileLaunch(new Logic.Game.Helper.Coordinate(7, 1)))));

            var warOver = false;
            var destroyedShip = "";
            //act
            do
            {
                var opponontReport = player.Incoming(opponent.MakeCalculatedLaunch());
                opponent.DamageReport(opponontReport);
                warOver = opponontReport.warOver;
                destroyedShip = opponontReport.ShipIdHit;

            } while (warOver == false);

            //Assert
            Assert.That(warOver, Is.True);
            Assert.That(player.PlayerShips.All(x => x.Sections.All(y => y.isHit)), Is.True);
        }
    }
}
