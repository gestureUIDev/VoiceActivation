using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.VoiceCommands;

namespace InClassCortana.BackgroundCortana
{
    public sealed class InClassBackgroundService : IBackgroundTask
    {
        private BackgroundTaskDeferral _deferral;
        // the background app service connection to Cortana
        VoiceCommandServiceConnection voiceServiceConection;
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            // add service functionality here
            // inform the system that the background task may 
            // continue after the Run method has completed
            this._deferral = taskInstance.GetDeferral();

            // added event handler to handle cancel by user
            taskInstance.Canceled += TaskInstance_Canceled;

            // trigger details gives more information about the 
            // task instance
            var triggerDetails = taskInstance.TriggerDetails as AppServiceTriggerDetails;

            if ((triggerDetails != null) &&
                (triggerDetails.Name == "InClassBackgroundService"))
            {
                // try to get the voice command and respond to it.
                try
                {
                    // Retrieves a VoiceCommandServiceConnection object 
                    // from the AppServiceTriggerDetails that 
                    // contains info associated with the background 
                    // task for the app service
                    voiceServiceConection =
                        VoiceCommandServiceConnection.
                        FromAppServiceTriggerDetails(triggerDetails);

                    // set up the command completed method to indicate completion
                    // calls deferral.complete here
                    voiceServiceConection.VoiceCommandCompleted += VoiceServiceConection_VoiceCommandCompleted;

                    // get the voice command information
                    VoiceCommand voiceCommand = await voiceServiceConection.
                                                    GetVoiceCommandAsync();

                    switch (voiceCommand.CommandName)
                    {
                        case "howManyGames":
                            {
                                sendUpdateMessageByReturn();
                                break;
                            }
                        default:
                            {
                                launchAppInForeground();
                                break;
                            }
                    }
                }
                finally
                {
                    if (this._deferral != null)
                    {
                        // complete the service deferral
                        this._deferral.Complete();
                    }
                } // end try
            }


        }


        private async void sendUpdateMessageByReturn()
        {
            // if longer than 0.5 seconds, then progress report has 
            // to be sent.  Call this method when necessary.
            string progressMessage = "Finding how many games being played.";
            await ShowProgressScreen(progressMessage);

            var userMsg = new VoiceCommandUserMessage();
            userMsg.DisplayMessage = userMsg.SpokenMessage =
                "You are currently playing 3 games";

            VoiceCommandResponse response =
                VoiceCommandResponse.CreateResponse(userMsg);
            await voiceServiceConection.ReportSuccessAsync(response);
        }

        private async Task ShowProgressScreen(string progressMessage)
        {
            var userProgressMsg = new VoiceCommandUserMessage();
            userProgressMsg.DisplayMessage =
                userProgressMsg.SpokenMessage = progressMessage;
            VoiceCommandResponse response =
                VoiceCommandResponse.CreateResponse(userProgressMsg);
            await voiceServiceConection.ReportProgressAsync(response);
        }


        private void VoiceServiceConection_VoiceCommandCompleted(VoiceCommandServiceConnection sender, VoiceCommandCompletedEventArgs args)
        {
            if (this._deferral != null)
            {
                this._deferral.Complete();
            }
        }

        private void TaskInstance_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            if (this._deferral != null)
            {
                this._deferral.Complete();
            }
        }

        private async void launchAppInForeground()
        {
            string progressMessage = "Trying to launch the tic tac toe app now";
            await ShowProgressScreen(progressMessage);

            var userMessage = new VoiceCommandUserMessage();
            userMessage.SpokenMessage = "Launching a game of tic tac toe";

            var response = VoiceCommandResponse.
                CreateResponse(userMessage);

            response.AppLaunchArgument = "";

            await voiceServiceConection.
                RequestAppLaunchAsync(response);
        }


    }
}
