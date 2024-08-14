﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;

namespace Projets1erSessionCSharp
{
    public partial class EnseignantEdit : Form
    {
        //1. Connection Sql server
        public static string connectionString = "Data Source=(localdb)\\Projects;Initial Catalog=db_projet;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False";

        // Liste des faculte et cours
        Dictionary<string, List<string>> Faculte = new Dictionary<string, List<string>>
        {
            { "Choisir", new List<string> { "Choisir un faculter d'abord" } },
            { "Genie Civil", new List<string> { "Dessin Technique", "Cartographie", "Math Logique", "Algebre lineaire" } },
            { "Informatique", new List<string> { "React", "Python", "Kotlin", "Swift" } },
            { "Communication", new List<string> { "Art Oratoire", "Micro trottroire", "Grammaire", "Protocole" } },
            { "Administration", new List<string> { "Droit Adminisatrative", "Statistique", "Comptabilite", "informatique de bureau" } },
            { "Bio-Medicale", new List<string> { "Biologie", "Micro-biologie", "Analyse", "Synthese" } },
            { "Infirmiere", new List<string> { "Anatomie", "Nursing", "Pediatrie", "Orthopedie" } },
           
        };

        string ID;
        int verification;

        public EnseignantEdit(string NId, int Nver)
        {
            InitializeComponent();
            txtFaculte.Items.AddRange(Faculte.Keys.ToArray());
            verification = Nver;
            ID = NId;
        }

        public void ShowToast(string type, string message)
        {
            ToastForm toast = new ToastForm(type, message);
            toast.Show();
        }

        private void EnseignantEdit_Load(object sender, EventArgs e)
        {
            panelMdp.Hide();
            if (verification == 1)
            {
                btnClear.Hide();
                lblMdpUpdate.Show();
                picturePanel.Show();
                btnBack.Hide();
            }
            else
            {
                btnBack.Show();
                lblMdpUpdate.Hide();
                picturePanel.Hide();
            }
            searchData();
        }

        public void searchData()
        {
            try
            {
                //2. Etablir la connection
                SqlConnection connection = new SqlConnection(connectionString);

                //3. Ouvrir la conection 
                connection.Open();

                //4. Requete
                string query = "SELECT * FROM enseignant WHERE code = '" + ID + "' ORDER BY code DESC";

                //5. Executer la requete
                SqlCommand command = new SqlCommand(query, connection);
                var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    txtNom.Text = reader["nom"].ToString();
                    txtPrenom.Text = reader["prenom"].ToString();
                    txtEmail.Text = reader["email"].ToString();
                    txtTel.Text = reader["telephone"].ToString();
                    txtSexe.Text = reader["sexe"].ToString();
                    txtFaculte.Text = reader["faculte"].ToString();
                    txtCours.Text = reader["cours"].ToString();
                    txtAdresse.Text = reader["adresse"].ToString();
                    byte[] imageBytes = (byte[])reader["photos"];
                    using (MemoryStream ms = new MemoryStream(imageBytes))
                    {
                        picProfileUpdate.Image = Image.FromStream(ms);
                    }
                }
                reader.Close();
                //6. Connection fermer
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }



        }

