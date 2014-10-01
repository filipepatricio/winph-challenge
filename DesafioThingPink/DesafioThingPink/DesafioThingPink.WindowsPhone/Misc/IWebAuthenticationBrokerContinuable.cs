using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;

namespace DesafioThingPink.Misc
{
    interface IWebAuthenticationBrokerContinuable
    {
        void ContinueWithWebAuthenticationBroker(WebAuthenticationBrokerContinuationEventArgs args);
    }
}
