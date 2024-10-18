public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int NbCoup { get; set; }
    public int NbVictoire { get; set; }

    public User(string name)
    {
        Name = name;
        NbCoup = 0;
        NbVictoire = 0;
    }
}