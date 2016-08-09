using System;

using CoreGraphics;
using Foundation;
using SpriteKit;
using UIKit;

namespace CardMatch
{
	public class DropDownMenu : SKSpriteNode
	{
		private SKTexture _MenuTex;
		private CGPoint _OffScene;
		private bool _InScene;
		public DropDownMenu()
		{
			base.Init();
			_MenuTex = SKTexture.FromImageNamed("Panel");
			_InScene = false;
			this.Texture = _MenuTex;

		}

		public void MoveIntoScene(CGPoint location, double speed)
		{
			_OffScene = Position;
			SKAction _MoveIntoScene = SKAction.MoveTo(location, speed);
			_InScene = true;
			this.RunAction(_MoveIntoScene);
		}

		public void MoveOutOfScene(double speed)
		{
			
			SKAction _MoveOutOfScene = SKAction.MoveTo(_OffScene, speed);
			_InScene = false;
			this.RunAction(_MoveOutOfScene);
		}

		public bool IsInScene
		{
			get
			{
				return _InScene;
			}
		}
	}
}

