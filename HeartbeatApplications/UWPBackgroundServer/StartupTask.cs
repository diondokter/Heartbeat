using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using Server;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace UWPBackgroundServer
{
    public sealed class StartupTask : IBackgroundTask
    {
		private static BackgroundTaskDeferral Deferral;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
			Deferral = taskInstance.GetDeferral();

			Controller.Start(5000, new Logger());
        }
    }
}
