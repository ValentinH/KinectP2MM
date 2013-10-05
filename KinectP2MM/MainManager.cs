using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect.Toolkit.Interaction;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace KinectP2MM
{
    class MainManager
    {
        //main window
        private MainWindow window;

        //kinect manager
        private KinectManager kinectManager;

        // our two hands
        private Tuple<Hand, Hand> hands;

        //list of words to manipulate
        private Sequence sequence;

        //list of words couple split
        private List<Tuple<Word, Word>> splitWordsCouple;

        //private Bin bin;

        public MainManager(MainWindow window)
        {
            this.window = window;
            this.hands = new Tuple<Hand, Hand>(this.window.leftCursor, this.window.rightCursor);
            this.hands.Item1.path = "images/left_cursor";
            this.hands.Item2.path = "images/right_cursor";

            sequence = new Sequence();                                    

            //has to be done at last
            this.kinectManager = new KinectManager(this, this.window.sensorChooserUi);
        }

        public void loadSequence(Sequence sequence)
        {
            this.sequence = sequence;
            resetWordsOnCanvas();

            //add words to canvas
            foreach (var word in this.sequence.words)
            {
                this.window.canvas.Children.Add(word);
            }

            splitWordsCouple = new List<Tuple<Word, Word>>();
        }

        private void resetWordsOnCanvas()
        {
            List<Word> wordsToDelete = new List<Word>();
            foreach (var word in this.window.canvas.Children.OfType<Word>())
            {
                wordsToDelete.Add(word);
            }
            foreach (var word in wordsToDelete)
            {
                this.window.canvas.Children.Remove(word);
            }
        }

        public void newImageReceived(UserInfo userInfo)
        {
            var hands = userInfo.HandPointers;
            foreach (var hand in hands)
            {
                updateHand(hand);
            }

            interactionOnText();
        }

        //function to update the model of the hand and to draw the cursor corresponding to the given hand
        private void updateHand(InteractionHandPointer h)
        {
            //current Hand
            Hand hand = h.HandType == InteractionHandType.Left ? hands.Item1 : hands.Item2;

            //update the model of the hand
            hand.x = h.X * window.canvas.ActualWidth;
            hand.y = h.Y * window.canvas.ActualHeight;

            //check if the hand is grip
            if (!hand.grip && h.HandEventType == InteractionHandEventType.Grip)
            {
                hand.grip = true;
            }
            else
                if (hand.grip && h.HandEventType == InteractionHandEventType.GripRelease)
                {
                    hand.grip = false;
                    hand.attachedObjectId = Guid.Empty;
                }

            //check if the hand is pressed
            if (h.PressExtent > 1 && !hand.pressed)
            {
                hand.pressed = true;
            }
            else
                if (h.PressExtent == 0 && hand.pressed)
                {
                    hand.pressed = false;
                }

            //set the just grip or just release value
            if (h.HandEventType == InteractionHandEventType.Grip && hand.lastEvent != "Grip")
            {
                hand.justGrip = true;
                Hand.distance = ImageTools.getDistance(hands.Item1, hands.Item2);
            }
            else
                hand.justGrip = false;

            //set just release value
            if (h.HandEventType == InteractionHandEventType.GripRelease && hand.lastEvent != "GripRelease")
            {
                hand.justReleased = true;
                Hand.distance = ImageTools.getDistance(hands.Item1, hands.Item2);
            }
            else
                hand.justReleased = false;

            //update of the lastHandEvent
            switch (h.HandEventType)
            {
                case InteractionHandEventType.Grip:
                    hand.lastEvent = "Grip";
                    break;
                case InteractionHandEventType.GripRelease:
                    hand.lastEvent = "GripRelease";
                    break;
                case InteractionHandEventType.None:
                    hand.lastEvent = "None";
                    break;
            }
        }

        
        private void interactionOnText()
        {
            //bin.hover = false;
            reinitialize(splitWordsCouple);

            foreach (var word in this.sequence.words.ToList())
            {
                word.hover = false;
                moveDetectionWord(word, InteractionHandType.Left);
                moveDetectionWord(word, InteractionHandType.Right);
            }

            foreach (var word in this.sequence.words.ToList())
            {                //if the left hand is on the word
                if (hands.Item1.grip && hands.Item1.attachedObjectId == word.id)
                {
                    fusionDetection(word, hands.Item1);
                }
                //if the right hand is on the word
                else if (hands.Item2.grip && hands.Item2.attachedObjectId == word.id)
                {
                    fusionDetection(word, hands.Item2);
                }   
            }

            foreach (var word in this.sequence.words.ToList())
            {                
                //if the left hand is on the word
                if (hands.Item1.grip && hands.Item1.attachedObjectId == word.id)
                {
                    separationDetection(word, hands.Item1, hands.Item2);
                }
                //if the right hand is on the word
                else if (hands.Item2.grip && hands.Item2.attachedObjectId == word.id)
                {
                    separationDetection(word, hands.Item2, hands.Item1);
                }   
            }

            foreach (var word in this.sequence.words.ToList())
            {               
                //if the left hand is on the word
                if (hands.Item1.grip && hands.Item1.attachedObjectId == word.id)
                {
                   zoomDetection(word, hands.Item2);
                   rotationDetection(word, hands.Item2);
                }
                //if the right hand is on the word
                else if (hands.Item2.grip && hands.Item2.attachedObjectId == word.id)
                {
                    zoomDetection(word, hands.Item1);
                    rotationDetection(word, hands.Item1);
                }  
            }
            
        }

        private void reinitialize(List<Tuple<Word, Word>> couplesList)
        {
            if (couplesList == null)
                return;
            foreach (var couple in couplesList.ToList())
            {
                if (ImageTools.getDistance(couple.Item1, couple.Item2) > 20000)
                {
                    couplesList.Remove(couple);
                }
            }
        }

        private void moveDetectionWord(Word word, InteractionHandType which)
        {
            var hand = which == InteractionHandType.Left ? hands.Item1 : hands.Item2;
            var secondHand = which == InteractionHandType.Right ? hands.Item1 : hands.Item2;

            //if the hand is on the text
            if (ImageTools.isOn(hand, word))
            {
                //detection of grip
                if (hand.justGrip)
                {
                    //we attach the label to the hand until the grip is released
                    hand.attachedObjectId = word.id;

                    //we savethe current rotation of the word
                    double rotationInDegrees = 0;
                    RotateTransform rotation = word.RenderTransform as RotateTransform;
                    if (rotation != null) // Make sure the transform is actually a RotateTransform
                    {
                        rotationInDegrees = rotation.Angle;
                    }
                    word.beginRotation = rotationInDegrees;
                }
                word.hover = true;
            }

            //moving of the attached text
            if (hand.grip && !hand.pressed && hand.attachedObjectId == word.id)
            {
                //this is to keep the words inside the canvas
                Point newPos = new Point(hand.x + hand.ActualWidth / 2 - word.ActualWidth / 2, hand.y + hand.ActualHeight / 2 - word.ActualHeight / 2);
                if (newPos.X > 0 && newPos.X - word.ActualWidth / 2 < this.window.canvas.ActualWidth - word.ActualWidth && newPos.Y > 0 && newPos.Y < this.window.canvas.ActualHeight - word.ActualHeight)
                {
                    word.x = newPos.X;
                    word.y = newPos.Y;
                    word.hover = true;
                }
                else if (newPos.X > 0 && newPos.X - word.ActualWidth / 2 < this.window.canvas.ActualWidth - word.ActualWidth)
                {
                    word.x = newPos.X;
                }
                else if (newPos.Y > 0 && newPos.Y < this.window.canvas.ActualHeight - word.ActualHeight)
                {
                    word.y = newPos.Y;
                }
            }
        }

        // method to manage to zoom detection
        private void zoomDetection(Word word, Hand secondHand)
        {
            if (!this.sequence.canZoom) return;
            //zoom if second hand gripping and without a text attached
            if (secondHand.grip && secondHand.attachedObjectId == Guid.Empty)
            {
                var distance = ImageTools.getDistance(hands.Item1, hands.Item2);
                var difference = distance - Hand.distance;
                var facteur = difference / 1000000 + 1;
                
                if (difference > 5000)
                {
                    word.applyZoom(facteur);                   
                }
                else
                    if (difference < -5000)
                    {
                        word.applyZoom(facteur);
                    }
                Hand.distance = distance;
            }
        }

        // method to manage to rotate detection
        private void rotationDetection(Word word, Hand secondHand)
        {
            if (!this.sequence.canRotate) return;
            //rotate if second hand gripping and without a text attached
            if (secondHand.grip && secondHand.attachedObjectId == Guid.Empty)
            {
                double wordRotation = word.beginRotation;

                var newRotation = (wordRotation - ImageTools.getRotation(hands.Item1, hands.Item2)) % 360;
                word.RenderTransform = new RotateTransform(newRotation);
            }

        }

        private bool separationDetection(Word word, Hand firstHand, Hand secondHand)
        {   // Detect if both hands are on the word
            if (secondHand.grip && secondHand.attachedObjectId == word.id)
            {
                if (word.typeWord == "complete")
                {
                    Word nouveau = word.Duplicate();
                    this.sequence.words.Add(nouveau);
                    Tuple<Word, Word> newCouple = new Tuple<Word, Word>(word, nouveau);
                    splitWordsCouple.Add(newCouple);
                    this.window.canvas.Children.Add(nouveau);
                    firstHand.attachedObjectId = word.id;
                    secondHand.attachedObjectId = nouveau.id;

                    return true;
                }                
            }
            return false;
        }

        // method to test if the fusion between 2 words is allowed, then it do the fusion
        private bool fusionDetection(Word currentWord, Hand mainHand)
        {
            foreach (var word in this.sequence.words.ToList())
            {
                if (word != currentWord && !forbiddenCoupleDetection(word, currentWord))
                {
                    if ((word.typeWord == "top" && currentWord.typeWord == "bottom") || (currentWord.typeWord == "top" && word.typeWord == "bottom"))
                    {
                        if (ImageTools.getDistance(word, currentWord) < 4000)
                        {
                            //do fusion
                            currentWord.Fusion(word);
                            this.sequence.words.Remove(word);
                            this.window.canvas.Children.Remove(word);
                            mainHand.attachedObjectId = currentWord.id;

                            return true;
                        }
                    }
                }
            }
            return false;
        }
        
        //method to detect if the couple of 2 words is in splitWordsCouple or not
        private bool forbiddenCoupleDetection(Word word1, Word word2)
        {
            Tuple<Word, Word> currentCouple = new Tuple<Word, Word>(word1, word2);

            foreach (var couple in splitWordsCouple.ToList())
            {
                if ((couple.Item1.id == currentCouple.Item1.id && couple.Item2.id == currentCouple.Item2.id) || 
                    (couple.Item1.id == currentCouple.Item2.id && couple.Item2.id == currentCouple.Item1.id))
                {
                    return true;
                }
            }
            return false;
        }

        //to set the text of the information label on the screen
        public void setInformationText(String s)
        {
            //window.userText.Content = s;
        }
    }
}
