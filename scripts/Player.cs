using System;
using Godot;

public partial class Player : CharacterBody2D
{
    private AnimatedSprite2D _animatedSprite;
	[Export]
    public int Speed { get; set; } = 200;

    public override void _Ready()
    {
        _animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }

    public void GetInput()
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
    public override void _Process(double delta)
    {
        GetInput();
        MoveAndSlide();
        UpdateAnimation();
    }
}