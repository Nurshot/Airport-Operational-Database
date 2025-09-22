using AODB_Front.Services;
using AODB_Front.Models;

namespace AODB_Front
{
    public partial class Form1 : Form
    {
        private readonly AuthenticationService _authService;
        private AuthenticationResult? _currentUser;

        public Form1()
        {
            InitializeComponent();
            _authService = new AuthenticationService();
            
            // Enter tuşu logini çağırır.
            this.KeyPreview = true;
            this.KeyDown += Form1_KeyDown;
            txtPassword.KeyDown += TxtPassword_KeyDown;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnLogin_Click(sender, e);
            }
        }

        private void TxtPassword_KeyDown(object sender, KeyEventArgs e)
        {

            //Logini çağırıyorum.
            if (e.KeyCode == Keys.Enter)
            {
                btnLogin_Click(sender, e);
            }
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                ShowStatus("Username Required.", Color.Red);
                txtUsername.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                ShowStatus("Password Required.", Color.Red);
                txtPassword.Focus();
                return;
            }

            
            btnLogin.Enabled = false;
            btnLogin.Text = "Loginning...";
            ShowStatus("Loginning...", Color.Blue);

            try
            {
                var result = await _authService.LoginAsync(txtUsername.Text, txtPassword.Text);

                if (result.IsSuccess)
                {
                    _currentUser = result;
                    ShowStatus($"Welcome, {result.User?.Username}!", Color.Green);
                    
                    // 1 saniye delay
                    await Task.Delay(1000);
                    
                    this.Hide();
                    
                    var mainForm = new MainForm(result);
                    mainForm.ShowDialog();
                    
                  
                    this.Show();

                    txtUsername.Clear();
                    txtPassword.Clear();
                    ShowStatus("", Color.Black);
                }
                else
                {
                    ShowStatus(result.ErrorMessage ?? "Login Failed", Color.Red);
                    txtPassword.Clear();
                    txtPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                ShowStatus($"Error: {ex.Message}", Color.Red);
            }
            finally
            {
                btnLogin.Enabled = true;
                btnLogin.Text = "Login";
            }
        }

        private void ShowStatus(string message, Color color)
        {
            lblStatus.Text = message;
            lblStatus.ForeColor = color;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _authService?.Dispose();
            base.OnFormClosed(e);
        }
    }
}
