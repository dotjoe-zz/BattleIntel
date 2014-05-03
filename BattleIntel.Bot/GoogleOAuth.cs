using Google.GData.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BattleIntel.Bot
{
    static class GoogleOAuth
    {
        public static OAuth2Parameters Authorize()
        {
            var parameters = new OAuth2Parameters
            {
                ClientId = ConfigurationManager.AppSettings.Get("Google-ClientID"),
                ClientSecret = ConfigurationManager.AppSettings.Get("Google-ClientSecret"),
                Scope = "https://spreadsheets.google.com/feeds",
                RedirectUri = "urn:ietf:wg:oauth:2.0:oob" //installed applications
            };

            var authUri = new Uri(OAuthUtil.CreateOAuth2AuthorizationUrl(parameters));
            using (var login = new GoogleLogin(authUri))
            {
                if (login.ShowDialog() != DialogResult.OK) return null;
                parameters.AccessCode = login.AccessCode;
            }

            try
            {
                OAuthUtil.GetAccessToken(parameters);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }

            return parameters;
        }
    }
}
