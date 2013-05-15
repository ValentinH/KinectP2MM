using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Text;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;
using Microsoft.Kinect.Toolkit.Interaction;


namespace TestKinect
{
    // Logique d'interaction pour MainWindow.xaml
    public partial class MainWindow : Window
    {
        //limit of size for image
        private const int MIN_WIDTH = 100, MAX_WIDTH = 1000;

        //zoom factors
        private const double ZOOM_FACTOR = 1.05, UNZOOM_FACTOR = 0.95;

        //Active Kinect sensor
        private KinectSensor _sensor;
        private InteractionStream _interactionStream;

        //for the sensor chooser UI interface that is used to give information about the Kinect device to the user
        private KinectSensorChooser sensorChooser;

        private UserInfo[] _userInfos; //the information about the interactive users

        private Hand leftHand, rightHand;
        private Tuple<Hand, Hand> hands;

        //list of words to manipulate
        private List<Image> words;

        //to store the last hands events
        private InteractionHandEventType _lastLeftHandEvent;
        private InteractionHandEventType _lastRightHandEvent;

        //to store the last distance between the hands
        private double hands_distance;

        //to store the rotation of the hands at the beginning of the movement
        private Dictionary<String, double> words_begin_rotation;

        //to know if the current word is hover
        private bool hoverWord, hoverCorbeille;

        public MainWindow()
        {
            InitializeComponent();
            //references the OnLoaded function on the OnLoaded event
            Loaded += WindowLoaded;

            //list of hands used to browse all the hands
            leftHand = new Hand();
            rightHand = new Hand();
            hands = new Tuple<Hand, Hand>(leftHand, rightHand);

            //list of labels used to browse all the words that can be grip
            words = new List<Image>();
            words.Add(tomateObject);
            words.Add(bananeObject);


            //initiate the dictionnary of rotations
            words_begin_rotation = new Dictionary<string, double>();

            _lastLeftHandEvent = new InteractionHandEventType();
            _lastRightHandEvent = new InteractionHandEventType();

        }

        // Execute startup tasks
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            //initialize the sensor chooser UI
            this.sensorChooser = new KinectSensorChooser();
            this.sensorChooser.KinectChanged += SensorChooserOnKinectChanged;
            this.sensorChooserUi.KinectSensorChooser = this.sensorChooser;
            this.sensorChooser.Start();
        }

        //Called when a Kinect is connected and ready
        private void SensorChooserOnKinectChanged(object sender, KinectChangedEventArgs args)
        {
            if (args.OldSensor != null)
            {
                try
                {
                    args.OldSensor.DepthStream.Range = DepthRange.Default;
                    args.OldSensor.SkeletonStream.EnableTrackingInNearRange = false;
                    args.OldSensor.DepthStream.Disable();
                    args.OldSensor.SkeletonStream.Disable();
                }
                catch (InvalidOperationException)
                {
                    MessageBox.Show("InvalidOperationException on old sensor");
                }
            }

            if (args.NewSensor != null)
            {
                try
                {
                    this._sensor = args.NewSensor;
                    _sensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                    _sensor.SkeletonStream.Enable();

                    //Because we have the Kinect for XBox 360, we can't use the Near Range
                    _sensor.DepthStream.Range = DepthRange.Default;
                    _sensor.SkeletonStream.EnableTrackingInNearRange = false;

                    _userInfos = new UserInfo[InteractionFrame.UserInfoArrayLength];

                    _interactionStream = new InteractionStream(_sensor, new DummyInteractionClient());
                    _interactionStream.InteractionFrameReady += InteractionStreamOnInteractionFrameReady;

                    _sensor.DepthFrameReady += SensorOnDepthFrameReady;
                    _sensor.SkeletonFrameReady += SensorOnSkeletonFrameReady;

                    _sensor.Start();
                }
                catch (InvalidOperationException)
                {
                    MessageBox.Show("InvalidOperationException on new sensor");
                }
            }
        }

        //called on every new frame
        private void InteractionStreamOnInteractionFrameReady(object sender, InteractionFrameReadyEventArgs args)
        {
            using (var iaf = args.OpenInteractionFrame()) //dispose as soon as possible
            {
                if (iaf == null)
                    return;

                iaf.CopyInteractionDataTo(_userInfos);
            }

            var hasUser = false;
            foreach (var userInfo in _userInfos)
            {
                var userID = userInfo.SkeletonTrackingId;
                if (userID == 0)
                    continue;

                hasUser = true;
                var hands = userInfo.HandPointers;
                foreach (var hand in hands)
                {
                    updateHand(hand);
                }

                interactionOnText();
                interactionOnBin();
                //we manage only the first user
                break;
            }
            if (!hasUser)
                userText.Content = "No User Detected";
            else
                userText.Content = "User Tracked";


        }

