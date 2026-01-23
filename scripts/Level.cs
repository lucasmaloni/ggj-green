using Godot;
using System;

public partial class Level : Node
{
	[Export]
	private Player _player;
	[Export]
	private TileMapLayer _dirtLayer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
		_player.onWaterSoil += onPlayerWaterSoil;
	}

	protected void onPlayerWaterSoil(Player player)
	{
		GD.Print("Coloquei agua em " + player.Position);
	}
}
