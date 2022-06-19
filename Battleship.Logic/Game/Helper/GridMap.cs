using System.Text.RegularExpressions;

namespace Battleship.Logic.Game.Helper
{
    static class GridMap
    {
        public static readonly string[] Alphabet = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" }; 

        public static Coordinate GetCoordinate(string gridKey)
        {
            var rgx = new Regex(@"([a-zA-Z]{1})(\d{1,2})", RegexOptions.IgnoreCase);
            var match = rgx.Match(gridKey);
            //should do some regex here.
            if (match.Success)
            {
                var xLabel = match.Groups[1].ToString();
                var xCoord = match.Groups[2].Value;

                var idx = Array.FindIndex(Alphabet, x => x == xLabel.ToUpper());

                return new Coordinate(idx+1, Convert.ToInt32(xCoord));
            }
            else
            {
                return null;
            }
        }

        public static string GetGridKey(Coordinate coordinate)
        {
           return $"{Alphabet[coordinate.PositionX - 1]}{coordinate.PositionY}";
        }
    }
}
