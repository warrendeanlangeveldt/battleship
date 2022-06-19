using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship.Logic.Game.Helper
{
    public class Coordinate
    {
        public int PositionX { get; }
        public int PositionY { get; }

        public Coordinate(int positionX, int positionY)
        {
            PositionX = positionX;
            PositionY = positionY;
        }
    }
}
