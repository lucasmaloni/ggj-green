using System;
using Godot;

public partial class Player : CharacterBody2D
{
    [Export]
    private AnimatedSprite2D _animatedSprite;
	[Export]
    private int Speed { get; set; } = 150;
    [Signal]
    public delegate void onWaterSoilEventHandler(Player player);

    public override void _Ready()
    {
        
    }

    private void GetInput()
    {
        // Definimos a velocidade (atributo herdado) que será chamado em outros métodos
        Vector2 inputDirection = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");

        // Fazemos a matematica para definir se a movimentação é vertical ou horizontal apenas
        if(Mathf.Abs(inputDirection.X) > Mathf.Abs(inputDirection.Y))
        {
            inputDirection.Y = 0;
        }
        else
        {
            inputDirection.X = 0;
        }

        if (Input.IsActionPressed("place_water"))
        {
            waterSoil();
        }

        Velocity = inputDirection * Speed;
    }

    public void UpdateAnimation()
    {
        //Lida com a velocidade x e y para determinar a animação e só ela
        if (Velocity.X > 0)
        {
            _animatedSprite.Play("walk_h");
            _animatedSprite.FlipH = false;
        }
        else if (Velocity.X < 0)
        {
            _animatedSprite.Play("walk_h");
            _animatedSprite.FlipH = true;
        }
        else if (Velocity.Y < 0)
        {
            _animatedSprite.Play("walk_up");
        }
        else if (Velocity.Y > 0)
        {
            _animatedSprite.Play("walk_down");
        }
        else
        {
            _animatedSprite.Play("idle");
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        GetInput();
        MoveAndSlide();
        UpdateAnimation();
    }

    private void waterSoil()
    {
        //Implmentar play da animação de aguar solo (lado do player)
        EmitSignal(SignalName.onWaterSoil, this);
    }
}