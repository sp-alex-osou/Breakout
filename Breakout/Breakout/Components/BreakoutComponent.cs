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

namespace Breakout.Components
{
	public abstract class BreakoutComponent : DrawableGameComponent
	{
		protected static ICameraService camera;
		protected static BasicEffect effect;


		public BreakoutComponent(Game game) : base(game)
		{
		}


		public override void Initialize()
		{
			base.Initialize();

			if (camera == null)
				camera = (ICameraService)Game.Services.GetService(typeof(ICameraService));

			if (effect == null)
				effect = new BasicEffect(Game.GraphicsDevice);
		}


		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			camera = (ICameraService)Game.Services.GetService(typeof(ICameraService));
		}


		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			effect.World = Matrix.Identity;
			effect.View = camera.View;
			effect.Projection = camera.Projection;

			effect.LightingEnabled = true;
			effect.PreferPerPixelLighting = true;
			effect.AmbientLightColor = Vector3.One * 0.2f;

			effect.DirectionalLight0.Direction = Vector3.Forward;
			effect.DirectionalLight0.DiffuseColor = Vector3.One;
			effect.DirectionalLight0.SpecularColor = Vector3.One;

			effect.DiffuseColor = Vector3.One;
			effect.SpecularColor = Vector3.Zero;
			effect.SpecularPower = 0.0f;
			effect.EmissiveColor = Vector3.Zero;
		}
	}
}
