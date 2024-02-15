namespace TextEventVisualizer.Models
{
    public class Bias
    {
        public List<string> Concepts { get; set; }
        public float Force { get; set; }
        public Bias()
        {
            Concepts = new();
            Force = 0.00f;
        }
    }
}
