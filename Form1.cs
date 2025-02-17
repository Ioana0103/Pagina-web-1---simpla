﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;


namespace Proiect1
{
    public partial class Form1 : Form
    {
        private SQLiteConnection conn;
        private List<string> listBlock = new List<string>();
        public Form1()
        {
            InitializeComponent();
            InitializeDatabase();
            LoadBlockedKeywords();
        }

        private void InitializeDatabase()
        {
            conn = new SQLiteConnection("Data Source=keywords.db;Version=3");
            try
            {
                conn.Open();
                MessageBox.Show("Conectarea la baza de date este reusita!");

            }
            catch (Exception ex)
            {
                MessageBox.Show("Eroare la conectarea la baza de date:" + ex.Message);
            }
        }

        private void LoadBlockedKeywords()
        {
            listBlock.Clear();
            string selectQuery = "SELECT Keyword FROM BlockedKeywords";

            try
            {
                using (var cmd = new SQLiteCommand(selectQuery, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        listBlock.Add(reader.GetString(0));
                    }
                }
                comboBox1.DataSource = null;
                comboBox1.DataSource = new BindingSource(listBlock, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eroare la incarcarea cuvintelor cheie: " + ex.Message);
            }
        }

        private void AddKeyword(string keyword)
        {
            try
            {
                string insertQuery = "INSERT INTO BlockedKeywords (Keyword) VALUES (@Keyword)";
                using (var cmd = new SQLiteCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Keyword", keyword);
                    cmd.ExecuteNonQuery();
                }
                LoadBlockedKeywords();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eroare la adaugarea cuvantului cheie: " + ex.Message);

            }
        }

        private void DeleteKeyword(string keyword)
        {
            try
            {
                string deleteQuery = "DELETE FROM BlockedKeywords WHERE Keyword = @Keyword";
                using (var cmd = new SQLiteCommand(deleteQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Keyword", keyword);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Cuvantul cheie a fost sters cu succes.");
                    }
                    else { MessageBox.Show("Nu s-a gasit cuvantul cheie pentru stergere."); }
                }
                LoadBlockedKeywords();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eroare la stergerea cuvantului cheie: " + ex.Message);
            }
        }

        //Butonul de go
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            String url = toolStripTextBox1.Text;
            webBrowser1.Navigate(url);
        }

        //Butonul de back
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (webBrowser1.CanGoBack)
            {
                webBrowser1.GoBack();
            }
        }


        //Butonul de forward
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (webBrowser1.CanGoForward)
            {
                webBrowser1.GoForward();
            }
        }

        //Butonul de home
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            webBrowser1.GoHome();
        }

        private void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            string url = e.Url.ToString();
            var blockedKeywords = listBlock;

            if (blockedKeywords.Any(keyword => url.Contains(keyword)))
            {
                e.Cancel = true;
                MessageBox.Show("URL-ul contine cuvinte cheie interzise!");
            }
        }

        private void toolStripTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) 
            {
                string url = toolStripTextBox1.Text; 
                webBrowser1.Navigate(url);
            }
        }


        //Buton pt adaugare cuvant cheie
        private void button1_Click(object sender, EventArgs e)
        {
            string keyword = textBox1.Text;
            if (!string.IsNullOrEmpty(keyword))
            {
                AddKeyword(keyword); 
                textBox1.Clear();
            }
            else
            {
                MessageBox.Show("Te rog sa introduci un cuvant cheie.");
            }
        }


        //Buton pt stergere cuvant cheie
        private void button2_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                string keywordToDelete = comboBox1.SelectedItem.ToString();
                DeleteKeyword(keywordToDelete);
            }
            else
            {
                MessageBox.Show("Te rog sa selectezi un cuvant cheie.");
            }
        }
    }
}
