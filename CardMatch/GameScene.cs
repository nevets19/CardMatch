using System;
using System.Collections.Generic;

using CoreGraphics;
using Foundation;
using SpriteKit;
using UIKit;

namespace CardMatch
{
	public class GameScene : SKScene
	{
		private List<Card> _Cards;
		private int _Difficulty = 1; //Easy by default
		private int _NumOfCards = 0;
		Random _Rnd = new Random();

		public GameScene(CGSize size) : base(size)
		{

		}

		public override void DidMoveToView(SKView view)
		{

			_Cards = new List<Card>();
			SetUpCards();
			Shuffle(_Cards);
			DrawCards();
			foreach (Card c in _Cards)
			{
				this.AddChild(c);
			}
		}

		public override void TouchesBegan(NSSet touches, UIEvent evt)
		{
			foreach (var touch in touches)
			{
				var location = ((UITouch)touch).LocationInNode(this);

				foreach (Card c in _Cards)
				{
					if (c.CheckTouchBegin(location) == true && c.HasActions == false)
					{
						c.Flip();
					}
				}
			}
		}

		public override void TouchesEnded(NSSet touches, UIEvent evt)
		{

		}

		private void SetUpCards()
		{
			int _pairInteger = 0;

			switch (_Difficulty)
			{
				case 1:
					_NumOfCards = 24;
					break;
				case 2:
					_NumOfCards = 36;
					break;
			}

			for (int i = 0; i < _NumOfCards/2; i++)
			{
				SKTexture _pairTexture = SKTexture.FromImageNamed("");

				_pairInteger = _Rnd.Next(0, 9);
				switch (_pairInteger)
				{
					case 1:
						_pairTexture = SKTexture.FromImageNamed("CircleCard");
						break;
					case 2:
						_pairTexture = SKTexture.FromImageNamed("SquareCard");
						break;
					case 3:
						_pairTexture = SKTexture.FromImageNamed("TriangleCard");
						break;
					case 4:
						_pairTexture = SKTexture.FromImageNamed("DiamondCard");
						break;
					case 5:
						_pairTexture = SKTexture.FromImageNamed("PentagonCard");
						break;
					case 6:
						_pairTexture = SKTexture.FromImageNamed("DownArrowCard");
						break;
					case 7:
						_pairTexture = SKTexture.FromImageNamed("StarCard");
						break;
					case 8:
						_pairTexture = SKTexture.FromImageNamed("LeftArrowCard");
						break;
				}

				Card c1 = new Card(_pairTexture, _pairInteger);
				Card c2 = new Card(_pairTexture, _pairInteger);

				c1.Size = new CGSize(40, 40);
				c2.Size = new CGSize(40, 40);

				_Cards.Add(c1);
				_Cards.Add(c2);
			}
		}

		private void DrawCards()
		{
			int index = 0;
			if (_NumOfCards == 24)
			{
				for (int i = 1; i <= 6; i++)
				{
					for (int j = 1; j <= 4; j++)
					{
						_Cards[index].Position = new CGPoint(j * 60, i * 60);
						index++;
					}
				}
			}
			else {
				for (int i = 1; i <= Math.Sqrt(_NumOfCards); i++)
				{
					for (int j = 1; j <= Math.Sqrt(_NumOfCards); j++)
					{
						_Cards[index].Position = new CGPoint(j * 60, i * 60);
						index++;
					}
				}
			}
		}

		private void Shuffle<T>(IList<T> list)
		{
			int n = list.Count;
			Console.WriteLine(list.Count);
			while (n > 0)
			{
				int k = (_Rnd.Next(0, n) % n);
				n--;

				T value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
		}
	}
}