        //function to update the model of the hand and to draw the cursor corresponding to the given hand
        private void updateHand(InteractionHandPointer h)
        {
            //current hand
            var hand = h.HandType == InteractionHandType.Left ? leftHand : rightHand;

            //cursor on the screen
            var cursor = h.HandType == InteractionHandType.Left ? leftCursor : rightCursor;
            //background of the cursor on the screen
            var cursorBackground = h.HandType == InteractionHandType.Left ? leftCursorBackground : rightCursorBackground;

            //update the model of the hand
            hand.x = h.X * canvas.Width;
            hand.y = h.Y * canvas.Height;

            //update the position of the hand on the screen
            Canvas.SetLeft(cursor, hand.x);
            Canvas.SetTop(cursor, hand.y);

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

            //checked if ThemeDictionaryExtension hand is gripping
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

            //set the just grip value
            var lastHandEvent = h.HandType == InteractionHandType.Left ? _lastLeftHandEvent : _lastRightHandEvent;
            if (h.HandEventType == InteractionHandEventType.Grip && lastHandEvent != InteractionHandEventType.Grip)
            {
                hand.justGrip = true;
                hands_distance = getDistanceHands();
            }
            else
                hand.justGrip = false;

            //update of the lastHandEvent
            lastHandEvent = h.HandEventType;

            //update the color
            manageColorCursor(hand, cursorBackground);
        }

        private void manageColorCursor(Hand hand, Rectangle cursorBackground)
        {
            if (hand.pressed)
                cursorBackground.Fill = Brushes.Green;
            else
                if (hand.grip)
                    cursorBackground.Fill = Brushes.Red;
                else
                    cursorBackground.Fill = Brushes.RoyalBlue;
        }

        private void interactionOnText()
        {
            foreach (var word in words)
            {
                hoverWord = false;
                moveDetectionWord(word, 1);
                moveDetectionWord(word, 2);

                //if the left hand is on the word
                if (hands.Item1.grip && hands.Item1.attachedObjectName == word.Name)
                {
                    zoomDetection(word, hands.Item2);
                    rotationDetection(word, hands.Item1);
                }
                else
                    //if the right hand is on the word
                    if (hands.Item2.grip && hands.Item2.attachedObjectName == word.Name)
                    {
                        zoomDetection(word, hands.Item1);
                        rotationDetection(word, hands.Item2);
                    }

                if (hoverWord)
                    word.Opacity = 0.6;
                else
                    word.Opacity = 1;
            }
        }

        private void interactionOnBin()
        {
            hoverCorbeille = false;
            moveDetectionBin(corbeille, 1);
            moveDetectionBin(corbeille, 2);
            if (hoverCorbeille)
            {
                corbeille.Opacity = 0.6;
            }
            else
                corbeille.Opacity = 1;
        }

