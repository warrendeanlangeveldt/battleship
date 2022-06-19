using Battleship.Logic.Game.Helper;

namespace Battleship.Logic.Components.ShipComponent
{
    public class Warship
    {
        public string  ShipName { get; set; }
        public string ShipId { get; set; }
        public int ShipLength { get; set; }

        public List<ShipSection> Sections { get; private set; }

        public Warship(int length,string shipName, string shipId)
        {
            this.ShipLength = length;
            Sections = new List<ShipSection>();
            ShipName = shipName;
            ShipId = shipId;
        }

        public void UpdateShipPlacement(List<Coordinate> shipPositions)
        {
            Sections = new List<ShipSection>();

            foreach(var coordinate in shipPositions)
            {
                Sections.Add(new ShipSection(coordinate));
            }
        }
    }




    public class ShipSection
    {
        public bool isHit { get; set; }

        public Coordinate Coordinate { get; set; }

        internal ShipSection(Coordinate coordinate)
        {
            Coordinate = coordinate;
        }
    }
}
