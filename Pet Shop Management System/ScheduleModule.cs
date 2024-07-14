using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Pet_Shop_Management_System
{
    public partial class ScheduleModule : Form
    {
        SqlConnection cn = new SqlConnection();
        SqlCommand cm = new SqlCommand();
        DbConnect dbcon = new DbConnect();
        SqlDataReader dr;
        bool check = false;
        Dashboard dashboard;
        string title = "Pet Shop Management System";
        public ScheduleModule(Dashboard dash)
        {
            InitializeComponent();
            cn = new SqlConnection(dbcon.connection());
            dashboard = dash;
            LoadSchedule();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            CheckField();
            if (check==true)
            {
                if (checkSchedule(lblcid.Text) == false)
                {
                    cm = new SqlCommand("INSERT INTO tbSchedule(scname)VALUES(@scname)", cn);
                    cm.Parameters.AddWithValue("@scname", txtName.Text);
                    cn.Open();
                    cm.ExecuteNonQuery();
                    cn.Close();
                    MessageBox.Show("Pet Schedule has been successfully registered!", title);
                    LoadSchedule();
                }
                else if (checkSchedule(lblcid.Text) == true)
                {
                    cm = new SqlCommand("UPDATE tbSchedule SET scname=@scname WHERE scid=@scid", cn);
                    cm.Parameters.AddWithValue("@scid", lblcid.Text);
                    cm.Parameters.AddWithValue("@scname", txtName.Text);
                    cn.Open();
                    cm.ExecuteNonQuery();
                    cn.Close();
                    MessageBox.Show("Schedule has been successfully update!", title);
                    LoadSchedule();
                }

            }
            Clear();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadSchedule();
        }
        #region method

        public void LoadSchedule()
        {
            int i = 0;
            dgvSchedule.Rows.Clear();
            cm = new SqlCommand("SELECT * FROM tbSchedule WHERE CONCAT(scid,scname) LIKE '%" + txtSearch.Text + "%'", cn);
            cn.Open();
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dgvSchedule.Rows.Add(i, dr[0].ToString(), dr[1].ToString());
            }
            dr.Close();
            cn.Close();
        }
        public bool checkSchedule(string Ccode)
        {
            bool flag = false;
            cn.Open();
            string sql = "SELECT scid FROM tbSchedule WHERE scid='" + Ccode + "'";
            SqlDataAdapter dap = new SqlDataAdapter(sql, cn);
            DataTable table = new DataTable();
            dap.Fill(table);
            if (table.Rows.Count > 0)
                flag = true;
            cn.Close();
            return flag;
        }
        public void CheckField()
        {
            if (txtName.Text == "")
            {
                MessageBox.Show("Required data field!", "Warning");
                return;
            }
            check = true;
        }
        
        public void Clear()
        {
            lblcid.Text = "0";
            txtName.Clear();
        }
        #endregion method

        private void dgvSchedule_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            lblcid.Text = dgvSchedule.Rows[e.RowIndex].Cells[1].Value.ToString();
            txtName.Text = dgvSchedule.Rows[e.RowIndex].Cells[2].Value.ToString();
        }

        private void dgvSchedule_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dgvSchedule.Columns[e.ColumnIndex].Name;
            if (colName == "Delete")
            {
                if (MessageBox.Show("Are you sure you want to delete this items?", "Delete Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    dbcon.executeQuery("DELETE FROM tbScheduleDetail WHERE scid LIKE '" + dgvSchedule.Rows[e.RowIndex].Cells[1].Value.ToString() + "'");
                    dbcon.executeQuery("DELETE FROM tbSchedule WHERE scid LIKE '" + dgvSchedule.Rows[e.RowIndex].Cells[1].Value.ToString() + "'");

                    MessageBox.Show("Item record has been successfully removed!", title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            LoadSchedule();
            Clear();
        }
    }
}
