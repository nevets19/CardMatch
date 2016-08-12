using System;

using CoreGraphics;
using Foundation;
using SpriteKit;
using UIKit;

namespace CardMatch
{
	public class ModeScene : SKScene
	{
		public SKSpriteNode _Background;

		private Button _NormalModeButton;
		private Button _ChallengeModeButton;
		private Button _BackButton;
		private NSUserDefaults _CrossSceneData;
		public ModeScene(CGSize size) : base(size)
		{
			
		}

		public override void DidMoveToView(SKView view)
		{
			_CrossSceneData = new NSUserDefaults();

			_Background = new SKSpriteNode(SKTexture.FromImageNamed("ModeSelectScene"));
			_Background.AnchorPoint = new CGPoint(0, 0);
			_Background.Size = new CGSize(this.Size.Width, this.Size.Height);
			_Background.Position = new CGPoint(0, 0);
			_Background.ZPosition = -1;

			SetUpButtons();

			this.AddChild(_Background);
		}

		public override void TouchesBegan(NSSet touches, UIEvent evt)
		{
			foreach (var touch in touches)
			{
				var location = ((UITouch)touch).LocationInNode(this);
				if (_NormalModeButton.CheckTouchBegin(location))
				{
					_NormalModeButton.SwapTexture();
				}
				if (_ChallengeModeButton.CheckTouchBegin(location))
				{
					_ChallengeModeButton.SwapTexture();
				}
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
				//if the normal button is pressed
				if (_NormalModeButton.CheckTouchRelease(location))
				{
					//set the cross scene data for difficulty to 1
					_CrossSceneData.SetInt(1, "difficulty");
					//present the game scene
					PresentGameScene();
				}
				//if the challenge mode button is pressed
				if (_ChallengeModeButton.CheckTouchRelease(location))
				{
					//set the cross scene data for difficulty to 2
					_CrossSceneData.SetInt(2, "difficulty");
					//Present the game scene
					PresentGameScene();
				}
				//if the back button is pressed go back to the main menu
				if (_BackButton.CheckTouchRelease(location))
				{
					MenuScene _MenuScene = new MenuScene(this.Size);

					this.View.PresentScene(_MenuScene, SKTransition.DoorsCloseVerticalWithDuration(1.0f));
				}
			}
		}

		private void PresentGameScene()
		{
			GameScene _GameScene = new GameScene(this.Size);

			this.View.PresentScene(_GameScene, SKTransition.DoorsOpenHorizontalWithDuration(1.0f));
		}

		public void SetUpButtons()
		{
			_NormalModeButton = new Button();
			_NormalModeButton.Position = new CGPoint(this.Frame.Width / 2.0f, (this.Frame.Height / 3.0f) * 1.7f);
			_NormalModeButton.Size = new CGSize((this.Frame.Width / 3.0f) * 2.0f, this.Frame.Height / 10.0f);
			_NormalModeButton.TextSize = _NormalModeButton.Size.Height / 1.5f;
			_NormalModeButton.Text = "Normal";

			_ChallengeModeButton = new Button();
			_ChallengeModeButton.Position = new CGPoint(this.Frame.Width / 2.0f, (this.Frame.Height / 3.0f) * 1.2f);
			_ChallengeModeButton.Size = new CGSize((this.Frame.Width / 3.0f) * 2.0f, this.Frame.Height / 10.0f);
			_ChallengeModeButton.TextSize = _ChallengeModeButton.Size.Height / 1.5f;
			_ChallengeModeButton.Text = "Challenge";

			_BackButton = new Button();
			_BackButton.Position = new CGPoint(this.Frame.Width / 2.0f, (this.Frame.Height / 3.0f) * 0.7f);
			_BackButton.Size = new CGSize((this.Frame.Width / 3.0f) * 2.0f, this.Frame.Height / 10.0f);
			_BackButton.TextSize = _BackButton.Size.Height / 1.5f;
			_BackButton.Text = "Back";


			this.AddChild(_NormalModeButton);
			this.AddChild(_ChallengeModeButton);
			this.AddChild(_BackButton);
		}
	}
}

