using System;

using CoreGraphics;
using Foundation;
using SpriteKit;
using UIKit;

namespace CardMatch
{
	public class Button : SKSpriteNode
	{
		private SKTexture _ButtonUpTex;
		private SKTexture _ButtonDownTex;
		private CGPoint _ButtonPos;
		private CGSize _ButtonSize;
		private SKLabelNode _ButtonText;

		public Button()
		{
			base.Init();

			_ButtonUpTex = SKTexture.FromImageNamed("ButtonUp");
			_ButtonDownTex = SKTexture.FromImageNamed("ButtonDown");
			_ButtonText = SKLabelNode.FromFont("Courier");
			this.Texture = _ButtonUpTex;
			SetUpLabel();
		}

		public Button(CGSize buttonSize)
		{
			base.Init();

			_ButtonUpTex = SKTexture.FromImageNamed("ButtonUp");
			_ButtonDownTex = SKTexture.FromImageNamed("ButtonDown");
			_ButtonText = SKLabelNode.FromFont("Courier");

			_ButtonSize = buttonSize;

			this.Size = _ButtonSize;
			this.Texture = _ButtonUpTex;
			SetUpLabel();
		}

		public Button(CGPoint buttonPos, CGSize buttonSize)
		{
			base.Init();

			_ButtonUpTex = SKTexture.FromImageNamed("ButtonUp");
			_ButtonDownTex = SKTexture.FromImageNamed("ButtonDown");
			_ButtonText = SKLabelNode.FromFont("Courier");

			_ButtonSize = buttonSize;
			_ButtonPos = buttonPos;

			this.Size = _ButtonSize;
			this.Position = _ButtonPos;
			this.Texture = _ButtonUpTex;
			SetUpLabel();
		}

		private void SetUpLabel()
		{
			this.AnchorPoint = new CGPoint(0.5, 0.5);
			_ButtonText.VerticalAlignmentMode = SKLabelVerticalAlignmentMode.Center;
			_ButtonText.Position = new CGPoint(0,0);
			_ButtonText.ZPosition = 2;
			_ButtonText.FontColor = UIColor.White;
			this.AddChild(_ButtonText);
		}

		public string Text
		{
			get { return _ButtonText.Text;}
			set { _ButtonText.Text = value;}
		}

		public nfloat TextSize
		{
			set { _ButtonText.FontSize = value;}
		}

		public UIColor TextColor
		{
			set { _ButtonText.FontColor = value;}
		}

		public bool CheckTouchBegin(CGPoint pressPos)
		{
			return this.ContainsPoint(pressPos);
		}

		public bool CheckTouchRelease(CGPoint pressPos)
		{
			if (this.ContainsPoint(pressPos))
			{
				return true;
			}
			else {
				this.Texture = _ButtonUpTex;
				return false;
			}
		}

		public void SwapTexture()
		{
			if (this.Texture == _ButtonUpTex)
			{
				this.Texture = _ButtonDownTex;
			}
			else {
				this.Texture = _ButtonUpTex;
			}
		}
	}
}

