﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GymManagementSystem
{
    public partial class MemberManagementForm : Form
    {
        // sql connection
        SqlConnection conn = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\ASUS\\Documents\\GymManagement.mdf;Integrated Security=True;Connect Timeout=30");
        public MemberManagementForm()
        {
            InitializeComponent();
        }

        private void MemberManagementForm_Load(object sender, EventArgs e)
        {
            fetchMemberData("FormLoad");
        }

        // get all the member data 
        private void fetchMemberData(string methodName)
        {
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Members", conn);
                cmd.ExecuteNonQuery();
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
                MessageBox.Show(ex.Message);
            }
        }

        private void MemberManagementForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            MainMenuForm mainMenuForm = new MainMenuForm();
            mainMenuForm.Show();
            this.Hide();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 8)
            {
                // edit case
                int memberID = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
                string name = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
                string email = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
                string address = dataGridView1.Rows[e.RowIndex].Cells[4].Value.ToString();
                double weight = Convert.ToDouble(dataGridView1.Rows[e.RowIndex].Cells[5].Value.ToString());
                double height = Convert.ToDouble(dataGridView1.Rows[e.RowIndex].Cells[6].Value.ToString());
                string joinDate = dataGridView1.Rows[e.RowIndex].Cells[7].Value.ToString();

                // redirect to member edit form
                EditMemberForm editMemberForm = new EditMemberForm(memberID, name, email, address, weight, height, joinDate);
                editMemberForm.Show();
                this.Hide();
            }
            else if (e.ColumnIndex == 9)
            {
                // delete case
                int memberID = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
                Member_Delete(memberID);
            }
        }
        private void Member_Delete(int memberID)
        {
            DialogResult result = MessageBox.Show("Do you want to delete this memebr?", "Alert!", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == DialogResult.OK)
            {
                // run delete query
                conn.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Members WHERE Member_ID=@memberID", conn);
                cmd.Parameters.AddWithValue("@memberID", memberID);
                cmd.ExecuteNonQuery();
                conn.Close();
                DialogResult result1 = MessageBox.Show("Member Deleted!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (result1 == DialogResult.OK)
                {
                    fetchMemberData("DeleteForm"); // call the fetch method again in order to view the updated data
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
            // if search txt box is not empty then process
            if (txtSearch.Text != "")
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT * FROM Members WHERE Name LIKE '%' + @search + '%' ", conn);
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

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            fetchMemberData("Refresh");
        }
    }
}
