using System;

using CoreGraphics;
using Foundation;
using SpriteKit;
using UIKit;

namespace CardMatch
{
	public class Card : SKSpriteNode
	{
		private SKTexture _PairTexture;
		private SKTexture _CardBackTexture;
		private int _PairInteger;
		private bool _Flipped = false;

		private float _FlipSpeed = 0.2f;

		public Card(SKTexture pairTex, int pairInteger)
		{
			base.Init();
			_CardBackTexture = SKTexture.FromImageNamed("CardBack");

			this.Texture = _CardBackTexture;
			_PairInteger = pairInteger;
			_PairTexture = pairTex;
		}

		public int PairNumber
		{
			get { return _PairInteger; }
			set { _PairInteger = value; }
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
				this.Texture = _CardBackTexture;
				return false;
			}
		}


		public void Flip()
		{
			SKAction _NegativeScale = SKAction.ScaleXTo(-1, _FlipSpeed);
			SKAction _ZeroScale = SKAction.ScaleXTo(0, _FlipSpeed);
			SKAction _PositiveScale = SKAction.ScaleXTo(1, _FlipSpeed);

			if (this.Texture != _PairTexture)
			{
				this.RunAction(_ZeroScale, completionHandler: () =>
				{
					this.Texture = _PairTexture;
					this.RunAction(_NegativeScale);
					Flipped = true;
				});
			}
			else {
				this.RunAction(_ZeroScale, completionHandler: () =>
				{
					this.Texture = SKTexture.FromImageNamed("CardBack");
					this.RunAction(_PositiveScale);
					Flipped = false;
				});
			}
		}


		public bool Flipped
		{
			get { return _Flipped;}
			set { _Flipped = value;}
		}

	}
}

