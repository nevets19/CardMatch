using System;

using CoreGraphics;
using Foundation;
using SpriteKit;
using UIKit;

namespace CardMatch
{
	public class Button : SKSpriteNode
	{
		//variables that are to be used in the button class
		private SKTexture _ButtonUpTex;
		private SKTexture _ButtonDownTex;
		private CGPoint _ButtonPos;
		private CGSize _ButtonSize;
		private SKLabelNode _ButtonText;
		private bool _TouchBegin = false;

		public Button()
		{
			base.Init(); //calls the bae class (SKSpriteNode) init function

			//initialize the variables I decalred above with values
			_ButtonUpTex = SKTexture.FromImageNamed("ButtonUp");
			_ButtonDownTex = SKTexture.FromImageNamed("ButtonDown");
			_ButtonText = SKLabelNode.FromFont("Courier");

			this.Texture = _ButtonUpTex; //sets the texture of this (The SKSpriteNode we are inheriting from) to the button up texture
			SetUpLabel(); //call the setupLabel function to set up the labelthat is on the button
		}

		//this method is the same as before except the button size is passed into the constructor
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

		//same as above except now the button size and position are being passed into the constructor
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
		//sets up the label (the button's text) to always be in the midd of the button sprite, and always be on top of the button.  
		//the font color is also set and the label is added as a child of the button
		private void SetUpLabel()
		{
			this.AnchorPoint = new CGPoint(0.5, 0.5);
			_ButtonText.VerticalAlignmentMode = SKLabelVerticalAlignmentMode.Center;
			_ButtonText.Position = new CGPoint(0,0);
			_ButtonText.ZPosition = 2;
			_ButtonText.FontColor = UIColor.White;
			this.AddChild(_ButtonText);
		}

		//setting/getter property for the button text
		public string Text
		{
			get { return _ButtonText.Text;}
			set { _ButtonText.Text = value;}
		}

		//setter for the button's text size
		public nfloat TextSize
		{
			set { _ButtonText.FontSize = value;}
		}

		//setter for the buttons text color
		public UIColor TextColor
		{
			set { _ButtonText.FontColor = value;}
		}

		//checks if the button was touched, returns true if it as or flase if it wasn't
		public bool CheckTouchBegin(CGPoint pressPos)
		{
			if (this.ContainsPoint(pressPos))
			{
				_TouchBegin = true;
				return true;
			}
			else {
				_TouchBegin = false;
				return false;
			}
		}

		//checks if a touch was released on the button, returns true if it was false if it wasn't
		public bool CheckTouchRelease(CGPoint pressPos)
		{
			if (this.ContainsPoint(pressPos) && _TouchBegin == true)
			{
				return true;
			}
			else {
				this.Texture = _ButtonUpTex;
				return false;
			}
		}

		//checks the current texture and sets it to the opposite texture
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

