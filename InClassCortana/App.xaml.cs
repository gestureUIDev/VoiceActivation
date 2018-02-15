using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.SpeechRecognition;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace InClassCortana
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();

                // register voice commands here.
                try
                {
                    System.Diagnostics.Debug.WriteLine("Loading VCD...");
                    StorageFile vcdFile =
                        await Package.Current.InstalledLocation.GetFileAsync(
                                                    @"VoiceCommands.xml");
                    await VoiceCommandDefinitionManager.InstallCommandDefinitionsFromStorageFileAsync(
                                                        vcdFile);
                    System.Diagnostics.Debug.WriteLine("successfully loaded VCD...");
                }
                catch (Exception err)
                {
                    System.Diagnostics.Debug.WriteLine(
                                "Failed to install VCD - " + err.Message);
                }


            }
        }



        /// <summary>
        /// add the override onActivated method
        /// allows you to take control when voice activated.
        /// </summary>
        protected override void OnActivated(IActivatedEventArgs args)
        {
            base.OnActivated(args);
            VoiceCommandActivatedEventArgs commandArgs = null;
            Type navigateToType = null;

            // handle voice activation here.
            if( args.Kind == ActivationKind.VoiceCommand )
            {
                // just got activated by voice command
                // get the voice command and any parameters
                commandArgs = args as VoiceCommandActivatedEventArgs;
                SpeechRecognitionResult speechRecognitionResult =
                                commandArgs.Result;
                string voiceCmdName = speechRecognitionResult.RulePath[0];
                string textSpoken = speechRecognitionResult.Text;

                System.Diagnostics.Debug.WriteLine(voiceCmdName);
                System.Diagnostics.Debug.WriteLine(textSpoken);

                switch (voiceCmdName)
                {
                    case "startNew":
                        System.Diagnostics.Debug.WriteLine("start new command");
                        navigateToType = typeof(MainPage);
                        break;
                    case "makeMove":
                        System.Diagnostics.Debug.WriteLine("make a move command");
                        //navigateToType = typeof(CurrentGame);
                        break;
                    default:
                        break;
                }

                // decided what page, what command, args
                // go to that page.
                Frame rootFrame = Window.Current.Content as Frame;
                if( rootFrame == null)
                {
                    // set up tthe content
                    rootFrame = new Frame();
                    rootFrame.NavigationFailed += OnNavigationFailed;
                    Window.Current.Content = rootFrame;
                }
                rootFrame.Navigate(navigateToType);
                Window.Current.Activate();
            }
        }



        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
