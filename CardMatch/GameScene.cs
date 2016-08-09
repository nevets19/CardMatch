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

		private Button _RestartButton;
		private Button _PauseButton;
		private Button _MenuButton;
		private Button _ResumeButton;


		private SKLabelNode _ScoreLabel;
		private SKLabelNode _TitleLabel;
		private SKLabelNode _MenuTitle;
		private SKLabelNode _MenuScoreLabel;

		private int _FlippedCards = 0;
		private int _Difficulty = 2; //Normal by default
		private int _NumOfCards = 24;
		private int _DuplicateCardAllowance = 4;
		private int _Flips = 0;
		private int _Time = 0;

		private SKSpriteNode _Background;

		private Timer _OneSecTimer;

		private NSUserDefaults _CrossSceneData;

		private Random _Rnd = new Random();

		private DropDownMenu _DropDownMenu;

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


			_PauseButton = new Button(new CGPoint(this.Size.Width / 2, (this.Size.Height / 10) * 8), new CGSize(90,25));
			_PauseButton.Text = "Pause";
			_PauseButton.TextSize = 18.0f;

			_ScoreLabel = new SKLabelNode("Courier");
			_ScoreLabel.Position = new CGPoint(this.Size.Width/2, (this.Size.Height / 10) * 8.5f);
			_ScoreLabel.FontSize = 18.0f;
			_ScoreLabel.ZPosition = 3;
			_ScoreLabel.VerticalAlignmentMode = SKLabelVerticalAlignmentMode.Center;

			_TitleLabel = new SKLabelNode("Courier");
			_TitleLabel.Position = new CGPoint(this.Size.Width / 2, (this.Size.Height / 10) * 9.0f);
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

			CreateMenuButtons();
			SetUpPauseMenu();
			SetUpCards();
			DrawCards();

			this.Add(_TitleLabel);
			this.Add(_ScoreLabel);
			this.Add(_PauseButton);
			this.Add(_Background);

			//foreach (Card c in _Cards)
			//{
			//	c.Flip();
			//}
		}

		public override void TouchesBegan(NSSet touches, UIEvent evt)
		{
			foreach (var touch in touches)
			{
				var location = ((UITouch)touch).LocationInNode(this);
				var menuLocation = ((UITouch)touch).LocationInNode(_DropDownMenu);
				if (_PauseButton.CheckTouchBegin(location))
				{
					_PauseButton.SwapTexture();
				}
				if (_ResumeButton.CheckTouchBegin(menuLocation))
				{
					_ResumeButton.SwapTexture();
				}
				if (_RestartButton.CheckTouchBegin(menuLocation))
				{
					_RestartButton.SwapTexture();
				}
				if (_MenuButton.CheckTouchBegin(menuLocation))
				{
					_MenuButton.SwapTexture();
				}
			}
		}

		public override void TouchesEnded(NSSet touches, UIEvent evt)
		{
			foreach (var touch in touches)
			{
				var location = ((UITouch)touch).LocationInNode(this);
				var menuLocation = ((UITouch)touch).LocationInNode(_DropDownMenu);
				foreach (Card c in _Cards)
				{
					if (c.CheckTouchBegin(location) == true && c.HasActions == false && c.Flipped == false && _FlippedCardTwo == null && _DropDownMenu.IsInScene == false)
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
				if (_PauseButton.CheckTouchRelease(location))
				{
					_DropDownMenu.MoveIntoScene(new CGPoint(this.Frame.Width / 2, this.Frame.Height / 2), 0.7f);
				}
				if (_ResumeButton.CheckTouchRelease(menuLocation))
				{
					_DropDownMenu.MoveOutOfScene(0.7f);
				}
				if (_RestartButton.CheckTouchRelease(menuLocation))
				{
					_RestartButton.SwapTexture();
					_DropDownMenu.MoveOutOfScene(0.7f);
					SetUpCards();
					DrawCards();
					SetUpPauseMenu();
				}
				if (_MenuButton.CheckTouchRelease(menuLocation))
				{
					MenuScene _MenuScene = new MenuScene(this.Size);

					this.View.PresentScene(_MenuScene, SKTransition.DoorsCloseHorizontalWithDuration(1.0f));
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

			if (_Cards.Count == 0 && _DropDownMenu.IsInScene == false)
			{

				SetUpGameFinishedMenu();
				_DropDownMenu.MoveIntoScene(new CGPoint(this.Frame.Width / 2, this.Frame.Height / 2),0.7f);
			}
		}

		private void IncrementTime(object o, EventArgs e)
		{
			if(_DropDownMenu.IsInScene == false)
				_Time++;
		}

		private void CreateMenuButtons()
		{
			_DropDownMenu = new DropDownMenu();
			_DropDownMenu.Size = new CGSize(250, 400);
			_DropDownMenu.Position = new CGPoint(this.Frame.Width / 2, this.Frame.Height + _DropDownMenu.Frame.Height);
			_DropDownMenu.ZPosition = 10;
			_DropDownMenu.AnchorPoint = new CGPoint(0.5f, 0.5f);


			_MenuTitle = new SKLabelNode("Courier");
			_MenuTitle.FontSize = 30.0f;
			_MenuTitle.Position = new CGPoint(0, 150);
			_MenuTitle.ZPosition = 11;
			_MenuTitle.VerticalAlignmentMode = SKLabelVerticalAlignmentMode.Center;
			_MenuTitle.FontColor = UIColor.White;


			_MenuScoreLabel = new SKLabelNode("Courier");
			_MenuScoreLabel.FontSize = 30.0f;
			_MenuScoreLabel.Position = new CGPoint(0, 100);
			_MenuScoreLabel.ZPosition = 11;
			_MenuScoreLabel.VerticalAlignmentMode = SKLabelVerticalAlignmentMode.Center;
			_MenuScoreLabel.FontColor = UIColor.White;

			_RestartButton = new Button();
			_RestartButton.Position = new CGPoint(0, 0);
			_RestartButton.Size = new CGSize((_DropDownMenu.Frame.Width / 3) * 2, _DropDownMenu.Frame.Height / 9);
			_RestartButton.Text = "Restart";
			_RestartButton.TextSize = 18.0f;
			_RestartButton.AnchorPoint = new CGPoint(0.5, 0.5);
			_RestartButton.ZPosition = 11;

			_ResumeButton = new Button();
			_ResumeButton.Position = new CGPoint(0, 100);
			_ResumeButton.Size = new CGSize((_DropDownMenu.Frame.Width / 3) * 2, _DropDownMenu.Frame.Height / 9);
			_ResumeButton.Text = "Resume";
			_ResumeButton.TextSize = 18.0f;
			_ResumeButton.AnchorPoint = new CGPoint(0.5, 0.5);
			_ResumeButton.ZPosition = 11;

			_MenuButton = new Button();
			_MenuButton.Position = new CGPoint(0, -100);
			_MenuButton.Size = new CGSize((_DropDownMenu.Frame.Width / 3) * 2, _DropDownMenu.Frame.Height / 9);
			_MenuButton.Text = "Menu";
			_MenuButton.TextSize = 18.0f;
			_MenuButton.AnchorPoint = new CGPoint(0.5, 0.5);
			_MenuButton.ZPosition = 11;

			this.AddChild(_DropDownMenu);
		}

		private void SetUpPauseMenu()
		{

			_MenuTitle.Text = "Paused";
			_DropDownMenu.RemoveAllChildren();
			_DropDownMenu.AddChild(_ResumeButton);
			_DropDownMenu.AddChild(_RestartButton);
			_DropDownMenu.AddChild(_MenuButton);
			_DropDownMenu.AddChild(_MenuTitle);
		}

		private void SetUpGameFinishedMenu()
		{
			_MenuTitle.Text = "You Win!";
			if (_Difficulty == 1)
			{
				_MenuScoreLabel.Text = "Flips:" + _Flips.ToString();
			}
			else {
				_MenuScoreLabel.Text = "Time:" + _Time.ToString();
			}


			_DropDownMenu.RemoveAllChildren();
			_DropDownMenu.AddChild(_RestartButton);
			_DropDownMenu.AddChild(_MenuButton);
			_DropDownMenu.AddChild(_MenuTitle);
			_DropDownMenu.AddChild(_MenuScoreLabel);
		}
	}
}