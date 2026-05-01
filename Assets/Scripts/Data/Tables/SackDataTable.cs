public class SackDataTable : IDataTable
{
    public int Id { get; set; }
    public string Name { get; set; }

    public SackLevel Level { get; set; }
    public int Capacity { get; set; }

    public bool CanDrop { get; set; }
}