using System;
using System.Collections.Generic;

using CoreGraphics;
using Foundation;
using SpriteKit;
using UIKit;
using System.Timers;

namespace CardMatch
{
	public class GameScene : SKScene
	{
		private List<Card> _Cards;

		private Card _FlippedCardOne;
		private Card _FlippedCardTwo;

		private bool _FlippedBack = true;

		private Button _Restart;

		private SKLabelNode _ScoreLabel;
		private SKLabelNode _TitleLabel;

		private int _FlippedCards = 0;
		private int _Difficulty = 2; //Normal by default
		private int _NumOfCards = 24;
		private int _DuplicateCardAllowance = 4;
		private int _Flips = 0;
		private int _Time = 0;

		private SKSpriteNode _Background;

		private Timer _OneSecTimer;

		private NSUserDefaults _CrossSceneData;

		Random _Rnd = new Random();

		public GameScene(CGSize size) : base(size)
		{

		}

		public override void DidMoveToView(SKView view)
		{
			_CrossSceneData = new NSUserDefaults();

			_OneSecTimer = new Timer(1000);
			_OneSecTimer.Elapsed += IncrementTime;
			_OneSecTimer.Start();

			_Background = new SKSpriteNode(SKTexture.FromImageNamed("BackGround"));
			_Background.AnchorPoint = new CGPoint(0, 0);
			_Background.Size = new CGSize(this.Size.Width, this.Size.Height);
			_Background.Position = new CGPoint(0, 0);
			_Background.ZPosition = -1;

			_Cards = new List<Card>();


			_Restart = new Button(new CGPoint(this.Size.Width / 2, (this.Size.Height / 10) * 8), new CGSize(90,25));
			_Restart.Text = "Restart";
			_Restart.TextSize = 18.0f;

			_ScoreLabel = new SKLabelNode("Courier");
			_ScoreLabel.Position = new CGPoint(this.Size.Width/2, (this.Size.Height / 10) * 8.5f);
			_ScoreLabel.FontSize = 18.0f;
			_ScoreLabel.ZPosition = 3;
			_ScoreLabel.VerticalAlignmentMode = SKLabelVerticalAlignmentMode.Center;

			_TitleLabel = new SKLabelNode("Courier");
			_TitleLabel.Position = new CGPoint(this.Size.Width / 2, (this.Size.Height / 10) * 9.5f);
			_TitleLabel.FontSize = 28.0f;
			_TitleLabel.ZPosition = 3;

			if (_CrossSceneData.IntForKey("difficulty") == 1)
			{
				_Difficulty = 1;
				_TitleLabel.Text = "Normal Mode";
			}
			else {
				_Difficulty = 2;
				_TitleLabel.Text = "Challenge Mode";
			}
			_TitleLabel.VerticalAlignmentMode = SKLabelVerticalAlignmentMode.Center;


			SetUpCards();
			DrawCards();

			this.Add(_TitleLabel);
			this.Add(_ScoreLabel);
			this.Add(_Restart);
			this.Add(_Background);

			foreach (Card c in _Cards)
			{
				c.Flip();
			}
		}

		public override void TouchesBegan(NSSet touches, UIEvent evt)
		{
			foreach (var touch in touches)
			{
				var location = ((UITouch)touch).LocationInNode(this);
				if (_Restart.CheckTouchBegin(location))
				{
					_Restart.SwapTexture();
				}

			}
		}

		public override void TouchesEnded(NSSet touches, UIEvent evt)
		{
			foreach (var touch in touches)
			{
				var location = ((UITouch)touch).LocationInNode(this);

				foreach (Card c in _Cards)
				{
					if (c.CheckTouchBegin(location) == true && c.HasActions == false && c.Flipped == false && _FlippedCardTwo == null)
					{
						c.Flip();
						c.Flipped = true;
						_Flips++;

						_FlippedCards++;

						if (_FlippedCardOne == null)
						{
							_FlippedCardOne = c;
						}
						else {
							_FlippedCardTwo = c;
							CheckMatch();
						}
					}
				}
				if (_Restart.CheckTouchRelease(location))
				{
					_Restart.SwapTexture();
					SetUpCards();
					DrawCards();
					foreach (Card c in _Cards)
					{
						c.Flip();
					}
				}
			}
		}

