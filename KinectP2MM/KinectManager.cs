using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;
using Microsoft.Kinect.Toolkit.Interaction;


namespace KinectP2MM
{
    class KinectManager
    {
        private KinectSensor _sensor;
        private InteractionStream _interactionStream;
        private KinectSensorChooser sensorChooser;
        private UserInfo[] _userInfos;
        private SequenceManager sequenceManager;
        private int defaultElevation;

        public KinectManager(SequenceManager manager, KinectSensorChooserUI sensorUi, int elevation)
        {
            this.sequenceManager = manager;
            //initialize the sensor chooser UI
            this.sensorChooser = new KinectSensorChooser();
            this.sensorChooser.KinectChanged += SensorChooserOnKinectChanged;
            sensorUi.KinectSensorChooser = this.sensorChooser;
            this.defaultElevation = elevation;
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

                    _sensor.DepthStream.Range = DepthRange.Default;
                    _sensor.SkeletonStream.EnableTrackingInNearRange = false;

                    _interactionStream = new InteractionStream(_sensor, new DummyInteractionClient());
                    _interactionStream.InteractionFrameReady += InteractionStreamOnInteractionFrameReady;


                    _userInfos = new UserInfo[InteractionFrame.UserInfoArrayLength];

                    _sensor.DepthFrameReady += SensorOnDepthFrameReady;
                    _sensor.SkeletonFrameReady += SensorOnSkeletonFrameReady;

                    _sensor.Start();
                    setElevation(defaultElevation);

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

                sequenceManager.newImageReceived(userInfo);
                hasUser = true;
                //we manage only the first user
                break;
            }
            if (!hasUser)
                sequenceManager.setInformationText("No User Detected");
            else
                sequenceManager.setInformationText("User Tracked");
        }

        public void setElevation(int el)
        {
            _sensor.ElevationAngle = el;
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
    }
}
