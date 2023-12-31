﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GymManagementSystem
{
    public partial class UserManagementForm : Form
    {
        SqlConnection conn = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\ASUS\\Documents\\GymManagement.mdf;Integrated Security=True;Connect Timeout=30");
        public UserManagementForm()
        {
            InitializeComponent();
        }

        private void UserManagementForm_Load(object sender, EventArgs e)
        {
            fetchUserData("FormLoad");
        }

        // fetch all the user data
        private void fetchUserData(string methodName)
        {
            try
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("SELECT * FROM Users", conn);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                DataTable dataTable = new DataTable();

                adapter.Fill(dataTable);

                conn.Close();

                if (dataTable.Rows.Count > 0)
                {

                    if (methodName == "FormLoad" || methodName=="Refresh")
                    {
                        if (methodName == "Refresh")
                        {
                            dataGridView1.Columns.Clear();
                        }
                        dataGridView1.DataSource = dataTable;
                        // Add columns to the DataGridView            
                        DataGridViewButtonColumn editButtonColumn = new DataGridViewButtonColumn();
                        editButtonColumn.Text = "Edit";
                        editButtonColumn.UseColumnTextForButtonValue = true;
                        // Set the button column's cell style to have a green background color
                        editButtonColumn.DefaultCellStyle.BackColor = Color.Green;
                        dataGridView1.Columns.Add(editButtonColumn);

                        DataGridViewButtonColumn deleteButtonColumn = new DataGridViewButtonColumn();
                        deleteButtonColumn.Text = "Delete";
                        deleteButtonColumn.UseColumnTextForButtonValue = true;
                        deleteButtonColumn.DefaultCellStyle.BackColor = Color.Red;
                        dataGridView1.Columns.Add(deleteButtonColumn);
                    }

                }

            }

            catch (Exception ex)

            {

                MessageBox.Show("Error: " + ex.Message);

            }
        }

        private void UserManagementForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            MainMenuForm mainMenuForm = new MainMenuForm();
            mainMenuForm.Show();
            this.Hide();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 4)
            {
                //edit case
                int id = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
                string userName = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                string password = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
                string role = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();

                // redirect to the edit user form with the parameters
                EditUserForm editUserForm = new EditUserForm(id, userName, password, role);
                editUserForm.Show();
                this.Hide();
            }
            else if (e.ColumnIndex == 5)
            {
                //delete case
                int id = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
                User_Delete(id);
            }
        }
        private void User_Delete(int userID)
        {
            DialogResult result = MessageBox.Show("Are you sure to delete this user?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("DELETE FROM Users WHERE User_ID=@userID", conn);

                    cmd.Parameters.AddWithValue("@userID", userID);

                    cmd.ExecuteNonQuery();

                    conn.Close();
                    DialogResult dialogResult = MessageBox.Show("Class Deleted!", "Information!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (dialogResult == DialogResult.OK)
                    {
                        fetchUserData("Delete_Trainer");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            MainMenuForm mainMenuForm = new MainMenuForm();
            mainMenuForm.Show();
            this.Hide();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (txtSearch.Text != "")
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT * FROM Users WHERE User_Name LIKE '%' +@search+ '%' ", conn);
                    cmd.Parameters.AddWithValue("@search", txtSearch.Text);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    conn.Close();
                    if (dataTable.Rows.Count > 0)
                    {
                        // Clear existing columns
                        dataGridView1.Columns.Clear();

                        // Set the data source
                        dataGridView1.DataSource = dataTable;

                        // Add columns to the DataGridView            
                        DataGridViewButtonColumn editButtonColumn = new DataGridViewButtonColumn();
                        editButtonColumn.Text = "Edit";
                        editButtonColumn.UseColumnTextForButtonValue = true;
                        // Set the button column's cell style to have a green background color
                        editButtonColumn.DefaultCellStyle.BackColor = Color.Green;
                        dataGridView1.Columns.Add(editButtonColumn);

                        DataGridViewButtonColumn deleteButtonColumn = new DataGridViewButtonColumn();
                        deleteButtonColumn.Text = "Delete";
                        deleteButtonColumn.UseColumnTextForButtonValue = true;
                        deleteButtonColumn.DefaultCellStyle.BackColor = Color.Red;
                        dataGridView1.Columns.Add(deleteButtonColumn);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void rdbAdmin_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbAdmin.Checked)
            {
                try
                {
                    // search the role by admin
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT * FROM Users WHERE Role=@role", conn);
                    cmd.Parameters.AddWithValue("@role", "admin");
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    conn.Close();
                    if (dataTable.Rows.Count > 0)
                    {
                        dataGridView1.DataSource = dataTable;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void rdbUser_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbUser.Checked)
            {
                //search by the role user
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT * FROM Users WHERE Role=@role", conn);
                    cmd.Parameters.AddWithValue("@role", "user");
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    conn.Close();
                    if (dataTable.Rows.Count > 0)
                    {
                        dataGridView1.DataSource = dataTable;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            fetchUserData("Refresh");
        }
    }
}
