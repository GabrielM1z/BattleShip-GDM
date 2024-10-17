namespace BattleShip.Models
{
    public class GameHistory
    {
        public List<GameStateHisto> _history = new List<GameStateHisto>();
        public int _currentIndex = -1;

        // Ajoute un nouvel état du jeu à l'historique
        public void InitSave(Game game)
        {
            Console.WriteLine($"HARD RESET => SaveState: _currentIndex={_currentIndex} _history.Count = {_history.Count}");
            _currentIndex = 0;
            _history = [new GameStateHisto(game)];
        }

        public void SaveState(Game game)
        {
            Console.WriteLine($"Start => SaveState: _currentIndex={_currentIndex} _history.Count = {_history.Count}");
            // Supprime les états futurs s'il y a eu des "undos"
            if (_currentIndex < _history.Count - 1)
            {
                _history.RemoveRange(_currentIndex + 1, _history.Count - _currentIndex - 1);
            }

            // Sauvegarde l'état actuel
            _history.Add(new GameStateHisto(game));
            _currentIndex++;
            Console.WriteLine($"End   => SaveState: _currentIndex={_currentIndex} _history.Count = {_history.Count}");
        }

        // Annule l'action et revient à l'état précédent
        public GameStateHisto Undo()
        {
            if (_currentIndex > 0)
            {
                _currentIndex--;
                Console.WriteLine($"Undo  => SaveState: _currentIndex={_currentIndex} _history.Count = {_history.Count}");
                return _history[_currentIndex];
            }
            return null; // Pas d'état précédent
        }



        // Obtient l'état actuel
        public GameStateHisto GetCurrentState()
        {
            if (_currentIndex >= 0)
            {
                return _history[_currentIndex];
            }
            return null; // Aucun état
        }
        public List<GameStateHisto> GetHistory()
        {
            return _history;
        }
    }
}
