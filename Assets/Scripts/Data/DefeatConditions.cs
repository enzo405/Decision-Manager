public class DefeatConditions
{
    public MinMaxDto Stress { get; set; }
    public MinMaxDto Turnover { get; set; }
    public MinMaxDto Performance { get; set; }
    public MinMaxDto Motivation { get; set; }
}


public class MinMaxDto
{
    public int Min { get; set; }
    public int Max { get; set; }
}