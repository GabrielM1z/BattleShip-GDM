public class Fleet
{
    public List<Boat> Boats { get; }

    public Fleet()
    {
        Boats = new List<Boat>();
    }

    public void AddBoat(Boat boat)
    {
        Boats.Add(boat);
    }
}
