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
using EcoClassroom_Windows.Models;
using EcoClassroom_Windows.Services;

namespace EcoClassroom_Windows.Forms
{
    public partial class MainForm : Form
    {
        private readonly User _currentUser;
        private readonly StudentService _studentService;
        private readonly AttendanceService _attendanceService;

        // Contrôles interface
        private TabControl tabControl;
        private TabPage tabStudents;
        private TabPage tabAttendance;
        private TabPage tabStats;

        // Onglet Élèves
        private ListBox lstStudents;
        private TextBox txtFirstName;
        private TextBox txtLastName;
        private Button btnAddStudent;
        private Button btnDeleteStudent;

        // Onglet Appel
        private DateTimePicker dtpAttendanceDate;
        private DataGridView dgvAttendance;
        private Button btnSaveAttendance;
        private Label lblDailyAverage;

        // Onglet Statistiques
        private Label lblWeeklyAverage;
        private Label lblWeeklyStats;
        private DateTimePicker dtpWeekStart;
        private Button btnCalculateStats;

        public MainForm(User currentUser)
        {
            _currentUser = currentUser;
            _studentService = new StudentService(new AppDbContext());
            _attendanceService = new AttendanceService(new AppDbContext());

            InitializeComponent();
            SetupCustomControls();
            LoadStudents();
        }

        private void SetupCustomControls()
        {
            this.Text = $"Gestion Transport Scolaire - {_currentUser.FirstName} {_currentUser.LastName}";
            this.Size = new Size(1000, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(800, 600);

            this.Controls.Clear();

            // TabControl principal
            tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10)
            };

            SetupAttendanceTab();
            SetupStudentsTab();
            SetupStatsTab();

            tabControl.TabPages.AddRange(new TabPage[] { tabAttendance, tabStudents, tabStats });
            this.Controls.Add(tabControl);

            // Charger automatiquement la grille d'appel pour aujourd'hui
            this.Load += async (s, e) => await LoadAttendanceGrid();

