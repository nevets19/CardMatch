using System;
using CoreGraphics;
using Foundation;
using SpriteKit;
using UIKit;

namespace CardMatch
{


	public class AboutScene : SKScene
	{
		private SKSpriteNode _Background;
		private Button _BackButton;

		public AboutScene(CGSize size) : base(size)
		{

		}

		public override void DidMoveToView(SKView view)
		{
			_BackButton = new Button();
			_BackButton.Size = new CGSize((this.Frame.Width / 3), this.Frame.Height / 10);
			_BackButton.Position = new CGPoint(this.Frame.Width / 2, this.Frame.Height / 14);
			_BackButton.TextSize = 18.0f;
			_BackButton.Text = "Back";


			_Background = new SKSpriteNode(SKTexture.FromImageNamed("AboutScene"));
			_Background.AnchorPoint = new CGPoint(0, 0);
			_Background.Size = new CGSize(this.Size.Width, this.Size.Height);
			_Background.Position = new CGPoint(0, 0);
			_Background.ZPosition = -1;

			this.AddChild(_Background);
			this.AddChild(_BackButton);
		}

		public override void TouchesBegan(NSSet touches, UIEvent evt)
		{
			foreach (var touch in touches)
			{
				var location = ((UITouch)touch).LocationInNode(this);

				if (_BackButton.CheckTouchBegin(location))
				{
					_BackButton.SwapTexture();
				}

			}
		}

		public override void TouchesEnded(NSSet touches, UIEvent evt)
		{
			foreach (var touch in touches)
			{
				var location = ((UITouch)touch).LocationInNode(this);

				if (_BackButton.CheckTouchRelease(location))
				{
					MenuScene _MenuScene = new MenuScene(this.Size);

					this.View.PresentScene(_MenuScene, SKTransition.PushWithDirection(SKTransitionDirection.Right, 1.0f));
				}
			}
		}
	}
}

