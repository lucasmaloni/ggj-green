using System;
using Godot;

public partial class Player : CharacterBody2D
{
    // --- Configurações de Export ---
    [Export] private AnimatedSprite2D _animatedSprite;
    [Export] private int Speed { get; set; } = 200;
    [Export] public int TileSize = 16; // Ajuste para o tamanho do seu grid (ex: 16, 32, 64)

    // --- Sinais ---
    [Signal] public delegate void onWaterSoilEventHandler(Player player);

    // --- Variáveis de Controle de Estado ---
    private bool _isWatering = false;
    private bool _isMoving = false;
    private Vector2 _targetPosition = Vector2.Zero;

    public override void _Ready()
    {
        // Garante que o alvo inicial seja onde o player começa
        _targetPosition = GlobalPosition;
        
        // Se esqueceu de arrastar o sprite no Inspector, tenta pegar via código
        if (_animatedSprite == null)
            _animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }

    public override void _PhysicsProcess(double delta)
    {
        // 1. BLOQUEIO: Se estiver regando, não faz mais nada.
        if (_isWatering) return;

        // 2. Lógica de movimentação
        if (!_isMoving)
        {
            GetInputTile();
        }
        else
        {
            MoveToTarget(delta);
        }
    }

    private void GetInputTile()
    {
        // PRIORIDADE 1: Ação de Regar (Espaço/Ação configurada)
        if (Input.IsActionJustPressed("place_water"))
        {
            waterSoil();
            return; // Sai para não tentar andar no mesmo frame
        }

        // PRIORIDADE 2: Movimentação
        Vector2 inputDirection = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");

        if (inputDirection != Vector2.Zero)
        {
            // Trava o movimento em apenas um eixo (Sua lógica original)
            if (Mathf.Abs(inputDirection.X) > Mathf.Abs(inputDirection.Y))
                inputDirection.Y = 0;
            else
                inputDirection.X = 0;

            if (inputDirection == Vector2.Zero) return;

            // Define o próximo quadrado
            _targetPosition = GlobalPosition + inputDirection.Normalized() * TileSize;
            _isMoving = true;

            // Inicia a animação de caminhada passando a direção
            UpdateAnimation(inputDirection);
        }
    }

    private void MoveToTarget(double delta)
    {
        // Move suavemente em direção ao alvo
        GlobalPosition = GlobalPosition.MoveToward(_targetPosition, Speed * (float)delta);

        // Verifica se chegou (com margem de erro pequena)
        if (GlobalPosition.IsEqualApprox(_targetPosition))
        {
            GlobalPosition = _targetPosition; // Snap para o valor exato
            _isMoving = false;
            
            Vector2 input = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
            if(input == Vector2.Zero)
            {
                UpdateAnimation(Vector2.Zero);
            }
        }
    }

    public void UpdateAnimation(Vector2 direction)
    {
        // Se estiver no meio da ação de regar, UpdateAnimation não pode mexer no sprite!
        if (_isWatering) return;

        if (direction == Vector2.Zero)
        {
            _animatedSprite.Play("idle");
            return;
        }

        if (direction.X > 0)
        {
            _animatedSprite.Play("walk_h");
            _animatedSprite.FlipH = false;
        }
        else if (direction.X < 0)
        {
            _animatedSprite.Play("walk_h");
            _animatedSprite.FlipH = true;
        }
        else if (direction.Y < 0)
        {
            _animatedSprite.Play("walk_up");
        }
        else if (direction.Y > 0)
        {
            _animatedSprite.Play("walk_down");
        }
    }

    private async void waterSoil()
    {
        if (_isWatering) return;

        _isWatering = true;
        Velocity = Vector2.Zero;
        
        // Pega a animação atual para saber a direção
        string currentAnim = _animatedSprite.Animation;

        // Toca a animação de regar correspondente
        if (currentAnim == "walk_up") _animatedSprite.Play("water_up");
        else if (currentAnim == "walk_down") _animatedSprite.Play("water_down");
        else _animatedSprite.Play("water_h");

        _animatedSprite.Frame = 0;

        // Avisa o sistema de solo
        EmitSignal(SignalName.onWaterSoil, this);

        // Aguarda um tempo fixo (Timer) para garantir que destrave
        // Ajuste 0.6f para o tempo que sua animação leva
        await ToSignal(GetTree().CreateTimer(0.6f), "timeout");

        _isWatering = false;
        UpdateAnimation(Vector2.Zero); // Volta ao estado normal
    }
}