using AODB_Front.Models;

namespace AODB_Front
{
    public partial class MainForm : Form
    {
        private readonly AuthenticationResult _user;

        public MainForm(AuthenticationResult user)
        {
            InitializeComponent();
            _user = user;
            
            // Kullanıcı biglileri
            this.Text = $"AODB - Welcome, {_user.User?.Username}";
            lblWelcome.Text = $"Welcome, {_user.User?.Username}!";
            lblRoles.Text = $"Roles: {string.Join(", ", _user.User?.Roles ?? new List<string>())}";
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to log out?", 
                                       "Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
            if (result == DialogResult.Yes)
            {
                this.Hide();
                
                // Login form
                var loginForm = new Form1();
                loginForm.ShowDialog();
                
                this.Close();
            }
        }

        private void btnFlights_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Uçuş işlemleri modülü burada açılacak.", "Bilgi", 
                          MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnAirports_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Havaalanı işlemleri modülü burada açılacak.", "Bilgi", 
                          MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnAirlines_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Havayolu işlemleri modülü burada açılacak.", "Bilgi", 
                          MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnAircraft_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Uçak işlemleri modülü burada açılacak.", "Bilgi", 
                          MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
