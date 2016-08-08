using System;
using CoreGraphics;
using Foundation;
using SpriteKit;
using UIKit;

namespace CardMatch
{


	public class AboutScene : SKScene
	{
		public SKSpriteNode _Background;

		public AboutScene(CGSize size) : base(size)
		{

		}

		public override void DidMoveToView(SKView view)
		{
			_Background = new SKSpriteNode(SKTexture.FromImageNamed("BackGround"));
			_Background.AnchorPoint = new CGPoint(0, 0);
			_Background.Size = new CGSize(this.Size.Width, this.Size.Height);
			_Background.Position = new CGPoint(0, 0);
			_Background.ZPosition = -1;

			this.AddChild(_Background);
		}

		public override void TouchesBegan(NSSet touches, UIEvent evt)
		{
			
		}

		public override void TouchesEnded(NSSet touches, UIEvent evt)
		{
	
		}
	}
}

