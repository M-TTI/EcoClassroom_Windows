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
    public partial class RegisterForm : Form
    {
        private readonly AuthService _authService;
        private TextBox txtEmail;
        private TextBox txtPassword;
        private TextBox txtConfirmPassword;
        private TextBox txtFirstName;
        private TextBox txtLastName;
        private Button btnCreate;
        private Button btnCancel;

        public RegisterForm()
        {
            InitializeComponent(); // Appel du Designer généré par Visual Studio
            _authService = new AuthService(new AppDbContext());
            SetupCustomControls();
        }

        private void SetupCustomControls()
        {
            this.Text = "Créer un compte professeur";
            this.Size = new Size(480, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Nettoyer les contrôles par défaut
            this.Controls.Clear();

            // Titre
            var lblTitle = new Label
            {
                Text = "Création de compte",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Size = new Size(420, 40),
                Location = new Point(30, 20),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.DarkGreen
            };

            // Prénom
            var lblFirstName = new Label
            {
                Text = "Prénom:",
                Location = new Point(40, 80),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 10)
            };

            txtFirstName = new TextBox
            {
                Location = new Point(40, 105),
                Size = new Size(380, 30),
                Font = new Font("Segoe UI", 11)
            };

            // Nom
            var lblLastName = new Label
            {
                Text = "Nom:",
                Location = new Point(40, 145),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 10)
            };

            txtLastName = new TextBox
            {
                Location = new Point(40, 170),
                Size = new Size(380, 30),
                Font = new Font("Segoe UI", 11)
            };

            // Email
            var lblEmail = new Label
            {
                Text = "Email:",
                Location = new Point(40, 210),
                Size = new Size(120, 20),
                Font = new Font("Segoe UI", 10)
            };

            txtEmail = new TextBox
            {
                Location = new Point(40, 235),
                Size = new Size(380, 30),
                Font = new Font("Segoe UI", 11)
            };

            // Mot de passe
            var lblPassword = new Label
            {
                Text = "Mot de passe:",
                Location = new Point(40, 275),
                Size = new Size(120, 20),
                Font = new Font("Segoe UI", 10)
            };

            txtPassword = new TextBox
            {
                Location = new Point(40, 300),
                Size = new Size(380, 30),
                PasswordChar = '*',
                Font = new Font("Segoe UI", 11)
            };

            // Confirmation mot de passe
            var lblConfirmPassword = new Label
            {
                Text = "Confirmer le mot de passe:",
                Location = new Point(40, 340),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI", 10)
            };

            txtConfirmPassword = new TextBox
            {
                Location = new Point(40, 365),
                Size = new Size(380, 30),
                PasswordChar = '*',
                Font = new Font("Segoe UI", 11)
            };

            // Boutons
            btnCreate = new Button
            {
                Text = "Créer le compte",
                Location = new Point(120, 420),
                Size = new Size(140, 40),
                BackColor = Color.SeaGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            btnCancel = new Button
            {
                Text = "Annuler",
                Location = new Point(280, 420),
                Size = new Size(140, 40),
                BackColor = Color.Gray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                DialogResult = DialogResult.Cancel
            };

            // Événements
            btnCreate.Click += BtnCreate_Click;
            this.CancelButton = btnCancel;

            // Ajouter les contrôles
            this.Controls.AddRange(new Control[] {
                lblTitle, lblFirstName, txtFirstName, lblLastName, txtLastName,
                lblEmail, txtEmail, lblPassword, txtPassword,
                lblConfirmPassword, txtConfirmPassword, btnCreate, btnCancel
            });
        }

        private async void BtnCreate_Click(object sender, EventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                string.IsNullOrWhiteSpace(txtLastName.Text) ||
                string.IsNullOrWhiteSpace(txtEmail.Text) ||
                string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Veuillez remplir tous les champs.", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validation de l'email
            if (!IsValidEmail(txtEmail.Text))
            {
                MessageBox.Show("Veuillez entrer une adresse email valide.", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtPassword.Text != txtConfirmPassword.Text)
            {
                MessageBox.Show("Les mots de passe ne correspondent pas.", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtPassword.Text.Length < 4)
            {
                MessageBox.Show("Le mot de passe doit contenir au moins 4 caractères.", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnCreate.Enabled = false;
            btnCreate.Text = "Création...";

            try
            {
                bool success = await _authService.RegisterAsync(
                    txtEmail.Text.Trim().ToLower(),
                    txtPassword.Text,
                    txtFirstName.Text.Trim(),
                    txtLastName.Text.Trim());

                if (success)
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Cette adresse email est déjà utilisée.", "Erreur",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la création du compte : {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnCreate.Enabled = true;
                btnCreate.Text = "Créer le compte";
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
    }
}
