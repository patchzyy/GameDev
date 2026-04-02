public class Stat
{
    public string Label { get; set; }
    public string Value { get; set; }

    public Stat(string label, string value)
    {
        Label = label;
        Value = value;
    }
}