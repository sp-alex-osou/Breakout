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
	public class Ball : BreakoutComponent
	{
		public Vector2 Position { get; set; }
		public Vector2 Velocity { get; set; }
		public float Radius { get; set; }
		public BoundingSphere BoundingSphere { get; private set; }

		static Model model;


		public Ball(Game game) : base(game)
		{
			Radius = 0.5f;
		}


		public override void Initialize()
		{
			base.Initialize();
		}


		protected override void LoadContent()
		{
			base.LoadContent();

			if (model == null)
				model = Game.Content.Load<Model>("Models/Sphere");
		}


		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			float elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

			if (elapsedSeconds == 0.0f)
				return;

			Position += Velocity * elapsedSeconds;
			BoundingSphere = new BoundingSphere(Position.ToVector3(), Radius);
		}


		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			effect.World = Matrix.CreateScale(2.0f * Radius) * Matrix.CreateTranslation(Position.ToVector3());
			effect.DiffuseColor = Vector3.One * 0.5f;

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
	}
}