        private void moveDetectionWord(Image word, byte which)
        {
            var hand = which == 1 ? hands.Item1 : hands.Item2;
            var secondHand = which == 2 ? hands.Item1 : hands.Item2;
            var cursor = which == 1 ? leftCursor : rightCursor;

            Point posWord = new Point(Canvas.GetLeft(word), Canvas.GetTop(word));

            //if the hand is on the text
            if (hand.x + cursor.ActualWidth / 2 >= posWord.X && (hand.x < posWord.X + word.ActualWidth) && hand.y + cursor.ActualHeight / 2 >= posWord.Y && (hand.y < posWord.Y + word.ActualHeight))
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
                    if (words_begin_rotation.ContainsKey(word.Name))
                        words_begin_rotation[word.Name] = rotationInDegrees;
                    else
                        words_begin_rotation.Add(word.Name, rotationInDegrees);
                   
                }
                hoverWord = true;
            }
            //moving of the attached text
            if (hand.grip && !hand.pressed && hand.attachedObjectName == word.Name)
            {
                Point newPos = new Point(hand.x + cursor.ActualWidth / 2 - word.ActualWidth / 2, hand.y + cursor.ActualHeight / 2 - word.ActualHeight / 2);
                if (newPos.X + word.Width / 2 > 0 && newPos.X - word.Width / 2 < canvas.Width - word.ActualWidth && newPos.Y > 0 && newPos.Y < canvas.Height - word.ActualHeight)
                {
                    Canvas.SetLeft(word, newPos.X);
                    Canvas.SetTop(word, newPos.Y);
                    if (hoverCorbeille)
                    {
                        this.canvas.Children.Remove(word);
                    }
                    hoverWord = true;
                }
                else
                {
                    hand.attachedObjectName = "";
                }
            }

        }

        private void moveDetectionBin(Image bin, byte which)
        {
            var hand = which == 1 ? hands.Item1 : hands.Item2;
            var secondHand = which == 2 ? hands.Item1 : hands.Item2;
            var cursor = which == 1 ? leftCursor : rightCursor;

            Point posBin = new Point(Canvas.GetLeft(bin), Canvas.GetTop(bin));

            //if the hand is on the text
            if (hand.x + cursor.ActualWidth / 2 >= posBin.X && (hand.x < posBin.X + bin.ActualWidth) && hand.y + cursor.ActualHeight / 2 >= posBin.Y && (hand.y < posBin.Y + bin.ActualHeight))
            {    
                hoverCorbeille = true;
            }
        }

        // method to manage to zoom detection
        private void zoomDetection(Image word, Hand secondHand)
        {
            //mzoom if second hand gripping and without a text attached
            if (secondHand.grip && secondHand.attachedObjectName == "")
            {
                var distance = getDistanceHands();
                if (distance > hands_distance && word.Width * ZOOM_FACTOR <= MAX_WIDTH)
                {
                    word.Width *= ZOOM_FACTOR;
                    word.Height *= ZOOM_FACTOR;
                }
                else
                    if (hands_distance > distance && word.Width * UNZOOM_FACTOR >= MIN_WIDTH)
                    {
                        word.Width *= UNZOOM_FACTOR;
                        word.Height *= UNZOOM_FACTOR;
                    }
                hands_distance = distance;
            }

        }

        // method to manage to rotate detection
        // maybe it would be better to start the rotation with the position of the second hand as a base
        private void rotationDetection(Image word, Hand mainHand)
        {
            //mzoom if second hand open and without a text attached
            if (mainHand.grip && mainHand.pressed)
            {
                double wordRotation = 0;
                if (words_begin_rotation.ContainsKey(word.Name))
                    wordRotation = words_begin_rotation[word.Name];

                var newRotation = (wordRotation - getRotationHands()) % 360;
                word.RenderTransform = new RotateTransform(newRotation);
            }

        }

        //distance calculation
        private double getDistanceHands()
        {
            return (hands.Item2.x - hands.Item1.x) * (hands.Item2.x - hands.Item1.x) + (hands.Item2.y - hands.Item1.y) * (hands.Item2.y - hands.Item1.y);
        }

        //rotation calculation in degrees
        private double getRotationHands()
        {
            Point A = new Point(hands.Item1.x, hands.Item1.y);
            Point B = new Point(hands.Item2.x, hands.Item2.y);

            Point C = new Point(B.X, A.Y);

            var AC = C.X - A.X;
            var AB = Math.Sqrt((B.X - A.X) * (B.X - A.X) + (B.Y - A.Y) * (B.Y - A.Y));
            var BC = C.Y - B.Y;

            var cos = Math.Acos(AC / AB);
            var sin = Math.Asin(BC / AB);

            double angle = Math.Acos(AC / AB) * 180 / Math.PI;
            if (sin < 0)
                angle = 360 - angle;

            return angle;
        }

        #region necessary code for the interaction stream

        public class DummyInteractionClient : IInteractionClient
        {
            public InteractionInfo GetInteractionInfoAtLocation(
                int skeletonTrackingId,
                InteractionHandType handType,
                double x,
                double y)
            {
                var result = new InteractionInfo();
                result.IsGripTarget = true;
                result.IsPressTarget = true;
                result.PressAttractionPointX = 0.5;
                result.PressAttractionPointY = 0.5;
                result.PressTargetControlId = 1;

                return result;
            }
        }

        private void SensorOnSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs skeletonFrameReadyEventArgs)
        {
            using (SkeletonFrame skeletonFrame = skeletonFrameReadyEventArgs.OpenSkeletonFrame())
            {
                if (skeletonFrame == null)
                    return;

                //initialisation to the max size
                Skeleton[] skeletons = new Skeleton[_sensor.SkeletonStream.FrameSkeletonArrayLength];
                try
                {
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                    var accelerometerReading = _sensor.AccelerometerGetCurrentReading();
                    _interactionStream.ProcessSkeleton(skeletons, accelerometerReading, skeletonFrame.Timestamp);
                }
                catch (InvalidOperationException)
                {
                    // SkeletonFrame functions may throw when the sensor gets
                    // into a bad state.  Ignore the frame in that case.
                }
            }
        }

        private void SensorOnDepthFrameReady(object sender, DepthImageFrameReadyEventArgs depthImageFrameReadyEventArgs)
        {
            using (DepthImageFrame depthFrame = depthImageFrameReadyEventArgs.OpenDepthImageFrame())
            {
                if (depthFrame == null)
                    return;

                try
                {
                    _interactionStream.ProcessDepth(depthFrame.GetRawPixelData(), depthFrame.Timestamp);
                }
                catch (InvalidOperationException)
                {
                    // DepthFrame functions may throw when the sensor gets
                    // into a bad state.  Ignore the frame in that case.
                }
            }
        }
        #endregion

        // Execute shutdown tasks
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (null != this._sensor)
            {
                this._sensor.Stop();
            }
        }
    }

}


