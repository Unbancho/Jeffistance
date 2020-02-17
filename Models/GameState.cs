namespace Jeffistance.Models
{
    public class GameState
    {
        private static GameState currentGameState;

        public User CurrentUser { get; set; }

        private GameState()
        {

        }

        public static GameState GetGameState()
        {
            return currentGameState ??= new GameState();
        }
    }
}
