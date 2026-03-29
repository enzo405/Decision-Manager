public class DefeatConditions
{
    public MinMax Stress { get; set; }
    public MinMax Turnover { get; set; }
    public MinMax Performance { get; set; }
    public MinMax Motivation { get; set; }
}


public class MinMax
{
    public int Min { get; set; }
    public int Max { get; set; }
}