using System;

using CoreGraphics;
using Foundation;
using SpriteKit;

#if __IOS__
using UIKit;

#else
using AppKit;
#endif

namespace SpriteKitGame
{
	public class GameScene : SKScene
	{
		public GameScene (IntPtr handle) : base (handle)
		{
		}

		public override void DidMoveToView (SKView view)
		{
			// Setup your scene here
			var myLabel = new SKLabelNode ("Chalkduster") {
				Text = "Hello, World!",
				FontSize = 50,
				Position = new CGPoint (Frame.Width / 2, Frame.Height / 2)
			};

			AddChild (myLabel);
		}

		#if __IOS__
		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			// Called when a touch begins
			foreach (var touch in touches) {
				var location = ((UITouch)touch).LocationInNode (this);

				var sprite = new SKSpriteNode ("Spaceship") {
					Position = location,
					XScale = 0.5f,
					YScale = 0.5f
				};

				var action = SKAction.RotateByAngle (NMath.PI, 1.0);

				sprite.RunAction (SKAction.RepeatActionForever (action));

				AddChild (sprite);
			}
		}
		
#else
		public override void MouseDown (NSEvent theEvent)
		{
			// Called when a mouse click occurs

			var location = theEvent.LocationInNode (this);

			var sprite = SKSpriteNode.FromImageNamed (NSBundle.MainBundle.PathForResource ("Spaceship", "png"));

			sprite.Position = location;
			sprite.SetScale (0.5f);

			var action = SKAction.RotateByAngle (NMath.PI, 1.0);

			sprite.RunAction (SKAction.RepeatActionForever (action));

			AddChild (sprite);
		}
		#endif

		public override void Update (double currentTime)
		{
			// Called before each frame is rendered
		}
	}
}


