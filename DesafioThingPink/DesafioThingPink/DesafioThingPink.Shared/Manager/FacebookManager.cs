using Facebook;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;

namespace DesafioThingPink.Manager
{
    //http://juank.io/autenticacion-facebook-universal-apps-c/
   partial class FacebookManager
    {
        public FacebookClient _fb = new FacebookClient();
        readonly Uri _callbackUri = WebAuthenticationBroker.GetCurrentApplicationCallbackUri();
        readonly Uri _loginUrl;

        public string AccessToken
        {
            get { return _fb.AccessToken; }
        }

        public FacebookManager()
        {
            _loginUrl = _fb.GetLoginUrl(new
                    {
                        client_id = App.Current.Resources["FacebookAppId"],
                        redirect_uri = _callbackUri.AbsoluteUri,
                        scope = App.Current.Resources["FacebookPermissions"],
                        display = "popup",
                        response_type = "token"
                    });
            _fb.AppId = (string) App.Current.Resources["FacebookAppId"];
            _fb.AppSecret = "a4df5f6b19347ec2916e36f27542f5f7";
        }

        private void ValidateAndProccessResult(WebAuthenticationResult result)
        {
            if (result.ResponseStatus == WebAuthenticationStatus.Success)
            {
                var responseUri = new Uri(result.ResponseData.ToString());
                var facebookOAuthResult = _fb.ParseOAuthCallbackUrl(responseUri);

                if (string.IsNullOrWhiteSpace(facebookOAuthResult.Error))
                    _fb.AccessToken = facebookOAuthResult.AccessToken;
                else
                {//erro de acceso negado por cancelacao da página
                }
            }
            else if (result.ResponseStatus == WebAuthenticationStatus.ErrorHttp)
            {// erro de http
            }
            else
            {// outros erros
            }
        }
    }

}
