using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using Server;

namespace PiServer
{
    public sealed class StartupTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
			BackgroundTaskDeferral Deferral = taskInstance.GetDeferral();

			Controller.Start(5000, new Logger());

			taskInstance.Canceled += OnProgramCanceled;
        }

		private void OnProgramCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
		{
			Controller.Stop();
		}
	}
}
