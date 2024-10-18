namespace BattleShip.Models
{
    public class GameHistory
    {
        private List<GameStateHisto> _history = new List<GameStateHisto>();
        private int _currentIndex = -1;

        // Initialise l'historique avec l'état initial du jeu
        public void InitSave(Game game)
        {
            Console.WriteLine($"HARD RESET => SaveState: _currentIndex={_currentIndex} _history.Count = {_history.Count}");
            _currentIndex = 0;
            _history = new List<GameStateHisto> { new GameStateHisto(game) }; // Corrected list initialization
        }

        // Sauvegarde l'état actuel du jeu
        public void SaveState(Game game)
        {
            Console.WriteLine($"Start => SaveState: _currentIndex={_currentIndex} _history.Count = {_history.Count}");
            _history.Add(new GameStateHisto(game));
            _currentIndex++;
            Console.WriteLine($"End => SaveState: _currentIndex={_currentIndex} _history.Count = {_history.Count}");
        }

        // Annule l'action et revient à l'état précédent
        public GameStateHisto Undo()
        {
            if (_currentIndex > 0)
            {
                _currentIndex--;
                if (_history.Count > 0){
                    _history.RemoveAt(_history.Count - 1);
                }
                Console.WriteLine($"Undo => SaveState: _currentIndex={_currentIndex} _history.Count = {_history.Count}");
                return _history[_currentIndex];
            }
            return null; // Pas d'état précédent
        }

        // Retourne l'état actuel
        public GameStateHisto GetCurrentState()
        {
            return _currentIndex >= 0 ? _history[_currentIndex] : null;
        }

        // Retourne l'historique complet
        public List<GameStateHisto> GetHistory()
        {
            return _history;
        }
    }
}
