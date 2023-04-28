using Godot;

public partial class TeaserLevel : Node
{
    private BusTeaser _bus;
    private LlamaTeaser _llama;

    public override void _Ready()
    {
        _bus = GetNode<BusTeaser>("bus_teaser");
        _llama = GetNode<LlamaTeaser>("llama_teaser");
    }



    public override void _Process(double delta)
    {
        

    }
}
