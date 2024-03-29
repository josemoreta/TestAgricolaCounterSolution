﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Common
{
    public sealed class SuspensionManager
    {
        private const string NavigationKey = "Navigation";
        private const string sessionStateFilename = "_sessionState.xml";
        private static List<Type> _knownTypes = new List<Type>();
        private static List<WeakReference<Frame>> _registeredFrames = new List<WeakReference<Frame>>();
        private static Dictionary<string, object> _sessionState = new Dictionary<string, object>();

        private static DependencyProperty FrameSessionBaseKeyProperty =
            DependencyProperty.RegisterAttached("_FrameSessionBaseKeyParams", typeof(string), typeof(SuspensionManager), null);

        private static DependencyProperty FrameSessionStateKeyProperty =
            DependencyProperty.RegisterAttached("_FrameSessionStateKey", typeof(string), typeof(SuspensionManager), null);

        private static DependencyProperty FrameSessionStateProperty =
            DependencyProperty.RegisterAttached("_FrameSessionState", typeof(Dictionary<string, object>), typeof(SuspensionManager), null);

        public static List<Type> KnownTypes
        {
            get { return _knownTypes; }
        }

        public static Dictionary<string, object> SessionState
        {
            get { return _sessionState; }
        }

        public static void RegisterFrame(Frame frame, string sessionStateKey, string sessionBaseKey = null)
        {
            if (frame.GetValue(FrameSessionStateKeyProperty) != null)
            {
                throw new InvalidOperationException("Frames can only be registered to one session state key");
            }

            if (frame.GetValue(FrameSessionStateProperty) != null)
            {
                throw new InvalidOperationException("Frames must be either be registerd before accessing frame session staet, or not registered at all");
            }

            if (!string.IsNullOrEmpty(sessionBaseKey))
            {
                frame.SetValue(FrameSessionBaseKeyProperty, sessionBaseKey);
                sessionStateKey = sessionStateKey + "_" + sessionStateKey;
            }

            frame.SetValue(FrameSessionStateKeyProperty, sessionStateKey);
            _registeredFrames.Add(new WeakReference<Frame>(frame));

            RestoreFrameNavigationState(frame);
        }

        public static async Task RestoreAsync(string sessionBaseKey = null)
        {
            _sessionState = new Dictionary<string, object>();

            try
            {
                StorageFile file = await
                    ApplicationData.Current.LocalFolder.GetFileAsync(sessionStateFilename);
                using (IInputStream inStream = await file.OpenSequentialReadAsync())
                {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(Dictionary<string, object>), _knownTypes);
                    _sessionState = (Dictionary<string, object>)serializer.ReadObject(inStream.AsStreamForRead());
                }

                foreach (var weakFrameReference in _registeredFrames)
                {
                    Frame frame;
                    if (weakFrameReference.TryGetTarget(out frame) && (string)frame.GetValue(FrameSessionBaseKeyProperty) == sessionBaseKey)
                    {
                        frame.ClearValue(FrameSessionStateProperty);
                        RestoreFrameNavigationState(frame);
                    }
                }
            }
            catch (Exception e)
            {
                throw new SuspensionManagerException(e);
            }
        }

        public static async Task SaveAsync()
        {
            try
            {
                foreach (var weakFrameReference in _registeredFrames)
                {
                    Frame frame;
                    if (weakFrameReference.TryGetTarget(out frame))
                    {
                        SaveFrameNavigationState(frame);
                    }
                }

                MemoryStream sessionData = new MemoryStream();
                DataContractSerializer serializer = new DataContractSerializer(typeof(Dictionary<string, object>), _knownTypes);
                serializer.WriteObject(sessionData, _sessionState);

                StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(sessionStateFilename, CreationCollisionOption.ReplaceExisting);
                using (Stream fileStream = await file.OpenStreamForWriteAsync())
                {
                    sessionData.Seek(0, SeekOrigin.Begin);
                    await sessionData.CopyToAsync(fileStream);
                }
            }
            catch (Exception e)
            {
                throw new SuspensionManagerException(e);
            }
        }

        public static Dictionary<string, object> SessionStateForFrame(Frame frame)
        {
            var frameState = (Dictionary<string, object>)frame.GetValue(FrameSessionStateProperty);

            if (frameState == null)
            {
                var frameSessionKey = (string)frame.GetValue(FrameSessionStateKeyProperty);
                if (frameSessionKey != null)
                {
                    if (!_sessionState.ContainsKey(frameSessionKey))
                    {
                        _sessionState[frameSessionKey] = new Dictionary<string, object>();
                    }

                    frameState = (Dictionary<string, object>)_sessionState[frameSessionKey];
                }
                else
                {
                    frameState = new Dictionary<string, object>();
                }

                frame.SetValue(FrameSessionStateProperty, frameState);
            }

            return frameState;
        }

        public static void UndregisterFrame(Frame frame)
        {
            SessionState.Remove((string)frame.GetValue(FrameSessionStateKeyProperty));
            _registeredFrames.RemoveAll((weakFrameRefernce) =>
                {
                    Frame testFrame;

                    return !weakFrameRefernce.TryGetTarget(out testFrame) || testFrame == frame;
                });
        }

        private static void RestoreFrameNavigationState(Frame frame)
        {
            var frameState = SessionStateForFrame(frame);
            if (frameState.ContainsKey(NavigationKey))
            {
                frame.SetNavigationState((string)frameState[NavigationKey]);
            }
        }

        private static void SaveFrameNavigationState(Frame frame)
        {
            var frameState = SessionStateForFrame(frame);
            frameState[NavigationKey] = frame.GetNavigationState();
        }
    }

    public class SuspensionManagerException : Exception
    {
        public SuspensionManagerException()
        {
        }

        public SuspensionManagerException(Exception e)
            : base("SuspensionManager failed", e)
        {
        }
    }
}