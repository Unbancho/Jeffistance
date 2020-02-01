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
            if (currentGameState is null) currentGameState = new GameState();
            return currentGameState;
        }
    }
}
