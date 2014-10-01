using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;

namespace DesafioThingPink.Manager
{
    partial class FacebookManager
    {
        public async Task LoginAsync()
        {
            var result = await WebAuthenticationBroker.AuthenticateAsync(
                                                        WebAuthenticationOptions.None,
                                                        _loginUrl);

            ValidateAndProccessResult(result);
        }
    }
}
