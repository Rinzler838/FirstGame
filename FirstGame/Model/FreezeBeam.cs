﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FirstGame
{
	public class FreezeBeam
	{
		// Image representing the Projectile
		public Texture2D Texture;

		public Texture 2D Texture
		{
			get { return texture; }
			set { texture = value; }
		}

		// Position of the Projectile relative to the upper left side of the screen
		public Vector2 Position;

		public Vector2 Position
		{
			get { return position; }
			set { position = value; }
		}

		// State of the Projectile
		public bool Active;

		// The amount of damage the projectile can inflict to an enemy
		public int Damage;

		public int Damage
		{
			get { return damage; }
			set { damage = value; }
		}

		// Determines how fast the projectile moves
		float projectileMoveSpeed;


		public void Initialize(Texture2D texture, Vector2 position)
		{
			Texture = texture;
			Position = position;
			this.viewport = viewport;

			Active = true;

			Damage = 1; //2

			projectileMoveSpeed = 1f; //20
		}

		public void Update()
		{
			// Projectiles always move to the right
			Position.X += projectileMoveSpeed;

			// Deactivate the bullet if it goes out of screen
			if (Position.X + Texture.Width / 2 > viewport.Width)
				Active = false;
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(Texture, Position, null, Color.White, 0f,
				new Vector2(Width / 2, Height / 2), 1f, SpriteEffects.None, 0f);
		}
	}
}

