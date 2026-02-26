using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Core;
using Screens;

namespace _487_Group_Project;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics = null!;
    private SpriteBatch _spriteBatch = null!;

    // Core systems
    private SimpleDrawer _drawer = null!;
    private InputState _input = null!;
    private KeyBindings _keyBindings = null!;
    private ScoreManager _scoreManager = null!;

    // Screen management
    private ScreenManager _screenManager = null!;
    private IScreen _activeScreen = null!;

    // Screens
    private MenuScreen _menuScreen = null!;
    private PlayingScreen _playingScreen = null!;
    private PauseScreen _pauseScreen = null!;
    private GameOverScreen _gameOverScreen = null!;
    private VictoryScreen _victoryScreen = null!;
    private KeyConfigScreen _keyConfigScreen = null!;

    // Track where KeyConfig was entered from
    private GameState _keyConfigReturnState = GameState.Menu;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        _graphics.PreferredBackBufferWidth = GameConfig.ScreenWidth;
        _graphics.PreferredBackBufferHeight = GameConfig.ScreenHeight;
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _drawer = new SimpleDrawer(GraphicsDevice);
        _input = new InputState();
        _keyBindings = new KeyBindings();
        _scoreManager = new ScoreManager();
        _screenManager = new ScreenManager();

        // Load fonts
        var defaultFont = Content.Load<SpriteFont>("Fonts/DefaultFont");
        var titleFont = Content.Load<SpriteFont>("Fonts/TitleFont");

        // Create screens
        _menuScreen = new MenuScreen(titleFont, defaultFont);
        _menuScreen.OnStartGame = () => TransitionTo(GameState.Playing);
        _menuScreen.OnKeyConfig = () => OpenKeyConfig(GameState.Menu);
        _menuScreen.OnQuit = () => Exit();

        _playingScreen = new PlayingScreen(_drawer, _input, _keyBindings, _scoreManager, defaultFont);
        _playingScreen.OnPause = () => TransitionTo(GameState.Paused);
        _playingScreen.OnGameOver = () =>
        {
            _gameOverScreen.SetScore(_playingScreen.CurrentScore);
            TransitionTo(GameState.GameOver);
        };
        _playingScreen.OnVictory = () =>
        {
            _victoryScreen.SetScore(_playingScreen.CurrentScore);
            TransitionTo(GameState.Victory);
        };

        _pauseScreen = new PauseScreen(titleFont, defaultFont);
        _pauseScreen.OnResume = () => TransitionTo(GameState.Playing);
        _pauseScreen.OnKeyConfig = () => OpenKeyConfig(GameState.Paused);
        _pauseScreen.OnQuitToMenu = () => TransitionTo(GameState.Menu);

        _gameOverScreen = new GameOverScreen(titleFont, defaultFont);
        _gameOverScreen.OnRetry = () => TransitionTo(GameState.Playing);
        _gameOverScreen.OnQuitToMenu = () => TransitionTo(GameState.Menu);

        _victoryScreen = new VictoryScreen(titleFont, defaultFont);
        _victoryScreen.OnRetry = () => TransitionTo(GameState.Playing);
        _victoryScreen.OnQuitToMenu = () => TransitionTo(GameState.Menu);

        _keyConfigScreen = new KeyConfigScreen(_keyBindings, titleFont, defaultFont);
        _keyConfigScreen.OnBack = () => ReturnFromKeyConfig();

        // Start on menu
        _activeScreen = _menuScreen;
        _activeScreen.OnEnter();
    }

    protected override void Update(GameTime gameTime)
    {
        _input.Update();
        _activeScreen.Update(gameTime, _input);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // If paused, draw the playing screen frozen underneath
        if (_screenManager.CurrentState == GameState.Paused)
        {
            _playingScreen.Draw(_spriteBatch, _drawer);
        }

        _activeScreen.Draw(_spriteBatch, _drawer);

        _spriteBatch.End();
        base.Draw(gameTime);
    }

    // ── Helpers ─────────────────────────────────────────────

    private void TransitionTo(GameState newState)
    {
        if (!_screenManager.TransitionTo(newState))
            return;

        _activeScreen.OnExit();
        _activeScreen = GetScreen(newState);
        _activeScreen.OnEnter();
    }

    private IScreen GetScreen(GameState state) => state switch
    {
        GameState.Menu => _menuScreen,
        GameState.Playing => _playingScreen,
        GameState.Paused => _pauseScreen,
        GameState.GameOver => _gameOverScreen,
        GameState.Victory => _victoryScreen,
        _ => _menuScreen
    };

    private void OpenKeyConfig(GameState returnState)
    {
        _keyConfigReturnState = returnState;
        _activeScreen.OnExit();
        _activeScreen = _keyConfigScreen;
        _activeScreen.OnEnter();
    }

    private void ReturnFromKeyConfig()
    {
        _activeScreen.OnExit();
        _activeScreen = GetScreen(_keyConfigReturnState);
        _activeScreen.OnEnter();
    }
}