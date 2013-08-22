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
	public class Cube : BreakoutComponent
	{
		public Vector2 Position { get; set; }
		public Vector2 Size { get; set; }
		public int Lives { get; set; }
		public BoundingBox BoundingBox { get; private set; }

		static Model model;

		const float depth = 3.0f;


		public Cube(Game game) : base(game)
		{
			Size = Vector2.One;
		}


		public override void Initialize()
		{
			base.Initialize();

			if (model == null)
				model = Game.Content.Load<Model>("Models/Cube");
		}


		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			BoundingBox = new BoundingBox()
			{
				Min = new Vector3(Position - (Size / 2.0f), -depth / 2.0f),
				Max = new Vector3(Position + (Size / 2.0f), depth / 2.0f)
			};
		}


		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			effect.World = Matrix.CreateScale(new Vector3(Size, depth)) * Matrix.CreateTranslation(Position.ToVector3());
			effect.DiffuseColor = GetColor().ToVector3();

			foreach (EffectPass pass in effect.CurrentTechnique.Passes)
			{
				pass.Apply();

				foreach (ModelMesh mesh in model.Meshes)
				{
					foreach (ModelMeshPart part in mesh.MeshParts)
						part.Effect = effect;

					mesh.Draw();
				}
			}
		}


		private Color GetColor()
		{
			switch (Lives)
			{
				case 0: return Color.Gray;
				case 1: return Color.Green;
				case 2: return Color.Orange;
				case 3: return Color.Red;
				default: return Color.Blue;
			}
		}


		public Plane GetPlane(CubeFace face)
		{
			switch (face)
			{
				case CubeFace.Left: return new Plane(Vector3.Left, Position.X - Size.X / 2.0f);
				case CubeFace.Right: return new Plane(Vector3.Right, Position.X + Size.X / 2.0f);
				case CubeFace.Bottom: return new Plane(Vector3.Down, Position.Y - Size.Y / 2.0f);
				case CubeFace.Top: return new Plane(Vector3.Up, Position.Y + Size.Y / 2.0f);
				default: throw new ArgumentException();
			}
		}
	}
}
