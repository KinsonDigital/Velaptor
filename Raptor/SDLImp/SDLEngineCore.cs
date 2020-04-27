using Raptor.Plugins;
using SDLCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace Raptor.SDLImp
{
    /// <summary>
    /// Provides the core of a game engine which facilitates how the engine starts, stops,
    /// manages time and how the game loop runs.
    /// </summary>
    internal class SDLEngineCore : IEngineCore
    {
        #region Public Events
        public event EventHandler<OnUpdateEventArgs>? OnUpdate;
        public event EventHandler<OnRenderEventArgs>? OnRender;
        public event EventHandler? OnInitialize;
        public event EventHandler? OnLoadContent;
        #endregion


        #region Private Fields
        private readonly SDL? _sdl = null;
        private readonly SDLImage? _sdlImage = null;
        private readonly SDLFonts? _sdlFonts = null;
        private Stopwatch _timer = new Stopwatch();
        private TimeSpan _lastFrameTime;
        private bool _isRunning;
        private float _targetFrameRate = 1000f / 120f;
        private readonly Queue<float> _frameTimes = new Queue<float>();
        #endregion


        #region Constructors
        /// <summary>
        /// Creates a new instance of <see cref="SDLEngineCore"/>.
        /// </summary>
        public SDLEngineCore()
        {
            //TODO: Add code here to load the SDL libraries using library loaders
        }
        #endregion


        #region Props
        /// <summary>
        /// Gets or sets the width of the game window.
        /// </summary>
        public int WindowWidth
        {
            get
            {
                if (_sdl is null)
                    return 0;

                _sdl.GetWindowSize(WindowPtr, out int w, out _);

                return w;
            }
            set
            {
                if (_sdl is null)
                    return;

                _sdl.GetWindowSize(WindowPtr, out _, out int h);
                _sdl.SetWindowSize(WindowPtr, value, h);
            }
        }

        /// <summary>
        /// Gets or sets the height of the game window.
        /// </summary>
        public int WindowHeight
        {
            get
            {
                if (_sdl is null)
                    return 0;

                _sdl.GetWindowSize(WindowPtr, out _, out int h);

                return h;
            }
            set
            {
                if (_sdl is null)
                    return;

                _sdl.GetWindowSize(WindowPtr, out int w, out _);
                _sdl.SetWindowSize(WindowPtr, w, value);
            }
        }

        /// <summary>
        /// Gets or sets the renderer that renders graphics to the window.
        /// </summary>
        public IRenderer? Renderer { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating the type of game loop to run.
        /// </summary>
        public TimeStepType TimeStep { get; set; } = TimeStepType.Fixed;

        /// <summary>
        /// Gets the current FPS for the current running frame.
        /// </summary>
        public float CurrentFPS { get; private set; }

        /// <summary>
        /// Gets the pointer to the window.
        /// </summary>
        internal static IntPtr WindowPtr { get; private set; } = IntPtr.Zero;

        /// <summary>
        /// Gets the renderer pointer.
        /// </summary>
        internal static IntPtr RendererPointer { get; private set; } = IntPtr.Zero;

        /// <summary>
        /// Gets or sets the current state of the keyboard for the current frame.
        /// </summary>
        internal static List<Keycode> CurrentKeyboardState { get; set; } = new List<Keycode>();

        /// <summary>
        /// Gets or sets the previous state of the keyboard for the previous frame.
        /// </summary>
        internal static List<Keycode> PreviousKeyboardState { get; set; } = new List<Keycode>();

        /// <summary>
        /// Gets the current state of the left mouse button.
        /// </summary>
        internal static bool CurrentLeftMouseButtonState { get; private set; }

        /// <summary>
        /// Gets the current state of the right mouse button.
        /// </summary>
        internal static bool CurrentRightMouseButtonState { get; private set; }

        /// <summary>
        /// Gets the current state of the middle mouse button.
        /// </summary>
        internal static bool CurrentMiddleMouseButtonState { get; private set; }

        /// <summary>
        /// Gets the current position of the mouse cursur in the game window.
        /// </summary>
        internal static Vector2 MousePosition { get; private set; }
        #endregion


        #region Public Methods
        /// <summary>
        /// Starts the engine.
        /// </summary>
        public void StartEngine()
        {
            InitEngine();
            OnInitialize?.Invoke(this, new EventArgs());
            Run();
        }


        /// <summary>
        /// Stops the engine.
        /// </summary>
        public void StopEngine()
        {
            _timer.Stop();
            _isRunning = false;
        }


        /// <summary>
        /// Sets how many frames the engine will process per second.
        /// </summary>
        /// <param name="value">The total number of frames.</param>
        public void SetFPS(float value) => _targetFrameRate = 1000f / value;


        /// <summary>
        /// Returns true if the engine is running.
        /// </summary>
        /// <returns></returns>
        public bool IsRunning() => _isRunning;


        /// <summary>
        /// Injects any arbitrary data into the plugin for use.  Must be a class.
        /// </summary>
        /// <typeparam name="T">The type of data to inject.</typeparam>
        /// <param name="data">The data to inject.</param>
        public void InjectData<T>(T data) where T : class => throw new NotImplementedException();



        /// <summary>
        /// Gets the data as the given type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="option">Used to pass in options for the <see cref="GetData{T}(int)"/> implementation to process.</param>
        /// <typeparam name="T">The type of data to get.</typeparam>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
        public T? GetData<T>(int option) where T : class
        {
            var ptrContainer = new PointerContainer();

            if (option == 1)
            {
                ptrContainer.PackPointer(RendererPointer);
            }
            else if (option == 2)
            {
                ptrContainer.PackPointer(WindowPtr);
            }
            else
            {
                throw new Exception($"Incorrect {nameof(option)} parameter in '{nameof(SDLEngineCore)}.{nameof(GetData)}()'");
            }


            return ptrContainer as T;
        }


        /// <summary>
        /// Disposes of the <see cref="SDLEngineCore"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            _isRunning = false;

            if (!(_sdl is null))
            {
                _sdl.DestroyRenderer(RendererPointer);
                _sdl.DestroyWindow(WindowPtr);
            }

            //Quit SDL sub systems
            if (!(_sdlFonts is null))
                _sdlFonts.Quit();

            if (!(_sdlImage is null))
                _sdlImage.Quit();

            if (!(_sdl is null))
                _sdl.Quit();
        }
        #endregion


        #region Private Methods
        /// <summary>
        /// Initializes the engine by setting up SDL.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "<Pending>")]
        private void InitEngine()
        {
            if (_sdl is null)
                throw new Exception("The SDL library is not loaded.");

            //Initialize SDL
            if (_sdl.Init(SDL.InitVideo) < 0)
            {
                throw new Exception($"SDL could not initialize! \n\nError: {_sdl.GetError()}");
            }
            else
            {
                //Set texture filtering to linear
                if (_sdl.SetHint(SDL.HintRenderScaleQuality, "0"))
                    throw new Exception("Warning: Linear texture filtering not enabled!");

                //Create window
                WindowPtr = _sdl.CreateWindow("SDL Tutorial", SDL.WindowPosCentered, SDL.WindowPosCentered,
                    640, 480, WindowFlags.WindowShown);

                if (WindowPtr == IntPtr.Zero)
                {
                    throw new Exception($"Window could not be created! \n\nError: {_sdl.GetError()}");
                }
                else
                {
                    //Create vsynced renderer for window
                    var renderFlags = RendererFlags.RendererAccelerated;

                    Renderer = new SDLRenderer();
                    RendererPointer = _sdl.CreateRenderer(WindowPtr, -1, renderFlags);

                    var ptrContainer = new PointerContainer();
                    ptrContainer.PackPointer(RendererPointer);

                    //TODO: Figure out how to deal with the pointer needed by the Renderer
                    //Renderer.InjectData(ptrContainer);

                    if (RendererPointer == IntPtr.Zero)
                    {
                        throw new Exception($"Renderer could not be created! \n\nError: {_sdl.GetError()}");
                    }
                    else
                    {
                        //Initialize renderer color
                        _sdl.SetRenderDrawColor(RendererPointer, 48, 48, 48, 255);

                        //Initialize PNG loading
                        var imgFlags = IMGInitFlags.PNG;

                        if (_sdlImage is null || (_sdlImage.Init(imgFlags) > 0 & imgFlags > 0) == false)
                            throw new Exception($"image could not initialize! \n\nimage Error: {_sdl.GetError()}");

                        //Initialize ttf
                        if (_sdlFonts is null || _sdlFonts.Init() == -1)
                            throw new Exception($"ttf could not initialize! \n\nttf Error: {_sdl.GetError()}");
                    }
                }
            }
        }


        /// <summary>
        /// Runs the engine.
        /// </summary>
        private void Run()
        {
            _isRunning = true;
            _timer = new Stopwatch();
            _timer.Start();

            while (_isRunning)
            {
                UpdateInputStates();

                if (TimeStep == TimeStepType.Fixed)
                {
                    if (_timer.Elapsed.TotalMilliseconds >= _targetFrameRate)
                    {
                        var engineTime = new EngineTime()
                        {
                            ElapsedEngineTime = _timer.Elapsed,
                            TotalEngineTime = _timer.Elapsed
                        };

                        OnUpdate?.Invoke(this, new OnUpdateEventArgs(engineTime));

                        OnRender?.Invoke(this, new OnRenderEventArgs(Renderer));

                        //Add the frame time to the list of previous frame times
                        _frameTimes.Enqueue((float)_timer.Elapsed.TotalMilliseconds);

                        //If the list is full, dequeue the oldest item
                        if (_frameTimes.Count >= 100)
                            _frameTimes.Dequeue();

                        //Calculate the average frames per second
                        CurrentFPS = (float)Math.Round(1000f / _frameTimes.Average(), 2);

                        _timer.Restart();
                    }
                }
                else if (TimeStep == TimeStepType.Variable)
                {
                    var currentFrameTime = _timer.Elapsed;
                    var elapsed = currentFrameTime - _lastFrameTime;

                    _lastFrameTime = currentFrameTime;

                    var engineTime = new EngineTime()
                    {
                        ElapsedEngineTime = _timer.Elapsed,
                        TotalEngineTime = _timer.Elapsed
                    };

                    OnUpdate?.Invoke(this, new OnUpdateEventArgs(engineTime));

                    OnRender?.Invoke(this, new OnRenderEventArgs(Renderer));

                    _timer.Stop();

                    //Add the frame time to the list of previous frame times
                    _frameTimes.Enqueue((float)elapsed.TotalMilliseconds);

                    //If the list is full, dequeue the oldest item
                    if (_frameTimes.Count >= 100)
                        _frameTimes.Dequeue();

                    //Calculate the average frames per second
                    CurrentFPS = (float)Math.Round(1000f / _frameTimes.Average(), 2);

                    _timer.Start();
                }

                //Update the previous state of the keyboard
                PreviousKeyboardState.Clear();
                PreviousKeyboardState.AddRange(CurrentKeyboardState);
            }
        }


        /// <summary>
        /// Properly shuts down the engine and releases SDL resources.
        /// </summary>
        private void ShutDown() => Dispose();


        /// <summary>
        /// Updates the state of all the keyboard keys.
        /// </summary>
        private void UpdateInputStates()
        {
            if (_sdl is null)
                return;

            while (_sdl.PollEvent(out var e) != 0)
            {
                if (e.type == EventType.Quit)
                {
                    ShutDown();
                }
                else if (e.type == EventType.KeyDown)
                {
                    if (!CurrentKeyboardState.Contains(e.key.keysym.sym))
                        CurrentKeyboardState.Add(e.key.keysym.sym);
                }
                else if (e.type == EventType.KeyUp)
                {
                    CurrentKeyboardState.Remove(e.key.keysym.sym);
                }
                else if (e.type == EventType.MouseButtonDown)
                {
                    CurrentLeftMouseButtonState = e.button.button == 1 && e.button.state == 1;
                    CurrentMiddleMouseButtonState = e.button.button == 2 && e.button.state == 1;
                    CurrentRightMouseButtonState = e.button.button == 3 && e.button.state == 1;
                }
                else if (e.type == EventType.MouseButtonUp)
                {
                    CurrentLeftMouseButtonState = e.button.button == 1 && e.button.state == 1;
                    CurrentMiddleMouseButtonState = e.button.button == 2 && e.button.state == 1;
                    CurrentRightMouseButtonState = e.button.button == 3 && e.button.state == 1;
                }
                else if (e.type == EventType.MouseMotion)
                {
                    MousePosition = new Vector2(e.button.x, e.button.y);
                }
            }
        }
        #endregion
    }
}
