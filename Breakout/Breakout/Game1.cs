using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using CameraLib;

using Breakout.Components;

namespace Breakout
{
	public class Game1 : Game
	{
		GraphicsDeviceManager graphics;

		ICamera camera;

		Cube bar;
		Ball ball;
		Level[] levels = new Level[3];

		Level currentLevel;

		Point center;

		SpriteFont font;
		SpriteFont fontEnd;
		SpriteBatch spriteBatch;

		int lives = 3;
		int score = 0;
		int level = 0;

		KeyboardState prevKeyboardState;

		SoundEffect bounce;
		SoundEffect life;

		bool end;

		const float speed = 30.0f;
		
		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";

			camera = new BasicCamera(this);
			bar = new Cube(this);
			ball = new Ball(this);

			for (int i = 0; i < levels.Length; i++)
				levels[i] = new Level(this);

			currentLevel = levels[0];

			Components.Add(camera);
			Components.Add(ball);
			Components.Add(bar);

			Services.AddService(typeof(ICameraService), camera);
		}


		protected override void Initialize()
		{
			for (int i = 0; i < levels.Length; i++)
			{
				levels[i].Width = 40.0f;
				levels[i].Height = 30.0f;
				levels[i].Difficulty = i;
				levels[i].Initialize();
			}

			camera.Position = new Vector3(0.0f, 15.0f, 50.0f);

			ball.Position = Vector2.UnitY;

			bar.Size = new Vector2(5.0f, 1.0f);

			InitializeMouse();

			base.Initialize();

			spriteBatch = new SpriteBatch(GraphicsDevice);
		}


		protected override void LoadContent()
		{
			font = Content.Load<SpriteFont>("Fonts/HUD");
			fontEnd = Content.Load<SpriteFont>("Fonts/End");
			bounce = Content.Load<SoundEffect>("Sounds/Bounce");
			life = Content.Load<SoundEffect>("Sounds/Life");

			base.LoadContent();
		}


		protected override void Update(GameTime gameTime)
		{
			if (Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			if (end)
				return;

			if (Keyboard.GetState().IsKeyDown(Keys.Enter) && prevKeyboardState.IsKeyUp(Keys.Enter) && level < 2)
				currentLevel = levels[++level];

			if (Keyboard.GetState().IsKeyDown(Keys.R) && prevKeyboardState.IsKeyUp(Keys.R))
			{
				ball.Position = bar.Position + Vector2.UnitY * bar.Size.Y;
				ball.Velocity = Vector2.Zero;
			}

			prevKeyboardState = Keyboard.GetState();

			if (Mouse.GetState().LeftButton == ButtonState.Pressed && ball.Velocity == Vector2.Zero)
				ball.Velocity = Vector2.Normalize(Vector2.UnitY) * speed;

			float move = Mouse.GetState().X - center.X;

			bar.Position += Vector2.UnitX * move * 0.1f;

			Mouse.SetPosition(center.X, center.Y);

			if (bar.Position.X + bar.Size.X / 2.0f > currentLevel.Width / 2.0f)
				bar.Position = new Vector2(currentLevel.Width / 2.0f - bar.Size.X / 2.0f, bar.Position.Y);
			else if (bar.Position.X - bar.Size.X / 2.0f < -currentLevel.Width / 2.0f)
				bar.Position = new Vector2(-currentLevel.Width / 2.0f + bar.Size.X / 2.0f, bar.Position.Y);

			if (ball.Position.Y < -2.0f)
			{
				lives--;
				ball.Velocity = Vector2.Zero;
				life.Play();
				currentLevel.ClearLastCollision();
			}

			if (ball.Velocity == Vector2.Zero && lives > 0)
				ball.Position = bar.Position + Vector2.UnitY * bar.Size.Y;
			else if (ball.Velocity.Y < 0.0f && ball.BoundingSphere.Intersects(bar.BoundingBox))
			{
				currentLevel.ClearLastCollision();

				bounce.Play();

				float f = (bar.Position.X - ball.Position.X) / (bar.Size.X / 2.0f);
				Vector2 normal = Vector2.Transform(Vector2.UnitY, Matrix.CreateRotationZ(f * MathHelper.PiOver4));
				ball.Velocity = Vector2.Reflect(ball.Velocity, normal);

				if (Math.Abs(ball.Velocity.X) > Math.Abs(ball.Velocity.Y))
 					ball.Velocity = Vector2.Normalize((ball.Velocity.X > 0.0f) ? Vector2.One : new Vector2(-1.0f, 1.0f)) * speed;
			}

			currentLevel.Update(gameTime);

			base.Update(gameTime);

			if (currentLevel.CheckCollision(ball))
				score++;

			if (currentLevel.IsDone())
			{
				level++;

				if (level < levels.Length)
				{
					currentLevel = levels[level];
					bar.Position = Vector2.Zero;
					ball.Position = Vector2.UnitY;
					ball.Velocity = Vector2.Zero;
				}
			}

			if (lives == 0 || level == 3)
			{
				end = true;
				level = 2;
			}
		}


		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.Black);

			spriteBatch.Begin();

			spriteBatch.DrawString(font, "Level: " + (level + 1), new Vector2(10, 10), Color.White);
			spriteBatch.DrawString(font, "Lives: " + lives, new Vector2(10, 40), Color.Red);
			spriteBatch.DrawString(font, "Score: " + score, new Vector2(10, 70), Color.Green);

			Vector2 youWon = fontEnd.MeasureString("YOU WON") / 2.0f;
			Vector2 gameOver = fontEnd.MeasureString("GAME OVER") / 2.0f;

			if (end)
			{
				if (lives > 0)
					spriteBatch.DrawString(fontEnd, "YOU WON", new Vector2(center.X - youWon.X, center.Y + youWon.Y), Color.Green);
				else
					spriteBatch.DrawString(fontEnd, "GAME OVER", new Vector2(center.X - gameOver.X, center.Y + gameOver.Y), Color.Red);
			}
				

			spriteBatch.End();

			GraphicsDevice.DepthStencilState = DepthStencilState.Default;

			currentLevel.Draw(gameTime);

			base.Draw(gameTime);
		}


		private void InitializeMouse()
		{
			center.X = Window.ClientBounds.Width / 2;
			center.Y = Window.ClientBounds.Height / 2;

			Mouse.SetPosition(center.X, center.Y);
		}
	}

	public static class Extension
	{
		public static Vector3 ToVector3(this Vector2 v)
		{
			return new Vector3(v, 0.0f);
		}
	}
}