        public void pictureUpdated()
        {
            //2. Etablir la connection
            SqlConnection connection = new SqlConnection(connectionString);

            //3. Ouvrir la conection 
            connection.Open();

            //4. Requete
            byte[] images = null;
            FileStream Stream = new FileStream(imgLocation, FileMode.Open, FileAccess.Read);
            BinaryReader brs = new BinaryReader(Stream);
            images = brs.ReadBytes((int)Stream.Length);

            string query = "UPDATE enseignant set photos = @photos WHERE code = @code";

            //5. Executer la requete
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@photos", images);
            command.Parameters.AddWithValue("@code", ID);

            try
            {
                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    ShowToast("SUCCESS", "Modification réussie");
                }
                else
                {
                    ShowToast("ERROR", "Modification échouée");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        string imgLocation = "";
        private void btnEditPicture_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "png files(*.png)|*.png|jpg files(*.jpg)|*.jpg|All files(*.*)|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                picProfileUpdate.Image = new Bitmap(openFileDialog.FileName);
                btnEditPicture.Text = openFileDialog.FileName;
                imgLocation = openFileDialog.FileName.ToString();
                picProfileUpdate.ImageLocation = imgLocation;
                pictureUpdated();

            }
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            //2. Etablir la connection
            SqlConnection connection = new SqlConnection(connectionString);

            //3. Ouvrir la conection 
            connection.Open();

            //4. Requete
            string query = "UPDATE enseignant set nom = @nom, prenom = @prenom, email = @email, adresse = @adresse, faculte = @faculte, cours = @cours, telephone = @telephone, sexe = @sexe WHERE code = @code";

            //5. Executer la requete
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@nom", txtNom.Text);
            command.Parameters.AddWithValue("@prenom", txtPrenom.Text);
            command.Parameters.AddWithValue("@email", txtEmail.Text);
            command.Parameters.AddWithValue("@adresse", txtAdresse.Text);
            command.Parameters.AddWithValue("@telephone", txtTel.Text);
            command.Parameters.AddWithValue("@faculte", txtFaculte.Text);
            command.Parameters.AddWithValue("@cours", txtCours.Text);
            command.Parameters.AddWithValue("@sexe", txtSexe.Text);
            command.Parameters.AddWithValue("@code", ID);
            try
            {
                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    ShowToast("SUCCESS", "Modification réussie");
                    if (verification == 0)
                    {
                        this.Close();
                    }

                }
                else
                {
                    ShowToast("ERROR", "Modification échouée");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        private void btnMdpUpdate_Click(object sender, EventArgs e)
        {
            string mdp = txtMdpUpdate.Text;
            string rmdp = txtRepeteMdpUpdate.Text;

            if (mdp == rmdp)
            {
                if (mdp == "" || rmdp == "")
                {
                    ShowToast("INFO", "Veuillez remplir le champ mot de passe");
                    txtMdpUpdate.Focus();
                    txtRepeteMdpUpdate.Focus();
                }
                else
                {
                    //2. Etablir la connection
                    SqlConnection connection = new SqlConnection(connectionString);

                    //3. Ouvrir la conection 
                    connection.Open();

                    //4. Requete
                    string query = "UPDATE enseignant set mdp = @mdp WHERE code = @code";

                    //5. Executer la requete
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@mdp", rmdp);
                    command.Parameters.AddWithValue("@code", ID);

                    try
                    {
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            ShowToast("SUCCESS", "Modification mot de passe réussie");
                        }
                        else
                        {
                            ShowToast("ERROR", "Modification mot de passe échouée");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            else
            {
                ShowToast("ERROR", "Mot de passe incohérent");
            }
        }

        public void disabled(bool boolVar)
        {
            txtNom.Enabled = boolVar;
            txtPrenom.Enabled = boolVar;
            txtEmail.Enabled = boolVar;
            txtTel.Enabled = boolVar;
            txtSexe.Enabled = boolVar;
            txtFaculte.Enabled = boolVar;
            txtCours.Enabled = boolVar;
            txtAdresse.Enabled = boolVar;
            picturePanel.Enabled = boolVar;
            btnCreate.Enabled = boolVar;
            btnClear.Enabled = boolVar;
            lblMdpUpdate.Enabled = boolVar;
            btnBack.Enabled = boolVar;
        }

        private void lblMdpUpdate_Click(object sender, EventArgs e)
        {
            panelMdp.Show();
            disabled(false);
        }

        private void btnPanelMdp_Click(object sender, EventArgs e)
        {
            panelMdp.Hide();
            disabled(true);
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void chkMdp_CheckedChanged(object sender, EventArgs e)
        {
            if (chkMdp.Checked)
            {
                txtMdpUpdate.PasswordChar = (char)0;
                txtRepeteMdpUpdate.PasswordChar = (char)0;
            }
            else
            {
                txtMdpUpdate.PasswordChar = Convert.ToChar("*");
                txtRepeteMdpUpdate.PasswordChar = Convert.ToChar("*");
            }
        }

        private void txtFaculte_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Effacer les éléments précédemment chargés dans le ComboBox des cours
            txtCours.Items.Clear();

            // Récupérer les facultes sélectionné dans le ComboBox des facultes
            string selectedCours = txtFaculte.SelectedItem.ToString();

            // Vérifier si la faculte sélectionné a des cours associes
            if (Faculte.ContainsKey(selectedCours))
            {
                // Charger les cours associées dans le ComboBox des cours
                txtCours.Items.AddRange(Faculte[selectedCours].ToArray());
            }
            else
            {
                // Si aucune correspondance n'est trouvée, afficher un message
                txtCours.Items.Add("Aucun cours n'est disponible");
            }
        }
    }
}