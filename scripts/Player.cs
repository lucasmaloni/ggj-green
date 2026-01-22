using Godot;

public partial class Character : CharacterBody2D
{
    private AnimatedSprite2D _animatedSprite;
	double movementSpeed = 200;

    public override void _Ready()
    {
        _animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionPressed("move_right"))
        {
            _animatedSprite.Play("walk_h");
        }
        else if (Input.IsActionPressed("move_left"))
        {
			// Por padrão, a animação de caminhar está direcionada para a direita
			// Quando andamos para a esquerda setamos o flipHorizontal como true para virar a sprte para a esquerda
			_animatedSprite.FlipH = true;
            _animatedSprite.Play("walk_h");
        }
		else if (Input.IsActionPressed("walk_up"))
		{
			_animatedSprite.Play("walk_up");
		}
		else if (Input.IsActionPressed("walk_down"))
		{
			_animatedSprite.Play("walk_down");
		}
		else // Se nenhuma dessas teclas foram apertadas, usamos a animação padrão: idle
		{
			_animatedSprite.Play("idle");
		}
    }
}