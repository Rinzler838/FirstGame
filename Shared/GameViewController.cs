using System;
using SpriteKit;
using Foundation;

#if __IOS__
using UIKit;

#else
using AppKit;
#endif

namespace SpriteKitGame
{
	[Register ("GameViewController")]
	#if __IOS__
	public partial class GameViewController : UIViewController
	#else
	public partial class GameViewController : NSViewController
	#endif
	{
		public GameViewController (IntPtr handle) : base (handle)
		{
		}

		#if __IOS__
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			Setup ();
		}	
		
#else
		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();
			Setup ();
		}
		#endif

		void Setup ()
		{
			// Configure the view.
			GameView.ShowsFPS = true;
			GameView.ShowsNodeCount = true;
			/* Sprite Kit applies additional optimizations to improve rendering performance */
			GameView.IgnoresSiblingOrder = true;

			// Create and configure the scene.
			var scene = SKNode.FromFile<GameScene> ("GameScene");
			scene.ScaleMode = SKSceneScaleMode.AspectFill;

			// Present the scene.
			GameView.PresentScene (scene);
		}

		#if __IOS__
		public override bool ShouldAutorotate ()
		{
			return true;
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone ? UIInterfaceOrientationMask.AllButUpsideDown : UIInterfaceOrientationMask.All;
		}

		public override bool PrefersStatusBarHidden ()
		{
			return true;
		}
		#endif
	}
}


