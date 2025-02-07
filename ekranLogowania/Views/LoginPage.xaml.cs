using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ekranLogowania.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            var email = EmailEntry.Text;
            var password = PasswordEntry.Text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Błąd", "Proszę wprowadzić e-mail i hasło.", "OK");
                return;
            }

            var token = await AuthenticateUser(email, password);
            if (!string.IsNullOrEmpty(token))
            {
                TokenLabel.Text = $"Otrzymany token: {token}";
            }
            else
            {
                await DisplayAlert("Błąd", "Nieprawidłowy e-mail lub hasło.", "OK");
            }
        }

        private async Task<string> AuthenticateUser(string email, string password)
        {
            try
            {
                var client = new HttpClient();
                var url = "http://localhost:3000/api/login"; // Zastąp odpowiednim adresem API
                var credentials = new LoginRequest { Email = email, Password = password };
                var json = JsonConvert.SerializeObject(credentials);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url, content);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<LoginResponse>(responseContent);
                    return result.Token;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Błąd", $"Wystąpił błąd: {ex.Message}", "OK");
            }

            return null;
        }
    }

    public class LoginResponse
    {
        [JsonProperty("token")]
        public string Token { get; set; }
    }

    public class LoginRequest
    {
        [JsonProperty("email")]
        public string Email { get; set; }


        [JsonProperty("password")]
        public string Password { get; set; }
    }
}