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

namespace Breakout.Components
{
	public class Level : BreakoutComponent
	{
		public float Width { get; set; }
		public float Height { get; set; }
		public int Difficulty { get; set; }

		const float wallSize = 0.5f;

		const int rows = 5;
		const int cols = 11;

		const float space = 0.5f;

		const float cubeWidth = 2.0f;
		const float cubeHeight = 1.0f;

		const float offsetY = 15.0f;

		List<Cube> cubes = new List<Cube>();

		Cube lastCollision = null;

		SoundEffect bounce;
		SoundEffect score;

		public Level(Game game) : base(game)
		{
		}


		public override void Initialize()
		{
			base.Initialize();

			if (Difficulty == 0)
			{
				for (int i = 0; i < 3; i++)
				{
					Cube cube = new Cube(Game);
					cube.Position = new Vector2(0.0f, i * (cubeHeight + space) + offsetY);
					cube.Size = new Vector2(cubeWidth, cubeHeight);
					cube.Lives = 3 - i;
					cubes.Add(cube);
				}
			}
			else if (Difficulty == 1)
			{
				int count = 0;

				for (int i = 0; i < rows; i++)
					for (int j = 0; j < cols; j++)
					{
						Cube cube = new Cube(Game);
						cube.Position = new Vector2(((1 - cols) / 2.0f + j) * (cubeWidth + space), i * (cubeHeight + space) + offsetY);
						cube.Size = new Vector2(cubeWidth, cubeHeight);
						cube.Lives = count++ % 4;
						cubes.Add(cube);
					}
			}
			else
			{
				for (int i = 0; i < rows; i++)
					for (int j = 0; j < cols; j++)
					{
						Cube cube = new Cube(Game);
						cube.Position = new Vector2(((1 - cols) / 2.0f + j) * (cubeWidth + space), i * (cubeHeight + space) + offsetY);
						cube.Size = new Vector2(cubeWidth, cubeHeight);
						cube.Lives = i % 4;
						cubes.Add(cube);
					}
			}

			Cube[] walls = new Cube[3];

			for (int i = 0; i < 3; i++)
				walls[i] = new Cube(Game);

			walls[0].Position = new Vector2(-(Width + wallSize) / 2.0f, Height / 2.0f);
			walls[0].Size = new Vector2(wallSize, Height + 2.0f * wallSize);

			walls[1].Position = new Vector2((Width + wallSize) / 2.0f, Height / 2.0f);
			walls[1].Size = new Vector2(wallSize, Height + 2.0f * wallSize);

			walls[2].Position = Vector2.UnitY * (Height + wallSize/ 2.0f);
			walls[2].Size = new Vector2(Width + 2.0f * wallSize, wallSize);

			cubes.AddRange(walls);			

			foreach (Cube cube in cubes)
				cube.Initialize();
		}


		protected override void LoadContent()
		{
			base.LoadContent();

			bounce = Game.Content.Load<SoundEffect>("Sounds/Bounce");
			score = Game.Content.Load<SoundEffect>("Sounds/Score");
		}


		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			foreach (Cube cube in cubes)
				if (cube.Enabled)
					cube.Update(gameTime);
		}


		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			foreach (Cube cube in cubes)
				if (cube.Visible)
					cube.Draw(gameTime);
		}


		public bool CheckCollision(Ball ball)
		{
			bool verticalChange = false;
			bool cubeDestroyed = false;

			foreach (Cube cube in cubes)
			{
				if (cube != lastCollision && cube.Enabled && ball.BoundingSphere.Intersects(cube.BoundingBox))
				{
					if ((IsBetween(ball.Position.Y, cube.Position.Y, cube.Size.Y) ||
						(ball.Position.Y < cube.Position.Y && ball.Velocity.Y < 0.0f) ||
						(ball.Position.Y > cube.Position.Y && ball.Velocity.Y > 0.0f)) && ball.Velocity.X != 0.0f)
						ball.Velocity *= new Vector2(-1.0f, 1.0f);
					else if (!verticalChange)
					{
						ball.Velocity *= new Vector2(1.0f, -1.0f);
						verticalChange = true;
					}

					if (cube.Lives > 0 && --cube.Lives == 0)
					{
						cube.Enabled = cube.Visible = false;
						score.Play();
						cubeDestroyed = true;
					}
					else
						bounce.Play();

					lastCollision = cube;
				}
			}

			return cubeDestroyed;
		}


		public void ClearLastCollision()
		{
			lastCollision = null;
		}


		public bool IsDone()
		{
			return cubes.Where(c => c.Lives > 0).Count() == 0;
		}


		private bool IsBetween(float value, float center, float size)
		{
			return center - size / 2.0f <= value & value <= center + size / 2.0f;
		}


		private Vector2 GetNormal(Vector2 position)
		{
			return Vector2.Zero;
		}
	}
}
