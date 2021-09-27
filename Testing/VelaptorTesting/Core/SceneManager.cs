using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Velaptor;
using Velaptor.Content;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.UI;

namespace VelaptorTesting.Core
{
    // TODO: Setup this class to be IDisposable
    public class SceneManager: IUpdatable, IDisposable
    {
        private readonly ISpriteBatch spriteBatch;
        private readonly List<IScene> scenes = new();
        private readonly Button nextButton;
        private readonly Button previousButton;
        private readonly TextBox myBox; 
        private int currentSceneIndex;

        public SceneManager(ISpriteBatch spriteBatch)
        {
            this.spriteBatch = spriteBatch;
            
            this.nextButton = new Button(@"next-scene");
            this.nextButton.Click += (sender, e) => NextScene();
            
            this.previousButton = new Button(@"previous-scene");
            this.previousButton.Click += (sender, e) => PreviousScene();

            this.myBox = new TextBox("text-box");
        }

        public void AddScene(IScene sceneBase)
        {
            if (SceneExists(sceneBase.Name))
            {
                throw new Exception($"The sceneBase '{sceneBase.Name}' already exists.");
            }
            
            this.scenes.Add(sceneBase);
        }

        public void RemoveScene(string sceneName)
        {
            if (SceneExists(sceneName) is false)
            {
                return;
            }

            this.scenes.Remove(this.scenes.Where(s => s.Name == sceneName).FirstOrDefault());
        }

        public IScene? CurrentScene => this.scenes.Count <= 0 ? null : this.scenes[this.currentSceneIndex];
        
        public void NextScene()
        {
            if (this.scenes.Count <= 0)
            {
                return;
            }
            
            var previousScene = this.currentSceneIndex;
            this.currentSceneIndex = this.currentSceneIndex >= this.scenes.Count - 1
                ? 0 : this.currentSceneIndex + 1;

            this.scenes[previousScene].IsActive = false;
            this.scenes[previousScene].Unload();
            
            this.scenes[this.currentSceneIndex].IsActive = true;
            this.scenes[this.currentSceneIndex].Load();
        }

        public void PreviousScene()
        {
            if (this.scenes.Count <= 0)
            {
                return;
            }
            
            var previousScene = this.currentSceneIndex;
            this.currentSceneIndex = this.currentSceneIndex <= 0
                ? this.scenes.Count - 1 : this.currentSceneIndex - 1;
            
            this.scenes[previousScene].IsActive = false;
            this.scenes[previousScene].Unload();
            
            this.scenes[this.currentSceneIndex].IsActive = true;
            this.scenes[this.currentSceneIndex].Load();
        }

        public void Load()
        {
            this.scenes[this.currentSceneIndex].Load();
            this.nextButton.LoadContent();
            this.previousButton.LoadContent();

            const int buttonSpacing = 15;
            const int rightMargin = 15;
            var buttonTops = MainWindow.WindowHeight - (new[] {this.nextButton.Height, this.previousButton.Height}.Max() + 20);
            var buttonGroupLeft = MainWindow.WindowWidth - (this.nextButton.Width + this.previousButton.Width + buttonSpacing + rightMargin); 
            this.previousButton.Position = new Point(buttonGroupLeft, buttonTops);
            this.nextButton.Position = new Point(this.previousButton.Position.X + this.previousButton.Width + buttonSpacing, buttonTops);
        }
        
        public void Update(FrameTime frameTime)
        {
            if (this.scenes.Count <= 0)
            {
                return;
            }
            
            this.nextButton.Update(frameTime);
            this.previousButton.Update(frameTime);
            this.scenes[this.currentSceneIndex].Update(frameTime);
        }

        public void Render()
        {
            if (this.scenes.Count <= 0)
            {
                return;
            }

            this.spriteBatch.Clear();
            this.spriteBatch.BeginBatch();
            
            this.scenes[this.currentSceneIndex].Render(this.spriteBatch);
            
            // Render the scene manager UI on top of all other textures
            this.nextButton.Render(this.spriteBatch);
            this.previousButton.Render(this.spriteBatch);
            
            this.spriteBatch.EndBatch();
        }
        
        private bool SceneExists(string name) => this.scenes.Any(s => s.Name == name);

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
