using System;

using CoreGraphics;
using Foundation;
using SpriteKit;
using UIKit;

namespace CardMatch
{
	public class DropDownMenu : SKSpriteNode
	{
		//declare variables used in this class

		private SKTexture _MenuTex;
		private CGPoint _OffScene;
		private bool _InScene;

		public DropDownMenu()
		{
			base.Init();
			_MenuTex = SKTexture.FromImageNamed("Panel"); //sets the menu's texture to that of the panel
			_InScene = false; //set the inScene boolean to false
			this.Texture = _MenuTex; //set the texture of this to the menu texture

		}

		//pass in a speed and a position
		public void MoveIntoScene(CGPoint location, double speed)
		{
			_OffScene = Position; //offScene = the current position of the menu, so it knows where to move back to when it closes
			SKAction _MoveIntoScene = SKAction.MoveTo(location, speed); //create an action to move the menu into the scene
			_InScene = true;
			this.RunAction(_MoveIntoScene); //run the action
		}

		public void MoveOutOfScene(double speed)
		{
			//same as the method above expect the location the menu moves to is offScene which is the variable wwe set above.
			SKAction _MoveOutOfScene = SKAction.MoveTo(_OffScene, speed);
			_InScene = false;
			this.RunAction(_MoveOutOfScene);
		}

		//getter property to check if the menu is in the scene
		public bool IsInScene
		{
			get
			{
				return _InScene;
			}
		}
	}
}