            // Menu
            var menuStrip = new MenuStrip();
            var fileMenu = new ToolStripMenuItem("Fichier");
            var logoutMenuItem = new ToolStripMenuItem("Déconnexion");
            logoutMenuItem.Click += (s, e) =>
            {
                this.Hide();
                new LoginForm().Show();
                this.Close();
            };
            fileMenu.DropDownItems.Add(logoutMenuItem);
            menuStrip.Items.Add(fileMenu);
            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);
        }

        private void SetupStudentsTab()
        {
            tabStudents = new TabPage("Gestion des Élèves");

            // Liste des élèves
            var lblStudents = new Label
            {
                Text = "Liste des élèves:",
                Location = new Point(20, 20),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };

            lstStudents = new ListBox
            {
                Location = new Point(20, 50),
                Size = new Size(400, 400),
                Font = new Font("Segoe UI", 10)
            };

            // Ajout d'élèves
            var lblAdd = new Label
            {
                Text = "Ajouter un élève:",
                Location = new Point(450, 20),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };

            var lblFirstName = new Label
            {
                Text = "Prénom:",
                Location = new Point(450, 60),
                Size = new Size(100, 20)
            };

            txtFirstName = new TextBox
            {
                Location = new Point(450, 85),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 10)
            };

            var lblLastName = new Label
            {
                Text = "Nom:",
                Location = new Point(450, 120),
                Size = new Size(100, 20)
            };

            txtLastName = new TextBox
            {
                Location = new Point(450, 145),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 10)
            };

            btnAddStudent = new Button
            {
                Text = "Ajouter",
                Location = new Point(450, 185),
                Size = new Size(100, 35),
                BackColor = Color.DodgerBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            btnDeleteStudent = new Button
            {
                Text = "Supprimer",
                Location = new Point(322, 450),
                Size = new Size(100, 35),
                BackColor = Color.Crimson,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            btnAddStudent.Click += BtnAddStudent_Click;
            btnDeleteStudent.Click += BtnDeleteStudent_Click;

            tabStudents.Controls.AddRange(new Control[] {
                lblStudents, lstStudents, lblAdd, lblFirstName, txtFirstName,
                lblLastName, txtLastName, btnAddStudent, btnDeleteStudent
            });
        }

        private void SetupAttendanceTab()
        {
            tabAttendance = new TabPage("Faire l'Appel");

            // Sélection de date
            var lblDate = new Label
            {
                Text = "Date:",
                Location = new Point(20, 20),
                Size = new Size(100, 25),
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };

            dtpAttendanceDate = new DateTimePicker
            {
                Location = new Point(80, 20),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 10),
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today
            };
            dtpAttendanceDate.ValueChanged += DtpAttendanceDate_ValueChanged;

            // Note explicative
            var lblNote = new Label
            {
                Text = "Instructions: Cochez 'Présent' puis sélectionnez le mode de transport. Laissez décoché si absent.",
                Location = new Point(20, 60),
                Size = new Size(800, 40),
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                ForeColor = Color.DarkBlue
            };

            // Label pour la moyenne du jour
            lblDailyAverage = new Label
            {
                Text = "Moyenne du jour: - ",
                Location = new Point(20, 580),
                Size = new Size(400, 30),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                TextAlign = ContentAlignment.MiddleLeft
            };

            // DataGridView pour l'appel
            dgvAttendance = new DataGridView
            {
                Location = new Point(20, 110),
                Size = new Size(820, 400),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                Font = new Font("Segoe UI", 10)
            };

            btnSaveAttendance = new Button
            {
                Text = "Enregistrer l'Appel",
                Location = new Point(350, 530),
                Size = new Size(180, 40),
                BackColor = Color.Green,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            btnSaveAttendance.Click += BtnSaveAttendance_Click;

            tabAttendance.Controls.AddRange(new Control[] {
                lblDate, dtpAttendanceDate, lblNote, dgvAttendance, btnSaveAttendance, lblDailyAverage
            });
        }

        private void SetupStatsTab()
        {
            tabStats = new TabPage("Statistiques");

            var lblWeekSelect = new Label
            {
                Text = "Sélectionner le début de semaine (lundi):",
                Location = new Point(20, 20),
                Size = new Size(300, 25),
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };

            dtpWeekStart = new DateTimePicker
            {
                Location = new Point(320, 20),
                Size = new Size(200, 25),
                Font = new Font("Segoe UI", 10),
                Format = DateTimePickerFormat.Short
            };

            btnCalculateStats = new Button
            {
                Text = "Calculer",
                Location = new Point(540, 18),
                Size = new Size(100, 30),
                BackColor = Color.Purple,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnCalculateStats.Click += BtnCalculateStats_Click;

            lblWeeklyAverage = new Label
            {
                Text = "Moyenne hebdomadaire: - ",
                Location = new Point(20, 80),
                Size = new Size(800, 40),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.DarkGreen
            };

            lblWeeklyStats = new Label
            {
                Text = "Détails des statistiques apparaîtront ici...",
                Location = new Point(20, 130),
                Size = new Size(800, 400),
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.LightGray,
                Padding = new Padding(10)
            };

            tabStats.Controls.AddRange(new Control[] {
                lblWeekSelect, dtpWeekStart, btnCalculateStats, lblWeeklyAverage, lblWeeklyStats
            });
        }

        private async void LoadStudents()
        {
            try
            {
                var students = await _studentService.GetStudentsByUserIdAsync(_currentUser.Id);
                lstStudents.Items.Clear();

                foreach (var student in students)
                {
                    lstStudents.Items.Add($"{student.LastName}, {student.FirstName}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement des élèves : {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnAddStudent_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                MessageBox.Show("Veuillez remplir le prénom et le nom.", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                bool success = await _studentService.AddStudentAsync(
                    _currentUser.Id,
                    txtFirstName.Text.Trim(),
                    txtLastName.Text.Trim());

                if (success)
                {
                    txtFirstName.Clear();
                    txtLastName.Clear();
                    LoadStudents();

                    await LoadAttendanceGrid(); // met a jour la grille d'appel

                    MessageBox.Show("Élève ajouté avec succès !", "Succès",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Erreur lors de l'ajout de l'élève.", "Erreur",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur : {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnDeleteStudent_Click(object sender, EventArgs e)
        {
            if (lstStudents.SelectedIndex == -1)
            {
                MessageBox.Show("Veuillez sélectionner un élève à supprimer.", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var result = MessageBox.Show("Êtes-vous sûr de vouloir supprimer cet élève ? Toutes ses données seront perdues.",
                "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    var students = await _studentService.GetStudentsByUserIdAsync(_currentUser.Id);
                    var selectedStudent = students[lstStudents.SelectedIndex];

                    bool success = await _studentService.DeleteStudentAsync(selectedStudent.Id, _currentUser.Id);

                    if (success)
                    {
                        LoadStudents();

                        await LoadAttendanceGrid(); // met a jour la grille d'appel

                        MessageBox.Show("Élève supprimé avec succès !", "Succès",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur : {ex.Message}", "Erreur",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void DtpAttendanceDate_ValueChanged(object sender, EventArgs e)
        {
            await LoadAttendanceGrid();
        }

        private async Task LoadAttendanceGrid()
        {
            try
            {
                var students = await _studentService.GetStudentsByUserIdAsync(_currentUser.Id);
                var attendances = await _attendanceService.GetAttendancesByDateAsync(_currentUser.Id, dtpAttendanceDate.Value);

                // Nettoyer et reconfigurer le DataGridView
                dgvAttendance.Columns.Clear();
                dgvAttendance.Rows.Clear();

                // Ajouter les colonnes
                dgvAttendance.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "StudentId",
                    HeaderText = "ID",
                    Visible = false
                });

                dgvAttendance.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "StudentName",
                    HeaderText = "Nom de l'élève",
                    Width = 200,
                    ReadOnly = true
                });

                dgvAttendance.Columns.Add(new DataGridViewCheckBoxColumn
                {
                    Name = "IsAbsent",
                    HeaderText = "Absent",
                    Width = 80
                });

                dgvAttendance.Columns.Add(new DataGridViewComboBoxColumn
                {
                    Name = "TransportMode",
                    HeaderText = "Mode de transport",
                    Width = 200,
                    Items = { "1 - Voiture", "2 - Covoiturage", "3 - Transport en commun", "4 - Vélo", "5 - À pied" }
                });

                // Ajouter les lignes pour chaque élève
                foreach (var student in students)
                {
                    var attendance = attendances.FirstOrDefault(a => a.StudentId == student.Id);

                    var rowIndex = dgvAttendance.Rows.Add();
                    var row = dgvAttendance.Rows[rowIndex];

                    row.Cells["StudentId"].Value = student.Id;
                    row.Cells["StudentName"].Value = $"{student.LastName}, {student.FirstName}";

                    // Logique inversée : coché = absent, non coché = présent
                    bool isAbsent = attendance != null ? !attendance.IsPresent : false;
                    row.Cells["IsAbsent"].Value = isAbsent;

                    if (attendance?.IsPresent == true && attendance.TransportMode.HasValue)
                    {
                        row.Cells["TransportMode"].Value = $"{attendance.TransportMode.Value} - {TransportModeHelper.GetDescription(attendance.TransportMode.Value)}";
                    }
                    else if (!isAbsent) // Si présent (pas absent) mais pas encore de transport, mettre par défaut
                    {
                        row.Cells["TransportMode"].Value = "5 - À pied"; // Transport par défaut
                    }
                    else
                    {
                        row.Cells["TransportMode"].Value = null; // Absent = pas de transport
                    }
                }

                // Événement pour gérer la logique présent/absent
                dgvAttendance.CellValueChanged += DgvAttendance_CellValueChanged;

                // Calculer et afficher la moyenne du jour
                await UpdateDailyAverage();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement de l'appel : {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task UpdateDailyAverage()
        {
            try
            {
                var attendances = await _attendanceService.GetAttendancesByDateAsync(_currentUser.Id, dtpAttendanceDate.Value);
                var presentAttendances = attendances.Where(a => a.IsPresent && a.TransportMode.HasValue).ToList();

                if (presentAttendances.Any())
                {
                    var average = presentAttendances.Average(a => a.TransportMode.Value);
                    lblDailyAverage.Text = $"Moyenne du jour: {average:F2}/5 ({presentAttendances.Count} présent{(presentAttendances.Count > 1 ? "s" : "")})";

                    // Couleur selon la performance
                    if (average >= 4.0)
                        lblDailyAverage.ForeColor = Color.Green;
                    else if (average >= 3.0)
                        lblDailyAverage.ForeColor = Color.Orange;
                    else
                        lblDailyAverage.ForeColor = Color.Red;
                }
                else
                {
                    lblDailyAverage.Text = "Moyenne du jour: Aucune donnée";
                    lblDailyAverage.ForeColor = Color.Gray;
                }
            }
            catch (Exception ex)
            {
                lblDailyAverage.Text = "Moyenne du jour: Erreur de calcul";
                lblDailyAverage.ForeColor = Color.Red;
            }
        }

        private void DgvAttendance_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // Si on change la case "Absent"
            if (dgvAttendance.Columns[e.ColumnIndex].Name == "IsAbsent")
            {
                var row = dgvAttendance.Rows[e.RowIndex];
                var isAbsent = Convert.ToBoolean(row.Cells["IsAbsent"].Value);

                if (isAbsent)
                {
                    // Si absent, vider le mode de transport
                    row.Cells["TransportMode"].Value = null;
                }
                else
                {
                    // Si présent, proposer un mode de transport par défaut
                    if (row.Cells["TransportMode"].Value == null)
                    {
                        row.Cells["TransportMode"].Value = "5 - À pied";
                    }
                }
            }
        }

        private async void BtnSaveAttendance_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in dgvAttendance.Rows)
                {
                    if (row.IsNewRow) continue;

                    var studentId = Convert.ToInt32(row.Cells["StudentId"].Value);
                    var isAbsent = Convert.ToBoolean(row.Cells["IsAbsent"].Value);
                    var isPresent = !isAbsent; // c'est long de tt changer

                    if (isPresent)
                    {
                        var transportText = row.Cells["TransportMode"].Value?.ToString();
                        if (string.IsNullOrEmpty(transportText))
                        {
                            MessageBox.Show($"Veuillez sélectionner un mode de transport pour tous les élèves présents.",
                                "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        var transportMode = int.Parse(transportText.Split(' ')[0]); // format: "5 - À pied"
                        await _attendanceService.MarkStudentPresentAsync(studentId, dtpAttendanceDate.Value, transportMode);
                    }
                    else
                    {
                        await _attendanceService.MarkStudentAbsentAsync(studentId, dtpAttendanceDate.Value);
                    }
                }

                MessageBox.Show("Appel enregistré avec succès !", "Succès",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                await UpdateDailyAverage();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'enregistrement : {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnCalculateStats_Click(object sender, EventArgs e)
        {
            try
            {
                var weekStart = dtpWeekStart.Value.Date;
                var stats = await _attendanceService.GetWeeklyStatsAsync(_currentUser.Id, weekStart);
                var average = await _attendanceService.GetWeeklyAverageAsync(_currentUser.Id, weekStart);

                lblWeeklyAverage.Text = $"Moyenne hebdomadaire: {average:F2}/5";

                var statsText = $"Statistiques pour la semaine du {weekStart:dd/MM/yyyy}\n\n";
                statsText += $"Nombre total d'élèves: {stats.TotalStudents}\n";
                statsText += $"Présences totales: {stats.TotalPresences}\n";
                statsText += $"Absences totales: {stats.TotalAbsences}\n\n";
                statsText += "Répartition des modes de transport:\n";

                foreach (var transport in stats.TransportModeBreakdown.OrderByDescending(kvp => kvp.Key))
                {
                    var modeNumber = transport.Key;
                    statsText += $"• {TransportModeHelper.GetDescription(modeNumber)}: {transport.Value} fois\n";
                }

                lblWeeklyStats.Text = statsText;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du calcul des statistiques : {ex.Message}",
                    "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
