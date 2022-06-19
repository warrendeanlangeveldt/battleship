using Battleship.Logic.Components.BoardComponent;
using Battleship.Logic.Components.PlayerComponent;
using Battleship.Logic.Components.ShipComponent;
using Battleship.Logic.Game.Helper;

namespace Battleship.Logic.Game
{
    public class Game
    {
        public Player Player1 { get; private set; }
        public Player AiPlayer { get; private set; }

        static int BOARDSIZEX = 10;
        static int BOARDSIZEY = 10;

        public AiDifficulty difficulty;
        private int maxValueForDifficultyRandom = 10;
        private int aiIntelligence = 3; //EASY

        public Game(AiDifficulty difficulty)
        {
            this.difficulty = difficulty;
            if(difficulty == AiDifficulty.MEDIUM)
            {
                aiIntelligence  = 5;
            }else if(difficulty == AiDifficulty.HARD)
            {
                aiIntelligence = 8;
            }

            Player1 = new Player();
            InitialisePlayer(Player1);


            AiPlayer = new Player();
            InitialisePlayer(AiPlayer);

        }

        public void InitialisePlayer(Player player)
        {
            var board = new Board();
            board.GenerateBoard(BOARDSIZEX, BOARDSIZEY);
            player.AddBoard(board);
            DeployFleet(player);
        }

        public void DeployFleet(Player player)
        {
            var rnd = new Random();

            var battleship = new Warship(5,"battleship","b5");
            player.AddShip(battleship);
            player.PlayerBoard.DeployShip(battleship, rnd.Next(0,2) == 0? Direction.VERTICAL:Direction.HORIZONTAL);

            var cruiser = new Warship(4, "cruiser","c4");
            player.AddShip(cruiser);
            player.PlayerBoard.DeployShip(cruiser, rnd.Next(0, 2) == 0 ? Direction.VERTICAL : Direction.HORIZONTAL);

            var frigate = new Warship(3, "frigate","f3");
            player.AddShip(frigate);
            player.PlayerBoard.DeployShip(frigate, rnd.Next(0, 2) == 0 ? Direction.VERTICAL : Direction.HORIZONTAL);

            var destroyer = new Warship(2, "destroyer","d2");
            player.AddShip(destroyer);
            player.PlayerBoard.DeployShip(destroyer, rnd.Next(0, 2) == 0 ? Direction.VERTICAL : Direction.HORIZONTAL);

            var submarine = new Warship(1, "submarine","s1");
            player.AddShip(submarine);
            player.PlayerBoard.DeployShip(submarine, rnd.Next(0, 2) == 0 ? Direction.VERTICAL : Direction.HORIZONTAL);

        }

        public MoveResponse PlayerMove(string gridKey = "")
        {
            var coordinate = GridMap.GetCoordinate(gridKey);

            if(coordinate!=null && (coordinate.PositionX<1 || coordinate.PositionX> Player1.PlayerBoard.BOARDSIZEX || coordinate.PositionY<0 || coordinate.PositionY> Player1.PlayerBoard.BOARDSIZEY))
            {
                return new MoveResponse { Message = "The Board is not big enough for ya?! try again pleass...", warOver = false };
            }

            //player1 will make a move
            var missile = Player1.MakeCalculatedLaunch(coordinate != null? coordinate.PositionX:0,coordinate != null?coordinate.PositionY:0);

            //invalid move
            if(missile == null) return new MoveResponse { Message = "MMM...You've tried that move before",warOver =false, InvalidMove = true};

            //AIplayer will get an incoming attack
            var report = AiPlayer.Incoming(missile);

            //player 1 will get a report
            Player1.DamageReport(report);

            //player1 won the game
            return new MoveResponse { Message = "PLAYER " + (report.IsHit?"Hit":"Miss") + $" on {report.GridKey.ToUpper()}", warOver = report.warOver };

        }

        public MoveResponse AiMove()
        {
            //AI will retaliate based on difficulty
            var rnd = new Random();
            var intelligence = rnd.Next(0, maxValueForDifficultyRandom);

            // so we make a calculated launch if the random number falls within the AiIntellegence level
            //the higher the difficulty level the more likely we will get a calculated launch
            var aiMissile = intelligence <= aiIntelligence ? AiPlayer.MakeCalculatedLaunch() : AiPlayer.MakeRandomLaunch();

            //AI attacks player. player sends back an attack report
            var aiReport = Player1.Incoming(aiMissile);

            //ai stores the report
            AiPlayer.DamageReport(aiReport);

            //AI Wins
            return new MoveResponse { Message = "AI " +( aiReport.IsHit ? "Hit" : "Miss") + $" on {aiReport.GridKey.ToUpper()}", warOver = aiReport.warOver };
        }

      
    }

    public class MoveResponse
    {
        public string Message { get; set; }
        public bool warOver { get; set; }

        public bool InvalidMove { get; set; }
    }


}
