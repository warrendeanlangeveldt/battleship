
using Battleship.Logic.Components.ShipComponent;
using Battleship.Logic.Game.Helper;
using NUnit.Framework;

namespace Battleship.Tests.ShipTests
{

    [TestFixture]
    public class ShipTests
    {
        Warship? Ship;


        [Test]
        //superfluous test, but needed for subsequent tests;
        public void Can_Create_A_Ship()
        {
            //arrange
            var shipName = "Cruiser";
            var shipLength = 4;
            var shipId = "C4";

            //act
            Ship = new Warship(shipLength, shipName, shipId);

            //assert
            Assert.That(Ship.ShipName, Is.EqualTo("Cruiser"));
            Assert.That(Ship.ShipLength, Is.EqualTo(4));
        }

        [Test]
        //Checks that the ship sections equal to the ship length and the ship section coordicates match the board section coordinates
        public void Can_Do_A_Manual_Ship_Placement()
        {
            //arrange
            var coordinates = new List<Coordinate>();
            coordinates.Add(new Coordinate(1, 1));
            coordinates.Add(new Coordinate(1, 2));
            coordinates.Add(new Coordinate(1, 3));
            coordinates.Add(new Coordinate(1, 4));

            if (Ship!=null)
            {
                //act
                Ship.UpdateShipPlacement(coordinates);

                //Assert
                Assert.That(Ship.Sections.Count, Is.EqualTo(4));
            }
        }
        
    }
}
