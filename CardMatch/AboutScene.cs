using System;
using CoreGraphics;
using Foundation;
using SpriteKit;
using UIKit;

namespace CardMatch
{


	public class AboutScene : SKScene
	{
		//decalre variables that are to be used in this class
		private SKSpriteNode _Background;
		private Button _BackButton;

		public AboutScene(CGSize size) : base(size)
		{

		}

		public override void DidMoveToView(SKView view)
		{
			//create button, set it's size position and textt
			_BackButton = new Button();
			_BackButton.Size = new CGSize((this.Frame.Width / 3), this.Frame.Height / 10);
			_BackButton.Position = new CGPoint(this.Frame.Width / 2, this.Frame.Height / 14);
			_BackButton.TextSize = 18.0f;
			_BackButton.Text = "Back";

			//create background, set it's size, position achorpoint and zPosition
			//Zposition makes sure its always at the back and never daws over anything
			_Background = new SKSpriteNode(SKTexture.FromImageNamed("AboutScene"));
			_Background.AnchorPoint = new CGPoint(0, 0);
			_Background.Size = new CGSize(this.Size.Width, this.Size.Height);
			_Background.Position = new CGPoint(0, 0);
			_Background.ZPosition = -1;

			//thats the button and backbround to the scene
			this.AddChild(_Background);
			this.AddChild(_BackButton);
		}

		public override void TouchesBegan(NSSet touches, UIEvent evt)
		{
			foreach (var touch in touches)
			{
				var location = ((UITouch)touch).LocationInNode(this);

				if (_BackButton.CheckTouchBegin(location)) //if the back button is touched
				{
					_BackButton.SwapTexture(); //swap the texture of the backbutton to give the illusion of the button getting pressed in
				}

			}
		}

		public override void TouchesEnded(NSSet touches, UIEvent evt)
		{
			foreach (var touch in touches)
			{
				var location = ((UITouch)touch).LocationInNode(this);

				if (_BackButton.CheckTouchRelease(location)) //if a touch is released on top of the back button
				{
					MenuScene _MenuScene = new MenuScene(this.Size); //create a new menu scene object with the size of the current scene

					this.View.PresentScene(_MenuScene, SKTransition.PushWithDirection(SKTransitionDirection.Right, 1.0f)); //present the scene with a push with direction SKTransition
				}
			}
		}
	}
}