		private void SetUpCards()
		{
			if (_FlippedCardOne != null)
			{
				_FlippedBack = true;
				_FlippedCardOne.RemoveAllActions();
			}
			if (_FlippedCardTwo != null)
			{
				_FlippedCardTwo.RemoveAllActions();
			}


			double cardSize;

			_Flips = 0;
			_Time = 0;

			_FlippedCardOne = null;
			_FlippedCardTwo = null;

			if(_Cards.Count > 0)
				DeleteCards();

			int _pairInteger = 0;

			for (int i = 0; i < _NumOfCards/2; i++)
			{
				SKTexture _pairTexture = SKTexture.FromImageNamed("");

				if (_Difficulty == 1)
				{
					GenerateCardInteger(ref _pairInteger);

					//if there is too many of one card in the list (more than the duplicate card allowance)
					if (CheckDuplicates(_pairInteger) >= _DuplicateCardAllowance)
					{
						int reroll = _Rnd.Next(1, 3);

						//chance to reroll a new card
						if (reroll == 1 || reroll == 2)
						{
							GenerateCardInteger(ref _pairInteger);
						}
					}
				}
				else {
					_pairInteger++;
				}


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
					case 9:
						_pairTexture = SKTexture.FromImageNamed("UpArrowCard");
						break;
					case 10:
						_pairTexture = SKTexture.FromImageNamed("RightArrowCard");
						break;
					case 11:
						_pairTexture = SKTexture.FromImageNamed("SmileFaceCard");
						break;
					case 12:
						_pairTexture = SKTexture.FromImageNamed("AngryFaceCard");
						break;
				}

				cardSize = this.Size.Height / 9.5;


				Card c1 = new Card(_pairTexture, _pairInteger);
				Card c2 = new Card(_pairTexture, _pairInteger);

				c1.Size = new CGSize(cardSize, cardSize);
				c2.Size = new CGSize(cardSize, cardSize);

				_Cards.Add(c1);
				_Cards.Add(c2);
			}
		}

		private void DeleteCards()
		{
			foreach (Card c in _Cards)
			{
				c.RemoveFromParent();
			}
			_Cards.Clear();
		}

		private void GenerateCardInteger(ref int pairInt)
		{
			pairInt = _Rnd.Next(1,13);
		}


		private int CheckDuplicates(int pairInt)
		{
			int count = 0;
			foreach (Card c in _Cards)
			{
				if (c.PairNumber == pairInt)
				{
					count++;
				}
			}
			return count;
		}

		private void DrawCards()
		{
			Shuffle(_Cards);
			int index = 0;
			double widthPadding;
			double heightPadding;

			if (_NumOfCards == 24)
			{
				widthPadding = this.Size.Width / 5;
				heightPadding = this.Size.Height / 9;
				for (int i = 1; i <= 6; i++)
				{
					for (int j = 1; j <= 4; j++)
					{
						_Cards[index].Position = new CGPoint(j * widthPadding, i * heightPadding);
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
			foreach (Card c in _Cards)
			{
				this.AddChild(c);
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

		private void CheckMatch()
		{
			if (_FlippedCardTwo.PairNumber == _FlippedCardOne.PairNumber)
			{
				SKAction _Wait = SKAction.WaitForDuration(1.5);

				this.RunAction(_Wait, completionHandler: () =>
				{
					SKAction _Fade = SKAction.FadeAlphaTo(0.0f, 1.0);

					_FlippedCardOne.RunAction(_Fade, completionHandler: () =>
					{
						_Cards.Remove(_FlippedCardOne);
						_FlippedCardOne.RemoveFromParent();
						_FlippedCardOne = null;
					});
					_FlippedCardTwo.RunAction(_Fade, completionHandler: () =>
					{
						_Cards.Remove(_FlippedCardTwo);
						_FlippedCardTwo.RemoveFromParent();
						_FlippedCardTwo = null;
					});
				});
			}
			else {
				//not a match

				SKAction _Wait = SKAction.WaitForDuration(2.0);

				this.RunAction(_Wait, completionHandler: () =>
				{
					_FlippedCardOne.Flip();
					_FlippedCardTwo.Flip();
					_FlippedBack = false;
				});
			}
		}

		public override void Update(double currentTime)
		{
			if (_FlippedBack == false && _FlippedCardOne.HasActions == false && _FlippedCardTwo.HasActions == false)
			{
				_FlippedCardOne = null;
				_FlippedCardTwo = null;
				_FlippedBack = true;
			}

			if (_Difficulty == 1)
			{
				_ScoreLabel.Text = "Flips:  " + _Flips.ToString();
			}
			else {
				_ScoreLabel.Text = "Time:  " + _Time.ToString();
			}
		}

		private void IncrementTime(object o, EventArgs e)
		{
			_Time++;
		}
	}
}