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
		private Button _AboutButton;
		private SKSpriteNode _Background;
		protected MenuScene(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public MenuScene(CGSize size) : base(size)
		{

		}
		public override void DidMoveToView(SKView view)
		{
			_Background = new SKSpriteNode(SKTexture.FromImageNamed("MainMenuScene"));
			_Background.AnchorPoint = new CGPoint(0, 0);
			_Background.Size = new CGSize(this.Size.Width, this.Size.Height);
			_Background.Position = new CGPoint(0, 0);
			_Background.ZPosition = -1;

			this.AddChild(_Background);


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
				if (_AboutButton.CheckTouchBegin(location))
				{
					_AboutButton.SwapTexture();
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
					SKScene _ModeScene = new ModeScene(this.Size);


					this.View.PresentScene(_ModeScene, SKTransition.DoorsOpenVerticalWithDuration(1.0f));
				}
				if (_AboutButton.CheckTouchRelease(location))
				{
					SKScene _AboutScene = new AboutScene(this.Size);


					this.View.PresentScene(_AboutScene, SKTransition.PushWithDirection(SKTransitionDirection.Left,1.0));
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
			_AboutButton = new Button();

			_PlayButton.Position = new CGPoint(this.Frame.Width / 2.0f, (this.Frame.Height / 3.0f) * 1.5f);
			_PlayButton.Size = new CGSize((this.Frame.Width / 3.0f) * 2.0f, this.Frame.Height / 9.0f);
			_PlayButton.TextSize = _PlayButton.Size.Height / 1.5f;
			_PlayButton.Text = "Play";

			_AboutButton.Position = new CGPoint(this.Frame.Width / 2.0f, (this.Frame.Height / 3.0f) * 1.0f);
			_AboutButton.Size = new CGSize((this.Frame.Width / 3.0f) * 2.0f, this.Frame.Height / 9.0f);
			_AboutButton.TextSize = _AboutButton.Size.Height / 1.5f;
			_AboutButton.Text = "About";

			//_PlayButton.TextColor = UIColor.FromRGB(0.34f,0.12f,0.4f);
			this.AddChild(_PlayButton);
			this.AddChild(_AboutButton);
		}
	}
}

