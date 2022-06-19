// See https://aka.ms/new-console-template for more information
using Battleship.Logic.Game;
using Battleship.Logic.Game.Helper;
using System.Text.RegularExpressions;

var game = new Game(AiDifficulty.MEDIUM);

PrintBoard(game);

LetTheWarBegin(game);

void LetTheWarBegin(Game game)
{
    //player1 the game
    ConsoleKeyInfo cki;

    bool gameOver = false;
    var winner = "";

    Console.WriteLine();
    Console.WriteLine("Input coordinate and press ENTER! (just press Enter to launch a calculated strike)");
    do
    {
        string gridKey = Console.ReadLine();
        var playerWon = false;
        var aiWon = false;

        var rgx = new Regex(@"[a-zA-Z]{1}\d{1,2}", RegexOptions.IgnoreCase);
        var match = rgx.Match(gridKey);

        if ((gridKey == "") || (match.Success))
        {
            //for now player1 only makes calculated launches. can extend this to accept coordinate input
            var playerMoveResponse = game.PlayerMove(match.Success?gridKey:"");
            playerWon = playerMoveResponse.warOver;
            PrintBoard(game);
            

            if (!playerWon && !playerMoveResponse.InvalidMove)
            {
                Console.WriteLine();
                Console.WriteLine("AI Player retaliating");
                Thread.Sleep(1000);

                var aiMoveResponse = game.AiMove();
                aiWon = aiMoveResponse.warOver;
                PrintBoard(game);
                Console.WriteLine();
                Console.WriteLine(aiMoveResponse.Message);

            }

            Console.WriteLine(playerMoveResponse.Message);
            Console.WriteLine();
            Console.WriteLine("Input coordinate and press ENTER! (just press Enter to launch a calculated strike)");

            if (playerWon)
            {
                Console.WriteLine("------ PLAYER WON THE WAR! --------");

                gameOver = true;
            }
            else if(aiWon)
            {
                Console.WriteLine("------ AI WON THE WAR! --------");

                gameOver = true;
            }

        }
        else
        {
            PrintBoard(game);
            Console.WriteLine();
            Console.WriteLine("You missed the launch button!. Be careful what you press, you might blow yourself up ;)");
        }

    } while (!gameOver);

    Console.WriteLine("-----------------------------" + winner + "-------------------------------");
    Console.ReadLine();
}

void PrintBoard(Game game)
{
    Console.Clear();
    Console.WriteLine("BATTLESHIP! - Difficulty: " + game.difficulty);
    //Show the boards 
    Console.WriteLine("----------------------------------------- Player1 Board / AI Missile Strikes ------------------------------------------");
    Console.WriteLine();
    Console.WriteLine();
    game.Player1.PrintToConsole();

    Console.WriteLine();
    Console.WriteLine("--------------------------------------------- AI Board / Player Missile Strikes ---------------------------------------");
    Console.WriteLine();
    game.AiPlayer.PrintToConsole();
}