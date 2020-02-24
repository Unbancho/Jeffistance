namespace Jeffistance.Models
{
    public class GameState
    {
        private static GameState _currentGameState;

        public User CurrentUser { get; set; }

        private GameState()
        {

        }

        public static GameState GetGameState()
        {
            return _currentGameState ??= new GameState();
        }
    }
}
