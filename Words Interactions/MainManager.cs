using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect.Toolkit.Interaction;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TestKinect
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
        private List<Word> words;

        private Bin bin;

        public MainManager(MainWindow window)
        {
            this.window = window;
            this.hands = new Tuple<Hand, Hand>(this.window.leftCursor, this.window.rightCursor);
            this.hands.Item1.path = "images/left_cursor";
            this.hands.Item2.path = "images/right_cursor";

            words = new List<Word>();
            foreach (var word in this.window.canvas.Children.OfType<Word>())
            {
                words.Add(word);
            }            

            bin = this.window.corbeille;

            //has to be done at last
            this.kinectManager = new KinectManager(this, this.window.sensorChooserUi);
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
            hand.x = h.X * window.canvas.Width;
            hand.y = h.Y * window.canvas.Height;

            //check if the hand is grip
            if (!hand.grip && h.HandEventType == InteractionHandEventType.Grip)
            {
                hand.grip = true;
            }
            else
                if (hand.grip && h.HandEventType == InteractionHandEventType.GripRelease)
                {
                    hand.grip = false;
                    hand.attachedObjectName = "";
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
            bin.hover = false;
            foreach (var word in words.ToList())
            {
                word.hover = false;
                moveDetectionWord(word, 1);
                moveDetectionWord(word, 2);

                //if the left hand is on the word
                if (hands.Item1.grip && hands.Item1.attachedObjectName == word.Name)
                {
                    zoomDetection(word, hands.Item2);
                    rotationDetection(word, hands.Item2);
                    transformationDetection(word, hands.Item2);
                }
                else
                    //if the right hand is on the word
                    if (hands.Item2.grip && hands.Item2.attachedObjectName == word.Name)
                    {
                        zoomDetection(word, hands.Item1);
                        rotationDetection(word, hands.Item1);
                        transformationDetection(word, hands.Item1);
                    }

            }
        }

        private void moveDetectionWord(Word word, byte which)
        {
            var hand = which == 1 ? hands.Item1 : hands.Item2;
            var secondHand = which == 2 ? hands.Item1 : hands.Item2;

            //if the word is on the bin
            if (ImageTools.isOn(word, bin))
            {
                //if we release the hand on the bin, we delete the word
                if (hand.justReleased)
                {
                    this.window.canvas.Children.Remove(word);
                }
                bin.hover = true;
            }

            //if the hand is on the text
            if (ImageTools.isOn(hand, word))
            {
                //detection of grip
                if (hand.justGrip)
                {
                    //we attach the label to the hand until the grip is released
                    hand.attachedObjectName = word.Name;

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
            if (hand.grip && !hand.pressed && hand.attachedObjectName == word.Name)
            {
                //this is to keep the words inside the canvas
                Point newPos = new Point(hand.x + hand.ActualWidth / 2 - word.ActualWidth / 2, hand.y + hand.ActualHeight / 2 - word.ActualHeight / 2);
                if (newPos.X + word.Width / 2 > 0 && newPos.X - word.Width / 2 < this.window.canvas.Width - word.ActualWidth && newPos.Y > 0 && newPos.Y < this.window.canvas.Height - word.ActualHeight)
                {
                    word.x = newPos.X;
                    word.y = newPos.Y;
                    word.hover = true;
                }
                else
                {
                    hand.attachedObjectName = "";
                }
            }
        }

        // method to manage to zoom detection
        private void zoomDetection(Word word, Hand secondHand)
        {
            //zoom if second hand gripping and without a text attached
            if (secondHand.grip && secondHand.attachedObjectName == "")
            {
                var distance = ImageTools.getDistance(hands.Item1, hands.Item2);
                var difference = distance - Hand.distance;
                var facteur = difference / 1000000 + 1;
                if (difference > 5000 && word.Width * facteur <= Word.MAX_WIDTH)
                {

                    word.Width *= facteur;
                    word.Height *= facteur;
                }
                else
                    if (difference < -5000 && word.Width * facteur >= Word.MIN_WIDTH)
                    {
                        word.Width *= facteur;
                        word.Height *= facteur;
                    }
                Hand.distance = distance;
            }

        }

        // method to manage to rotate detection
        private void rotationDetection(Word word, Hand secondHand)
        {
            //mzoom if second hand open and without a text attached
            if (secondHand.grip && secondHand.attachedObjectName == "")
            {
                double wordRotation = word.beginRotation;

                var newRotation = (wordRotation - ImageTools.getRotation(hands.Item1, hands.Item2)) % 360;
                word.RenderTransform = new RotateTransform(newRotation);
            }

        }

        private void transformationDetection(Word word, Hand secondHand)
        {   // Detect if both hands are on the word
            if (secondHand.grip && secondHand.attachedObjectName == word.Name)
            {
                if (!word.separated)
                {
                    Word nouveau = word.Duplicate();
                    words.Add(nouveau);
                    this.window.canvas.Children.Add(nouveau);
                    secondHand.attachedObjectName = nouveau.Name;
                }
            }

        }

        //to set the text of the information label on the screen
        public void setInformationText(String s)
        {
            window.userText.Content = s;
        }
    }
}
