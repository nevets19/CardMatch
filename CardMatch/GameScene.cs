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

		//decalre variables used in this class
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

		//All this happens when the scene becomes active
		public override void DidMoveToView(SKView view)
		{
			
			_CrossSceneData = new NSUserDefaults(); //Set the cross scene data to a new NSUserDefaults object.

			//create a timer, this is used for Chalenge mode
			_OneSecTimer = new Timer(1000);
			_OneSecTimer.Elapsed += IncrementTime;
			_OneSecTimer.Start();

			//create and setup a background image, much like in the other scenes
			_Background = new SKSpriteNode(SKTexture.FromImageNamed("BackGround"));
			_Background.AnchorPoint = new CGPoint(0, 0);
			_Background.Size = new CGSize(this.Size.Width, this.Size.Height);
			_Background.Position = new CGPoint(0, 0);
			_Background.ZPosition = -1;

			//creates a new list for the cards
			_Cards = new List<Card>();

			//create the pause button, this includes location, size, text and text size
			_PauseButton = new Button(new CGPoint(this.Size.Width / 2, (this.Size.Height / 10) * 8), new CGSize(90,25));
			_PauseButton.Text = "Pause";
			_PauseButton.TextSize = 18.0f;

			//create and position the label that shows the score
			_ScoreLabel = new SKLabelNode("Courier");
			_ScoreLabel.Position = new CGPoint(this.Size.Width/2, (this.Size.Height / 10) * 8.5f);
			_ScoreLabel.FontSize = 18.0f;
			_ScoreLabel.ZPosition = 3;
			_ScoreLabel.VerticalAlignmentMode = SKLabelVerticalAlignmentMode.Center;

			//create and position the label that shows the title (game type)
			_TitleLabel = new SKLabelNode("Courier");
			_TitleLabel.Position = new CGPoint(this.Size.Width / 2, (this.Size.Height / 10) * 9.0f);
			_TitleLabel.FontSize = 28.0f;
			_TitleLabel.ZPosition = 3;

			//if the value stored in the NSUserDefaults is 1 then the mode is Normal Mode
			if (_CrossSceneData.IntForKey("difficulty") == 1)
			{
				_Difficulty = 1;
				_TitleLabel.Text = "Normal Mode";
			}
			else { //if the value is not 1 then it's challenge mode
				_Difficulty = 2;
				_TitleLabel.Text = "Challenge Mode";
			}

			_TitleLabel.VerticalAlignmentMode = SKLabelVerticalAlignmentMode.Center;

			CreateMenuButtons(); //call the create menu buttons function
			SetUpPauseMenu(); //calls the setup pause menu function
			SetUpCards(); //calls the setup cards function
			DrawCards(); //calls the draw cards function

			//ads the stuff created above to the scene
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
				var menuLocation = ((UITouch)touch).LocationInNode(_DropDownMenu); //this is needed for the buttons on the drop down menu

				//if a touch occurs on a button then swap it's texture
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
					//if a card is touched, it has no actions, it's not flipped and the Placeholder for the second flippsed card is null (either none or one card has been flipped) and the menu isn't in the scene
					if (c.CheckTouchBegin(location) == true && c.HasActions == false && c.Flipped == false && _FlippedCardTwo == null && _DropDownMenu.IsInScene == false)
					{
						c.Flip(); //flips the card
						c.Flipped = true; //set flipped to true
						_Flips++; //increament the flipped variable, thisis for the score

						//_FlippedCards++;

						//if this is the first card to be flipped
						if (_FlippedCardOne == null)
						{
							//set the first flipped card variable to the card that was just flipped
							_FlippedCardOne = c;
						}
						else { // this is the second card to be flipped
							//set this card to the second flipped card variable
							_FlippedCardTwo = c;

							CheckMatch(); //call the check match function
						}
					}
				}
				if (_PauseButton.CheckTouchRelease(location))
				{
					//if pause is pressed open drop down menu
					_DropDownMenu.MoveIntoScene(new CGPoint(this.Frame.Width / 2, this.Frame.Height / 2), 0.7f);
				}
				if (_ResumeButton.CheckTouchRelease(menuLocation))
				{
					//if resume is pressed move the menu out of the screen
					_DropDownMenu.MoveOutOfScene(0.7f);
				}
				if (_RestartButton.CheckTouchRelease(menuLocation))
				{
					//if the restart button s pressed
					_RestartButton.SwapTexture();
					_DropDownMenu.MoveOutOfScene(0.7f); //move menu of out screen
					SetUpCards(); //setup the card
					DrawCards(); // draw the cards
					SetUpPauseMenu(); //set up the pause menu
				}
				if (_MenuButton.CheckTouchRelease(menuLocation))
				{
					//if the menu button is pressed
					MenuScene _MenuScene = new MenuScene(this.Size); //create a new menu scene object
					//present the newly created menu scene with doors close horizontal transition effect.
					this.View.PresentScene(_MenuScene, SKTransition.DoorsCloseHorizontalWithDuration(1.0f));
				}
			}
		}

		private void SetUpCards()
		{
			//this smal section is to fix a bug that would occure if someone pressed retart while the cards were in the middle of an action
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

			_Flips = 0; //set the flips score for normal mode to 0
			_Time = 0; //sets the time score of challenge mode to 0

			//sets both of the flipped card placeholders to null
			_FlippedCardOne = null;
			_FlippedCardTwo = null;

			//if there are cards in the list delete them
			if(_Cards.Count > 0)
				DeleteCards();

			int _pairInteger = 0;

			for (int i = 0; i < _NumOfCards/2; i++)
			{
				SKTexture _pairTexture = SKTexture.FromImageNamed(""); //set pair texutre to nothing right now

				if (_Difficulty == 1) //if difficulty is normal mode
				{
					GenerateCardInteger(ref _pairInteger); //generate a random integer for the card

					//if there is too many of one card in the list (more than the duplicate card allowance)
					if (CheckDuplicates(_pairInteger) >= _DuplicateCardAllowance)
					{
						int reroll = _Rnd.Next(1, 3); //randomise a new number

						//chance to reroll a new card if the new random number is 1 or 2
						if (reroll == 1 || reroll == 2)
						{
							GenerateCardInteger(ref _pairInteger);
						}
					}
				}
				else { //if the mode is challenge mode, increment the pair integer
					_pairInteger++;
				}

				//this switch statement checks what the pair integer is and sets the _pairTexture to an appropriate image
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

				//sets the card size to the height of the screen divided by 9.5
				cardSize = this.Size.Height / 9.5;

				//create two new cards, passing in the pair integer and pair texture
				Card c1 = new Card(_pairTexture, _pairInteger);
				Card c2 = new Card(_pairTexture, _pairInteger);

				//sets the size of the cards to the value created above
				c1.Size = new CGSize(cardSize, cardSize);
				c2.Size = new CGSize(cardSize, cardSize);

				//ads the cards that we just made to the list
				_Cards.Add(c1);
				_Cards.Add(c2);
			}
		}

		private void DeleteCards()
		{
			//remove each card from the parent node and then clear the list
			foreach (Card c in _Cards)
			{
				c.RemoveFromParent();
			}
			_Cards.Clear();
		}

		//generates a random number from 1 - 12.
		private void GenerateCardInteger(ref int pairInt)
		{
			pairInt = _Rnd.Next(1,13);
		}

		//checks and returns how many of one card are in the list
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
			Shuffle(_Cards); //shuffles the cards in the list
			int index = 0;
			double widthPadding;
			double heightPadding;

			if (_NumOfCards == 24)
			{
				widthPadding = this.Size.Width / 5; //create width padding based on screen size
				heightPadding = this.Size.Height / 9; //create height passing based on screen size
				for (int i = 1; i <= 6; i++)
				{
					for (int j = 1; j <= 4; j++)
					{
						_Cards[index].Position = new CGPoint(j * widthPadding, i * heightPadding); //create the cards and position 
						//them based on the passing variables made above
						index++;
					}
				}
			}
			else { //this part is never run as as things are now, there will only be 24 cards.  I made this when i planned to have more cards
				for (int i = 1; i <= Math.Sqrt(_NumOfCards); i++)
				{
					for (int j = 1; j <= Math.Sqrt(_NumOfCards); j++)
					{
						_Cards[index].Position = new CGPoint(j * 60, i * 60);
						index++;
					}
				}
			}
			//adds each card to the parent node
			foreach (Card c in _Cards)
			{
				this.AddChild(c);
			}
		}
		//this function re-arranges the cards in the list, so when they are drawn, the pairs are not next to each other
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

		//checks for a match and waits time before pervormaing the appropriate action
		private void CheckMatch()
		{
			if (_FlippedCardTwo.PairNumber == _FlippedCardOne.PairNumber) // if there is a pair
			{
				SKAction _Wait = SKAction.WaitForDuration(1.0);

				this.RunAction(_Wait, completionHandler: () => //wait for 1 second
				{
					SKAction _Fade = SKAction.FadeAlphaTo(0.0f, 1.0); // ceate a fade action

					_FlippedCardOne.RunAction(_Fade, completionHandler: () => //oncefading is complete
					{
						_Cards.Remove(_FlippedCardOne); //remove the flipped card
						_FlippedCardOne.RemoveFromParent();
						_FlippedCardOne = null;
					});
					_FlippedCardTwo.RunAction(_Fade, completionHandler: () => //same as above pretty much
					{
						_Cards.Remove(_FlippedCardTwo);
						_FlippedCardTwo.RemoveFromParent();
						_FlippedCardTwo = null;
					});
				});
			}
			else {
				//not a match

				SKAction _Wait = SKAction.WaitForDuration(1.5);

				this.RunAction(_Wait, completionHandler: () => //once the wait action is complete
				{
					//flip the cards back
					_FlippedCardOne.Flip(); 
					_FlippedCardTwo.Flip();
					_FlippedBack = false;
				});
			}
		}

		public override void Update(double currentTime)
		{
			//if the cards have not been marked as flipped back and have no actions
			if (_FlippedBack == false && _FlippedCardOne.HasActions == false && _FlippedCardTwo.HasActions == false)
			{
				//set the placeholder cards to null and flipped back to true
				_FlippedCardOne = null;
				_FlippedCardTwo = null;
				_FlippedBack = true;
			}

			if (_Difficulty == 1) //normal mode
			{
				_ScoreLabel.Text = "Flips:  " + _Flips.ToString(); // the score label is set to flips
			}
			else { //challenge mode
				_ScoreLabel.Text = "Time:  " + _Time.ToString(); //the score label is set to time
			}

			if (_Cards.Count == 0 && _DropDownMenu.IsInScene == false) //if there are no cards in the list and the drop down menu isn't in
				//the scene
			{
				SetUpGameFinishedMenu(); //set up the menu to be game finished
				_DropDownMenu.MoveIntoScene(new CGPoint(this.Frame.Width / 2, this.Frame.Height / 2),0.7f); //bring the drop down menu
				//into the scene
			}
		}

		//thisi s called every 1000 ms
		private void IncrementTime(object o, EventArgs e)
		{
			// increment time, unless the drop down menu is in the scene (game paused)
			if(_DropDownMenu.IsInScene == false)
				_Time++;
		}

		//creates, sets size, position and text of everything that is on the drop down menu
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

		//adds the required children to display buttons and text for the game being paused
		private void SetUpPauseMenu()
		{

			_MenuTitle.Text = "Paused";
			_DropDownMenu.RemoveAllChildren();
			_DropDownMenu.AddChild(_ResumeButton);
			_DropDownMenu.AddChild(_RestartButton);
			_DropDownMenu.AddChild(_MenuButton);
			_DropDownMenu.AddChild(_MenuTitle);
		}

		//adds the required children to display the buttons and text for the game being finished
		private void SetUpGameFinishedMenu()
		{
			_MenuTitle.Text = "You Win!";
			//displays flips or time on the drop down menu based on what the active mode is
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