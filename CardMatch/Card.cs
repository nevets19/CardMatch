using System;

using CoreGraphics;
using Foundation;
using SpriteKit;
using UIKit;

namespace CardMatch
{
	public class Card : SKSpriteNode
	{
		//Declare the variables used in the class
		private SKTexture _PairTexture;
		private SKTexture _CardBackTexture;
		private int _PairInteger;
		private bool _Flipped = false;

		private float _FlipSpeed = 0.2f;

		//constructor takes in an SKtexture and an integer
		public Card(SKTexture pairTex, int pairInteger)
		{
			base.Init();
			//the back of the card texture never changes
			_CardBackTexture = SKTexture.FromImageNamed("CardBack");

			this.Texture = _CardBackTexture; //sets the current texture to the back of the card

			_PairInteger = pairInteger; //sets the pair integer to the one that was entered.  This is used for determining if two
			//flipped cards are a pair or not
			_PairTexture = pairTex;
			//this is the image the players sees when a card is flipped
		}

		//getter and setter for pair number
		public int PairNumber
		{
			get { return _PairInteger; }
			set { _PairInteger = value; }
		}

		//returns true if a touch began on the card
		public bool CheckTouchBegin(CGPoint pressPos)
		{
			return this.ContainsPoint(pressPos);
		}

		//returns true if a touch was released on that card
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

		//function to create the illusion of the card flipping
		public void Flip()
		{
			SKAction _NegativeScale = SKAction.ScaleXTo(-1, _FlipSpeed); // scales the card's X to -1
			SKAction _ZeroScale = SKAction.ScaleXTo(0, _FlipSpeed); //scales the card's X to 0
			SKAction _PositiveScale = SKAction.ScaleXTo(1, _FlipSpeed); //scales the card's X to 1

			if (this.Texture != _PairTexture) //if the card isn't flipped
			{
				this.RunAction(_ZeroScale, completionHandler: () => //scale the card to 0
				{
					this.Texture = _PairTexture; //change the card's texture once action is complete
					this.RunAction(_NegativeScale); //scale the card to -1 once the 0 scale action is complete
					Flipped = true;
				});
			}
			else { //if the card is already flipped (now its time to flip back)
				this.RunAction(_ZeroScale, completionHandler: () => //scale the cad to 0
				{
					this.Texture = SKTexture.FromImageNamed("CardBack"); //set the card's texture to the card back once scaling is complete
					this.RunAction(_PositiveScale); //run the positive scale action
					Flipped = false;
				});
			}
		}

		//getter nd setter method for checking if the card is flipped or not
		public bool Flipped
		{
			get { return _Flipped;}
			set { _Flipped = value;}
		}

	}
}

