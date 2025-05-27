using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EcoClassroom_Windows.Data;
using EcoClassroom_Windows.Services;

namespace EcoClassroom_Windows.Forms
{
    public partial class LoginForm : Form
    {
        private readonly AuthService _authService;
        private TextBox txtEmail;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnRegister;
        private Label lblTitle;
        private Panel pnlLogin;

        public LoginForm()
        {
            _authService = new AuthService(new AppDbContext());
            InitializeComponent();
            SetupCustomControls();
        }

        private void SetupCustomControls()
        {
            this.Text = "Gestion Transport Scolaire - Connexion";
            this.Size = new Size(450, 350);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            // Nettoyer les contrôles existants
            this.Controls.Clear();

            // Panel principal
            pnlLogin = new Panel
            {
                Size = new Size(380, 220),
                Location = new Point(35, 70),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

            // Titre
            lblTitle = new Label
            {
                Text = "Connexion Professeur",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Size = new Size(380, 40),
                Location = new Point(35, 20),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.DarkBlue
            };

            // Email
            var lblEmail = new Label
            {
                Text = "Email:",
                Location = new Point(30, 30),
                Size = new Size(120, 20),
                Font = new Font("Segoe UI", 10)
            };

            txtEmail = new TextBox
            {
                Location = new Point(30, 55),
                Size = new Size(320, 30),
                Font = new Font("Segoe UI", 11)
            };

            // Mot de passe
            var lblPassword = new Label
            {
                Text = "Mot de passe:",
                Location = new Point(30, 95),
                Size = new Size(120, 20),
                Font = new Font("Segoe UI", 10)
            };

            txtPassword = new TextBox
            {
                Location = new Point(30, 120),
                Size = new Size(320, 30),
                PasswordChar = '*',
                Font = new Font("Segoe UI", 11)
            };

            // Boutons
            btnLogin = new Button
            {
                Text = "Se connecter",
                Location = new Point(30, 170),
                Size = new Size(140, 40),
                BackColor = Color.DodgerBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            btnRegister = new Button
            {
                Text = "Créer un compte",
                Location = new Point(190, 170),
                Size = new Size(140, 40),
                BackColor = Color.SeaGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            // Boutons
            btnLogin = new Button
            {
                Text = "Se connecter",
                Location = new Point(30, 170),
                Size = new Size(140, 40),
                BackColor = Color.DodgerBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            btnRegister = new Button
            {
                Text = "Créer un compte",
                Location = new Point(190, 170),
                Size = new Size(140, 40),
                BackColor = Color.SeaGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            // Événements
            btnLogin.Click += BtnLogin_Click;
            btnRegister.Click += BtnRegister_Click;
            txtPassword.KeyPress += TxtPassword_KeyPress;

            // Ajout des contrôles
            pnlLogin.Controls.AddRange(new Control[] {
                lblEmail, txtEmail, lblPassword, txtPassword, btnLogin, btnRegister
            });

            this.Controls.AddRange(new Control[] {
                lblTitle, pnlLogin
            });
        }

        private void TxtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                BtnLogin_Click(sender, e);
            }
        }

        private async void BtnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Veuillez remplir tous les champs.", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validation basique de l'email
            if (!IsValidEmail(txtEmail.Text))
            {
                MessageBox.Show("Veuillez entrer une adresse email valide.", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnLogin.Enabled = false;
            btnLogin.Text = "Connexion...";

            try
            {
                var user = await _authService.LoginAsync(txtEmail.Text, txtPassword.Text);

                if (user != null)
                {
                    this.Hide();
                    var mainForm = new MainForm(user);
                    mainForm.FormClosed += (s, args) => this.Close();
                    mainForm.Show();
                }
                else
                {
                    MessageBox.Show("Email ou mot de passe incorrect.", "Erreur",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la connexion : {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnLogin.Enabled = true;
                btnLogin.Text = "Se connecter";
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            var registerForm = new RegisterForm();
            if (registerForm.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show("Compte créé avec succès ! Vous pouvez maintenant vous connecter.",
                    "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {

        }
    }
}
