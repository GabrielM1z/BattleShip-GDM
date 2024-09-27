public class Game
{
    public Guid Id { get; set; } 
    public char[][] GridJ1 { get; set; }
    public char[][] GridJ2 { get; set; }
    public bool?[][] MaskedGridJ1 { get; set; }
    public bool?[][] MaskedGridJ2 { get; set; }
}
