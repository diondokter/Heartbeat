using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using System.Diagnostics;
using Server;

namespace UWPBackgroundServer
{
    public sealed class StartupTask : IBackgroundTask
    {
		private BackgroundTaskDeferral Deferral;

		public void Run(IBackgroundTaskInstance taskInstance)
        {
			Deferral = taskInstance.GetDeferral();
			Controller.Start(5000, null);
        }
    }
}
