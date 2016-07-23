using System;

using CoreGraphics;
using Foundation;
using SpriteKit;
using UIKit;

namespace CardMatch
{
	public class MenuScene : SKScene
	{
		private Button _PlayButton;
		protected MenuScene(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public override void DidMoveToView(SKView view)
		{
			SetUpButtons();
		}

		public override void TouchesBegan(NSSet touches, UIEvent evt)
		{
			// Called when a touch begins
			foreach (var touch in touches)
			{
				var location = ((UITouch)touch).LocationInNode(this);
				if (_PlayButton.CheckTouchBegin(location))
				{
					//play button was pressed
					Console.WriteLine(location.ToString());
					_PlayButton.SwapTexture();
				}
			}
		}

		public override void TouchesEnded(NSSet touches, UIEvent evt)
		{
			foreach (var touch in touches)
			{
				var location = ((UITouch)touch).LocationInNode(this);
				if (_PlayButton.CheckTouchRelease(location))
				{
					SKScene _GameScene = new GameScene(this.Size);


					this.View.PresentScene(_GameScene, SKTransition.DoorsOpenVerticalWithDuration(1.0f));
				}
			}
		}

		public override void Update(double currentTime)
		{
			// Called before each frame is rendered
		}

		private void SetUpButtons()
		{
			_PlayButton = new Button();
			_PlayButton.Position = new CGPoint(this.Frame.Width / 2.0f, (this.Frame.Height / 3.0f) * 2.0f);
			_PlayButton.Size = new CGSize((this.Frame.Width / 3.0f) * 2.0f, this.Frame.Height / 9.0f);
			_PlayButton.TextSize = _PlayButton.Size.Height / 1.5f;
			_PlayButton.Text = "Play";
			//_PlayButton.TextColor = UIColor.FromRGB(0.34f,0.12f,0.4f);
			this.AddChild(_PlayButton);
		}
	}
}

