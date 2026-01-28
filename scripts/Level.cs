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
		Vector2I tileToBeWatered = _dirtLayer.LocalToMap(player.Position);
		Vector2I tileToBeWateredAtlas = _dirtLayer.GetCellAtlasCoords(tileToBeWatered);


		/* Se o tile pra ser aguado for o bottom da sprite, ele deve ser rotacionado */
		_dirtLayer.SetCell(tileToBeWatered, 1, tileToBeWateredAtlas, 0);
	}
}
