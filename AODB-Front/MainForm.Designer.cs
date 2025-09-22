namespace AODB_Front
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblWelcome = new Label();
            lblRoles = new Label();
            btnLogout = new Button();
            panel1 = new Panel();
            btnAircraft = new Button();
            btnAirlines = new Button();
            btnAirports = new Button();
            btnFlights = new Button();
            label1 = new Label();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // lblWelcome
            // 
            lblWelcome.AutoSize = true;
            lblWelcome.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblWelcome.ForeColor = Color.FromArgb(0, 123, 255);
            lblWelcome.Location = new Point(30, 30);
            lblWelcome.Name = "lblWelcome";
            lblWelcome.Size = new Size(200, 30);
            lblWelcome.TabIndex = 0;
            lblWelcome.Text = "Hoş geldiniz!";
            // 
            // lblRoles
            // 
            lblRoles.AutoSize = true;
            lblRoles.Font = new Font("Segoe UI", 10F);
            lblRoles.Location = new Point(30, 70);
            lblRoles.Name = "lblRoles";
            lblRoles.Size = new Size(45, 19);
            lblRoles.TabIndex = 1;
            lblRoles.Text = "Roller:";
            // 
            // btnLogout
            // 
            btnLogout.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnLogout.BackColor = Color.FromArgb(220, 53, 69);
            btnLogout.FlatStyle = FlatStyle.Flat;
            btnLogout.ForeColor = Color.White;
            btnLogout.Location = new Point(850, 30);
            btnLogout.Name = "btnLogout";
            btnLogout.Size = new Size(100, 35);
            btnLogout.TabIndex = 2;
            btnLogout.Text = "Çıkış";
            btnLogout.UseVisualStyleBackColor = false;
            btnLogout.Click += btnLogout_Click;
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(248, 249, 250);
            panel1.Controls.Add(btnAircraft);
            panel1.Controls.Add(btnAirlines);
            panel1.Controls.Add(btnAirports);
            panel1.Controls.Add(btnFlights);
            panel1.Controls.Add(label1);
            panel1.Location = new Point(30, 120);
            panel1.Name = "panel1";
            panel1.Size = new Size(920, 400);
            panel1.TabIndex = 3;
            // 
            // btnAircraft
            // 
            btnAircraft.BackColor = Color.FromArgb(40, 167, 69);
            btnAircraft.FlatStyle = FlatStyle.Flat;
            btnAircraft.ForeColor = Color.White;
            btnAircraft.Location = new Point(480, 200);
            btnAircraft.Name = "btnAircraft";
            btnAircraft.Size = new Size(180, 60);
            btnAircraft.TabIndex = 4;
            btnAircraft.Text = "Uçak Yönetimi";
            btnAircraft.UseVisualStyleBackColor = false;
            btnAircraft.Click += btnAircraft_Click;
            // 
            // btnAirlines
            // 
            btnAirlines.BackColor = Color.FromArgb(255, 193, 7);
            btnAirlines.FlatStyle = FlatStyle.Flat;
            btnAirlines.ForeColor = Color.Black;
            btnAirlines.Location = new Point(260, 200);
            btnAirlines.Name = "btnAirlines";
            btnAirlines.Size = new Size(180, 60);
            btnAirlines.TabIndex = 3;
            btnAirlines.Text = "Havayolu Yönetimi";
            btnAirlines.UseVisualStyleBackColor = false;
            btnAirlines.Click += btnAirlines_Click;
            // 
            // btnAirports
            // 
            btnAirports.BackColor = Color.FromArgb(108, 117, 125);
            btnAirports.FlatStyle = FlatStyle.Flat;
            btnAirports.ForeColor = Color.White;
            btnAirports.Location = new Point(480, 100);
            btnAirports.Name = "btnAirports";
            btnAirports.Size = new Size(180, 60);
            btnAirports.TabIndex = 2;
            btnAirports.Text = "Havaalanı Yönetimi";
            btnAirports.UseVisualStyleBackColor = false;
            btnAirports.Click += btnAirports_Click;
            // 
            // btnFlights
            // 
            btnFlights.BackColor = Color.FromArgb(0, 123, 255);
            btnFlights.FlatStyle = FlatStyle.Flat;
            btnFlights.ForeColor = Color.White;
            btnFlights.Location = new Point(260, 100);
            btnFlights.Name = "btnFlights";
            btnFlights.Size = new Size(180, 60);
            btnFlights.TabIndex = 1;
            btnFlights.Text = "Uçuş Yönetimi";
            btnFlights.UseVisualStyleBackColor = false;
            btnFlights.Click += btnFlights_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            label1.Location = new Point(30, 30);
            label1.Name = "label1";
            label1.Size = new Size(90, 25);
            label1.TabIndex = 0;
            label1.Text = "Modüller";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(984, 561);
            Controls.Add(panel1);
            Controls.Add(btnLogout);
            Controls.Add(lblRoles);
            Controls.Add(lblWelcome);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "AODB - Ana Sayfa";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblWelcome;
        private Label lblRoles;
        private Button btnLogout;
        private Panel panel1;
        private Label label1;
        private Button btnFlights;
        private Button btnAirports;
        private Button btnAirlines;
        private Button btnAircraft;
    }
}
