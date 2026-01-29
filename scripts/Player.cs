using System;
using Godot;

public partial class Player : CharacterBody2D
{
    [Export]
    private AnimatedSprite2D _animatedSprite;
	[Export]
    private int Speed { get; set; } = 200;

    [Signal]
    public delegate void onWaterSoilEventHandler(Player player);

    //variavel que controla o estado de regar 
    private bool _isWatering = false;

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
            this.waterSoil();  
        }

        Velocity = inputDirection * Speed;
    }

    public void UpdateAnimation()
    {

        if (_isWatering) return;

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
        
        if (_isWatering) return;

        GetInput();
        MoveAndSlide();
        UpdateAnimation();
    }

    private async void waterSoil()
    {
        
        if(_isWatering) return;

        _isWatering = true;
        Velocity = Vector2.Zero; //trava o jogador no tile que esta sendo aguado
        string currentAnimation = _animatedSprite.Animation;

        //troca de animações dependendo da direção que o player tava andando antes de apertar space
        if (currentAnimation == "walk_up") 
        {
            _animatedSprite.Play("water_up");
        }

        else if (currentAnimation == "idle")
        {
            _animatedSprite.Play("water_down");
        }

        else if (currentAnimation == "walk_down") 
        {
            _animatedSprite.Play("water_down");
        }
        else 
        {
            _animatedSprite.Play("water_h");
        }

        //Implmentar
        EmitSignal(SignalName.onWaterSoil, this);
        
        // espera a animação terminar 
        await ToSignal(GetTree().CreateTimer(1.0), "timeout");

         _isWatering = false; // devolve o controle ao jogador
    }
}